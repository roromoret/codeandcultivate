using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// attached to the ScriptColumn prefab
// owns the farmer drowndown ui for that column
public class ColumnFarmerSelector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown farmerDropdown;

    private ColumnExecutor _executor;
    private bool _refreshing; // reentering guard

    private const string UNASSIGNED_LABEL = "(No Farmer)";

    private void Awake()
    {
        _executor = GetComponent<ColumnExecutor>();
    }

    private void OnEnable() => FarmerAssignment.OnAssignmentsChanged += RefreshDropdown;

    private void OnDisable() => FarmerAssignment.OnAssignmentsChanged -= RefreshDropdown;

    private void Start() => RefreshDropdown();


    // Rebuilds dropdown whenever farmers are spawned or assignments get changed
    private void RefreshDropdown()
    {
        if (farmerDropdown == null || FarmerAssignment.Instance == null) return;
        if (_refreshing) return;
        _refreshing = true;

        Farmer currentlyAssigned    = FarmerAssignment.Instance.GetAssigned(_executor);
        List<Farmer> available      = FarmerAssignment.Instance.GetAvailableFarmers(currentlyAssigned);

        farmerDropdown.ClearOptions();

        var labels = new List<string> { UNASSIGNED_LABEL };
        int selectIndex = 0;

        for (int i = 0; i < available.Count; i++)
        {
            labels.Add(available[i].FarmerName);
            if (available[i] == currentlyAssigned) selectIndex = i+1;
        }

        farmerDropdown.AddOptions(labels);
        farmerDropdown.SetValueWithoutNotify(selectIndex);
        farmerDropdown.onValueChanged.AddListener(OnDropdownChanged);

        _refreshing = false;
    }

    private void OnDropdownChanged(int index)
    {
        if (FarmerAssignment.Instance == null) return;
        if (_refreshing) return;

        if (index == 0) // unassigned
        {
            FarmerAssignment.Instance.Assign(_executor, null); 
            return;
        }

        Farmer currentlyAssigned    = FarmerAssignment.Instance.GetAssigned(_executor);
        List<Farmer> available      = FarmerAssignment.Instance.GetAvailableFarmers(currentlyAssigned);

        int farmerIndex = index-1;
        if (farmerIndex < available.Count) FarmerAssignment.Instance.Assign(_executor, available[farmerIndex]);
    }
}
