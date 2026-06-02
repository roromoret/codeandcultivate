using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum ConditionType 
{ 
    WallNorth, 
    WallSouth, 
    WallWest, 
    WallEast, 
    PlantableSoil, 
    HarvestableSoil 
}

public abstract class ConditionalBlock : ExecutableBlock
{
    [Header("UI Elements")]
    public TMP_Dropdown conditionDropdown;
    public Toggle expectedStateToggle;
    
    protected bool EvaluateCondition()
    {
        if (conditionDropdown == null) return false;

        ConditionType currentCondition = (ConditionType)conditionDropdown.value;
        
        bool rawConditionResult = false;

        switch (currentCondition)
        {
            case ConditionType.WallNorth: 
                rawConditionResult = CheckWallNorth(); 
                break;
            case ConditionType.WallSouth: 
                rawConditionResult = CheckWallSouth(); 
                break;
            case ConditionType.WallWest:
                rawConditionResult = CheckWallWest(); 
                break;
            case ConditionType.WallEast:
                rawConditionResult = CheckWallEast(); 
                break;
            case ConditionType.PlantableSoil:
                rawConditionResult = CheckPlantableSoil(); 
                break;
            case ConditionType.HarvestableSoil:
                rawConditionResult = CheckHarvestableSoil(); 
                break;
        }

        if (expectedStateToggle == null) 
        {
            return rawConditionResult;
        }
        
        return rawConditionResult == expectedStateToggle.isOn;
    }

    private Farmer GetTargetFarmer()
    {
        ColumnExecutor parentColumn = GetComponentInParent<ColumnExecutor>();
        
        if (parentColumn == null)
        {
            return null;
        }

        if (FarmerAssignment.Instance != null)
        {
            return FarmerAssignment.Instance.GetAssigned(parentColumn);
        }
        
        return null;
    }

    private Vector2Int GetFarmerTile()
    {
        Farmer farmer = GetTargetFarmer();
        if (farmer == null) return Vector2Int.zero;
        
        return WorldGrid.Instance.WorldToTile(farmer.transform.position);
    }

    private bool IsWallAt(Vector2Int coords)
    {
        if (!TileDataManager.Instance.TryGetTile(coords, out TileData tile))
        {
            return true;
        }
        
        return tile.Type == TileType.Rock;
    }

    private bool CheckWallNorth()
    {
        Vector2Int farmerPos = GetFarmerTile();
        return IsWallAt(farmerPos + new Vector2Int(0, 1)); 
    }

    private bool CheckWallSouth()
    {
        Vector2Int farmerPos = GetFarmerTile();
        return IsWallAt(farmerPos + new Vector2Int(0, -1));
    }

    private bool CheckWallEast()
    {
        Vector2Int farmerPos = GetFarmerTile();
        return IsWallAt(farmerPos + new Vector2Int(1, 0));
    }

    private bool CheckWallWest()
    {
        Vector2Int farmerPos = GetFarmerTile();
        return IsWallAt(farmerPos + new Vector2Int(-1, 0));
    }

    private bool CheckPlantableSoil()
    {
        Vector2Int farmerPos = GetFarmerTile();
        
        if (!TileDataManager.Instance.TryGetTile(farmerPos, out TileData tile))
        {
            return false;
        }

        return tile.Type == TileType.Normal && tile.Occupant == OccupantType.None;
    }

    private bool CheckHarvestableSoil()
    {
        Vector2Int farmerPos = GetFarmerTile();
        
        if (!TileDataManager.Instance.TryGetTile(farmerPos, out TileData tile))
        {
            return false;
        }

        if (tile.Occupant != OccupantType.Crop)
        {
            return false;
        }

        var cropData = CropManager.Instance.GetCropData(farmerPos);
        return cropData != null && cropData.IsMature;
    }
}