using System.Collections.Generic;
using UnityEngine;

public class FarmerSpawner : MonoBehaviour
{
    public static FarmerSpawner Instance { get; private set; }

    public static event System.Action OnFarmerSpawned;

    [Header("Prefabs")]
    [SerializeField] private GameObject mainFarmerPrefab;   // unique prefab for the first farmer
    [SerializeField] private GameObject helperFarmerPrefab; // shared prefab for all subsequent farmers

    [Header("Scene")]
    [SerializeField] private Transform farmerParent; // keeps hierarchy tidy
    
    private readonly List<Farmer> _farmers = new();
    public IReadOnlyList<Farmer> Farmers => _farmers;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    
    public void SpawnInitialFarmer(Vector3 position)
    {
        if (mainFarmerPrefab == null)
        {
            Debug.LogError("[FarmerSpawner] mainFarmerPrefab is not assigned");
            return;
        }

        Farmer farmer = InstantiateFarmer(mainFarmerPrefab, position, farmerIndex: 1);
        Debug.Log($"[FarmerSpawner] Initial farmer '{farmer.FarmerName}' spawned at {position}");
    }

    public bool TrySpawnAdditionalFarmer(Vector3 position)
    {
        if (helperFarmerPrefab == null)
        {
            Debug.LogError("[FarmerSpawner] additionalFarmerPrefab is not assigned.");
            return false;
        }
 
        if (IsTileOccupiedByFarmer(position))
        {
            Debug.Log("[FarmerSpawner] Spawn blocked - centre tile is already occupied by a farmer.");
            return false;
        }
 
        int    newIndex = _farmers.Count + 1;
        Farmer farmer   = InstantiateFarmer(helperFarmerPrefab, position, newIndex);
        Debug.Log($"[FarmerSpawner] Spawned '{farmer.FarmerName}' at {position}. " +
                  $"Total farmers: {_farmers.Count}.");
        return true;
    }

    private Farmer InstantiateFarmer(GameObject prefab, Vector3 position, int farmerIndex)
    {
        GameObject go     = Instantiate(prefab, position, Quaternion.identity, farmerParent);
        Farmer     farmer = go.GetComponent<Farmer>();
 
        if (farmer == null)
        {
            Debug.LogError($"[FarmerSpawner] Prefab '{prefab.name}' has no Farmer component.");
            Destroy(go);
            return null;
        }
 
        farmer.FarmerName   = farmerIndex == 1 ? "Farmer_1" : $"Farmer_{farmerIndex}";
        go.name             = farmer.FarmerName;
 
        _farmers.Add(farmer);
        OnFarmerSpawned?.Invoke();
        return farmer;
    }

    private bool IsTileOccupiedByFarmer(Vector3 worldPosition)
    {
        Vector2Int targetTile = WorldGrid.Instance.WorldToTile(worldPosition);
 
        foreach (Farmer farmer in _farmers)
        {
            if (farmer == null) continue; // guard against destroyed farmers
            Vector2Int farmerTile = WorldGrid.Instance.WorldToTile(farmer.transform.position);
            if (farmerTile == targetTile) return true;
        }
 
        return false;
    }
}
