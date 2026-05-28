using System.Collections.Generic;
using UnityEngine;

// Attach to any scene GameObject (NOT the Farmer prefab).
// Builds a composite IFarmerActions that broadcasts every block command to all
// live farmers, then registers it once with ExecutableBlock.
// When a new farmer is spawned, FarmerSpawner.OnFarmerSpawned triggers a refresh
// so the composite stays current without requiring a full scene restart.
public class FarmerBlockBridge : MonoBehaviour
{
    private FarmerBroadcaster _broadcaster;

    private void Awake()
    {
        _broadcaster = new FarmerBroadcaster();
        ExecutableBlock.RegisterFarmer(_broadcaster);
    }

    private void OnEnable() => FarmerSpawner.OnFarmerSpawned += RefreshBroadcaster;
    private void OnDisable() => FarmerSpawner.OnFarmerSpawned -= RefreshBroadcaster;

    private void Start()
    {
        RefreshBroadcaster();
    }

    private void RefreshBroadcaster()
    {
        _broadcaster.SetFarmers(FarmerSpawner.Instance?.Farmers);
        Debug.Log($"[FarmerBlockBridge] Broadcaster updated - controlling {_broadcaster.FarmerCount} farmer(s).");
    }


    // composite IFarmerActions - fans out every call to all registered farmers
    private class FarmerBroadcaster : IFarmerActions
    {
        private readonly List<IFarmerActions> _farmers = new();

        public int FarmerCount => _farmers.Count;

        public void SetFarmers(IReadOnlyList<Farmer> farmers)
        {
            _farmers.Clear();
            if (farmers == null) return;
            foreach (var f in farmers) _farmers.Add(f);
        }

        // IsBusy is true if ANY farmer is still busy - blocks wait for all to finish
        public bool IsBusy
        {
            get
            {
                foreach (var f in _farmers) if (f.IsBusy) return true;
                return false;
            }
        }

        public void MoveNorth() => Broadcast(f => f.MoveNorth());
        public void MoveSouth() => Broadcast(f => f.MoveSouth());
        public void MoveEast() => Broadcast(f => f.MoveEast());
        public void MoveWest() => Broadcast(f => f.MoveWest());
        public void Plant() => Broadcast(f => f.Plant());
        public void Harvest() => Broadcast(f => f.Harvest());

        private void Broadcast(System.Action<IFarmerActions> action)
        {
            foreach (var f in _farmers) action(f);
        }
    }
}