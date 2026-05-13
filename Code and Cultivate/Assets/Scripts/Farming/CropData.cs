using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class CropData
{
    public TileType    CropType     { get; set; }
    public GrowthStage Stage        { get; set; }

    [Header("Yield Config")]
    public int      BaseYield       { get; set; }
    public float    YieldVariance   { get; set; }   // random % added on top of baseYield

    public bool IsMature => Stage == GrowthStage.Mature;

    public CropData(TileType cropType, GrowthStage startingStage, int baseYield, float yieldVariance)
    {
        CropType        = cropType;
        Stage           = startingStage;
        BaseYield       = baseYield;
        YieldVariance   = yieldVariance;
    }

    // Crop yield: base + random % up to yield variance
    public int CalculateYield()
    {
        if (!IsMature) return 0;

        int bonus = Mathf.RoundToInt(BaseYield * Random.Range(0f, YieldVariance));
        return BaseYield + bonus;
    }

    public void GrowOneStage()
    {
        if (Stage < GrowthStage.Mature) Stage++;
    }
}