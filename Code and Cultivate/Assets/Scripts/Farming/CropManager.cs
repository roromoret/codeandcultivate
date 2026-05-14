using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set; }

    [Header("Yield Settings")]
    [SerializeField] private int    baseYield       = 5;
    [SerializeField] private float  yieldVariance   = 0.2f;

    // Master crop registry (keyed by tile coords)
    private Dictionary<Vector2Int, (CropData data, CropVisual visual)> _crops = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        if (TurnManager.Instance != null) TurnManager.Instance.OnTurnPassed += OnTurnPassed;
    }

    private void OnDisable()
    {
        if (TurnManager.Instance != null) TurnManager.Instance.OnTurnPassed -= OnTurnPassed;
    }

    // Registration - called by WorldGenerator.SpawnTiles
    public void RegisterCrop(Vector2Int coords, TileType cropType, CropVisual visual)
    {
        // Random starting stage between 1 and 3
        GrowthStage startingStage = (GrowthStage) Random.Range(
            (int) GrowthStage.Sprout,
            (int) GrowthStage.Vegetative
            );
        
        CropData data = new CropData(cropType, startingStage, baseYield, yieldVariance);
        _crops[coords] = (data, visual);

        TileDataManager.Instance.SetCrop(coords, data, cropType);
        visual?.UpdateVisual(startingStage);

        Debug.Log($"[CropManager] Registered {cropType} at {coords} starting at stage {startingStage}");
    }

    // Turn advancement
    private void OnTurnPassed(int turnCount)
    {
        Debug.Log($"[CropManager] Advancing all crops (turn {turnCount})");

        foreach (var kvp in _crops)
        {
            CropData data       = kvp.Value.data;
            CropVisual visual   = kvp.Value.visual;

            if (data.IsMature) continue;

            data.GrowOneStage();
            visual?.UpdateVisual(data.Stage);
        }
    }

    // Harvest - called by Farmer.HarvestRoutine
    public int Harvest(Vector2Int coords)
    {
        if (!_crops.TryGetValue(coords, out var entry))
        {
            Debug.Log($"[CropManager] Harvest called at {coords} but no crop is registered");
            return 0;
        }

        CropData data   = entry.data;
        int      yield  = data.CalculateYield();

        Debug.Log($"[CropManager] Harvesting {data.CropType} at {coords} " +
        $"- stage: {data.Stage}, yield: {yield}");

        _crops.Remove(coords);
        TileDataManager.Instance.SetCrop(coords, null, TileType.Normal);
        if (entry.visual != null) Destroy(entry.visual.gameObject);

        return yield;
    }

    // Helpers
    public bool HasCropAt(Vector2Int coords) => _crops.ContainsKey(coords);
    public CropData GetCropData(Vector2Int coords)
    {
        _crops.TryGetValue(coords, out var entry);
        return entry.data;
    }
}
