using System.Collections;
using System.Transactions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Farmer : MonoBehaviour, IFarmerActions
{
    public static Farmer Instance { get; private set; }
    public static event System.Action<Vector2Int> OnTileChanged;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f; // units per second during tween

    public bool IsBusy { get; private set; }
    
    // Init the singleton in Awake to ensure it happens before any Start methods try to access it
    private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    
    Instance = this;
}





    // 
    // IFarmerActions Implementation
    //
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

    //
    // Internal movement
    //
    private void TryMove(Vector3 direction)
    {
        if (IsBusy) return;

        Vector3 targetWorld    = WorldGrid.Instance.SnapToGrid(transform.position + direction);
        Vector2Int targetTile  = WorldGrid.Instance.WorldToTile(targetWorld);
        
        Debug.Log($"[Farmer] Attempting to move to tile {targetTile}");

        if (!TileDataManager.Instance.IsWalkable(targetTile)) 
        {   
            Debug.Log($"[Farmer] Tile {targetTile} is not walkable - move blocked");
            return;
        }

        StartCoroutine(MoveRoutine(targetWorld));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        IsBusy = true;

        Vector2Int previousTile = WorldGrid.Instance.WorldToTile(transform.position);
        Vector3 start = transform.position;
        float   dist  = Vector3.Distance(start, target);
        float   duration = dist / moveSpeed;
        float   elapsed  = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        transform.position = target;

        // Notify tile manager when the farmer moves
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        TileDataManager.Instance.SetFarmerPosition(previousTile, currentTile);
        Debug.Log($"[Farmer] In MoveRoutine - OnTileChanged firing for tile {currentTile}");
        OnTileChanged?.Invoke(currentTile);

        yield return new WaitForSeconds(0.2f);
        IsBusy = false;
    }

    private IEnumerator PlantRoutine()
    {
        IsBusy = true;
        
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        Debug.Log($"[Farmer] Attempting to plant at tile {currentTile}");

        // Get tile data
        if (!TileDataManager.Instance.TryGetTile(currentTile, out TileData tile))
        {
            Debug.Log($"[Farmer] Plant failed - no tile data found at {currentTile}");
            IsBusy = false;
            yield break;
        }

        Debug.Log($"[Farmer] Tile type at {currentTile}: {tile.Type}");

        // null fix. i hope i dont need this when the world is instantiated
        ResourceType? nullableResource = TileTypeToResource(tile.Type);
        if (!nullableResource.HasValue)
        {
            Debug.Log($"[Farmer] Plant failed - {tile.Type} is a NORMAL tile and right now the code doesn't support being able to choose what crop to plant");
            IsBusy = false;
            yield break;
        }

        // Resolve which resource this tile type maps to
        ResourceType? resource = TileTypeToResource(tile.Type);

        // Check player can afford to plant (-1 crop)
        if (!ResourceManager.Instance.CanAfford(resource.Value, 1))
        {
            Debug.Log($"[Farmer] Plant failed - insufficient {resource.Value} " +
                    $"(have {ResourceManager.Instance.Get(resource.Value)}, need 1)");
            IsBusy = false;
            yield break;
        }

        yield return new WaitForSeconds(0.5f); // placeholder animation time

        bool spent = ResourceManager.Instance.Spend(resource.Value, 1);

        if (spent)
        {
            Debug.Log($"[Farmer] Planted at {currentTile} - spent 1 {resource.Value}. " +
                    $"Remaining: {ResourceManager.Instance.Get(resource.Value)}");

            // TODO: update tile state and world visuals once world is instantiated at runtime
            // e.g. TileDataManager.Instance.SetCrop(currentTile, newCropData, tile.Type);
            // and spawn/update the crop model on Tilemap_Crops
        }
        else Debug.Log($"[Farmer] Plant failed - Spend returned false for {resource.Value}");

        IsBusy = false;
    }

    private IEnumerator HarvestRoutine()
    {
        IsBusy = true;
        
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        Debug.Log($"[Farmer] Attempting to harvest at tile {currentTile}");

        // Get tile data
        if (!TileDataManager.Instance.TryGetTile(currentTile, out TileData tile))
        {
            Debug.Log($"[Farmer] Plant failed - no tile data found at {currentTile}");
            IsBusy = false;
            yield break;
        }

        Debug.Log($"[Farmer] Tile type at {currentTile}: {tile.Type}");

        // Resolve which resource this tile type maps to
        ResourceType? resource = TileTypeToResource(tile.Type);

        // Break if not a resource tile
        if (resource == null)
        {
            Debug.Log($"Harvest failed - tile type {tile.Type} is not a crop tile");
            IsBusy = false;
            yield break;
        }

        yield return new WaitForSeconds(0.5f); // placeholder animation time

        ResourceManager.Instance.Add(resource.Value, 3);

            // TODO: update tile state and world visuals once world is instantiated at runtime
            // e.g. TileDataManager.Instance.SetCrop(currentTile, newCropData, tile.Type);
            // and spawn/update the crop model on Tilemap_Crops

        IsBusy = false;
    }

    // Private helper - Maps TileType to corresponding ResourceType - returns null if normal or rock
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

    // Init
    void Start()
    {
        // Snap to grid immediately on spawn (handles any imprecise placement)
        if (WorldGrid.Instance != null)
            transform.position = WorldGrid.Instance.SnapToGrid(transform.position);
    }
}
