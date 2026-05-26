using System.Collections;
using System.Reflection;
using UnityEngine;

public class Farmer : MonoBehaviour, IFarmerActions
{
    public static event System.Action<Vector2Int, Farmer> OnTileChanged;
    public string FarmerName { get; set; } = "Farmer"; // label shown on logs
    public bool IsBusy { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f; // units per second during tween

    private void Start()
    {
        if (WorldGrid.Instance != null) transform.position = WorldGrid.Instance.SnapToGrid(transform.position);
        // WorldGenerator sets position first, then this corrects subunit drift
    }


    // IFarmerActions
    public void MoveNorth() => TryMove(Vector3.forward);
    public void MoveSouth() => TryMove(Vector3.back);
    public void MoveEast()  => TryMove(Vector3.right);
    public void MoveWest()  => TryMove(Vector3.left);

    public void Plant()
    {
        if (IsBusy) return;
        StartCoroutine(PlantRoutine());
    }

    public void Harvest()
    {
        if (IsBusy) return;
        StartCoroutine(HarvestRoutine());
    }


    // Internal movement
    private void TryMove(Vector3 direction)
    {
        if (IsBusy) return;

        Vector3     targetWorld = WorldGrid.Instance.SnapToGrid(transform.position + direction);
        Vector2Int  targetTile  = WorldGrid.Instance.WorldToTile(targetWorld);

        Debug.Log($"[{FarmerName}] Attempting to move to tile {targetTile}");

        if (!TileDataManager.Instance.IsWalkable(targetTile))
        {
            Debug.Log($"[{FarmerName}] Tile {targetTile} is not walkable - move blocked");
            return;
        }

        StartCoroutine(MoveRoutine(targetWorld));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        IsBusy = true;

        Vector2Int  previousTile    = WorldGrid.Instance.WorldToTile(transform.position);
        Vector3     start           = transform.position;
        float       duration        = Vector3.Distance(start, target) / moveSpeed;
        float       elapsed         = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
 
        transform.position = target;
 
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        TileDataManager.Instance.SetFarmerPosition(previousTile, currentTile);
        Debug.Log($"[{FarmerName}] OnTileChanged firing for tile {currentTile}");
        OnTileChanged?.Invoke(currentTile, this);
 
        yield return new WaitForSeconds(0.2f);  // wait time until next action
        IsBusy = false;
    }

    private IEnumerator PlantRoutine()
    {
        IsBusy = true;
 
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        Debug.Log($"[{FarmerName}] Attempting to plant at tile {currentTile}");
 
        if (!TileDataManager.Instance.TryGetTile(currentTile, out TileData tile))
        {
            Debug.Log($"[{FarmerName}] Plant failed - no tile data found at {currentTile}");
            IsBusy = false;
            yield break;
        }
 
        Debug.Log($"[{FarmerName}] Tile type at {currentTile}: {tile.Type}");
 
        ResourceType? resource = TileTypeToResource(tile.Type);
        if (!resource.HasValue)
        {
            Debug.Log($"[{FarmerName}] Plant failed - {tile.Type} is a NORMAL tile and right now " +
                      "the code doesn't support being able to choose what crop to plant");
            IsBusy = false;
            yield break;
        }
 
        if (!ResourceManager.Instance.CanAfford(resource.Value, 1))
        {
            Debug.Log($"[{FarmerName}] Plant failed - insufficient {resource.Value} " +
                      $"(have {ResourceManager.Instance.Get(resource.Value)}, need 1)");
            IsBusy = false;
            yield break;
        }
 
        yield return new WaitForSeconds(0.5f); // placeholder animation time
 
        bool spent = ResourceManager.Instance.Spend(resource.Value, 1);
 
        if (spent)
            Debug.Log($"[{FarmerName}] Planted at {currentTile} - spent 1 {resource.Value}. " +
                      $"Remaining: {ResourceManager.Instance.Get(resource.Value)}");
        else
            Debug.Log($"[{FarmerName}] Plant failed - Spend returned false for {resource.Value}");
 
        IsBusy = false;

    }

        private IEnumerator HarvestRoutine()
    {
        IsBusy = true;
 
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        CropData   cropData    = CropManager.Instance.GetCropData(currentTile);
 
        Debug.Log($"[{FarmerName}] Attempting to harvest at tile {currentTile}");
 
        if (!TileDataManager.Instance.TryGetTile(currentTile, out TileData tile))
        {
            Debug.Log($"[{FarmerName}] Harvest failed - no tile data found at {currentTile}");
            IsBusy = false;
            yield break;
        }
 
        if (tile.Occupant != OccupantType.Crop)
        {
            Debug.Log($"[{FarmerName}] Harvest failed - {currentTile} is not a crop tile");
            IsBusy = false;
            yield break;
        }
 
        if (cropData == null)
        {
            Debug.Log($"[{FarmerName}] Harvest failed - CropManager has no data at {currentTile}");
            IsBusy = false;
            yield break;
        }
 
        if (!cropData.IsMature)
            Debug.Log($"[{FarmerName}] Harvesting immature crop at {currentTile} - no yield expected");
 
        yield return new WaitForSeconds(0.5f); // placeholder animation time
 
        int           harvestYield = CropManager.Instance.Harvest(currentTile);
        ResourceType? resource     = TileTypeToResource(cropData.CropType);
 
        if (resource.HasValue && harvestYield > 0)
            ResourceManager.Instance.Add(resource.Value, harvestYield);
 
        IsBusy = false;
    }

    // Maps TileType to corresponding ResourceType - returns null for Normal and Rock
    private ResourceType? TileTypeToResource(TileType tileType)
    {
        return tileType switch
        {
            TileType.Fruit      => ResourceType.Fruits,
            TileType.Vegetable  => ResourceType.Vegetables,
            TileType.Berry      => ResourceType.Berries,
            _                   => null
        };
    }
}
