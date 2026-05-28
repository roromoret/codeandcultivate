using System.Collections.Generic;
using UnityEngine;

// Tracks which faremr is assigned to which ColumnExecutor
// ColumnFarmerSelector reads from here to build its dropdown
public class FarmerAssignment : MonoBehaviour
{
    public static FarmerAssignment Instance { get; private set; }
    public static event System.Action OnAssignmentsChanged;    
    private readonly Dictionary<ColumnExecutor, Farmer> _assignments = new();


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable() => FarmerSpawner.OnFarmerSpawned += NotifyChanged;
    private void OnDisable() => FarmerSpawner.OnFarmerSpawned -= NotifyChanged;

    private void NotifyChanged() => OnAssignmentsChanged?.Invoke();

    // called by ColumnFarmerSelector when the player picks a farmer from the dropdown
    public void Assign(ColumnExecutor column, Farmer farmer)
    {
        if (column == null) return;
        _assignments[column] = farmer;
        NotifyChanged();
    }

    // called by ColumnController.DeleteColumn so dead columns don't hold assignments
    public void Unassign(ColumnExecutor column)
    {
        if (_assignments.Remove(column)) NotifyChanged();
    }

    // returns farmer assigned to column
    public Farmer GetAssigned(ColumnExecutor column)
    {
        _assignments.TryGetValue(column, out Farmer farmer);
        return farmer;
    }

    // returns all farmers that are not yet assigned to any column
    // + always include the one already assigned to the calling column
    public List<Farmer> GetAvailableFarmers(Farmer alreadyAssignedToThisColumn)
    {
        var allFarmers  = FarmerSpawner.Instance.Farmers;
        var available   = new List<Farmer>();

        if (allFarmers == null) return available;

        var assignedElsewhere = new HashSet<Farmer>();
        foreach (var kvp in _assignments)
            if (kvp.Value != null && kvp.Value != alreadyAssignedToThisColumn) 
            assignedElsewhere.Add(kvp.Value);

        foreach (Farmer f in allFarmers)
            if (!assignedElsewhere.Contains(f)) available.Add(f);

        return available;
    }
}
