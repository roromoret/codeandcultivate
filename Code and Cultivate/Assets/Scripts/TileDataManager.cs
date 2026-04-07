using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Tilemaps;

public class TileDataManager : MonoBehaviour
{
    public static TileDataManager Instance { get; private set; }

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap cropTilemap;

    private Dictionary<Vector2Int, TileData> _tiles = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
        BuildTileMap();
    }

    private void BuildTileMap()
    {
        _tiles.Clear();

        // 1: scan child GameObjects of Tilemap_Ground
        // currently, the ground is made of 3D prefab tiles, so this might need to be changed if
        // the world is going to be instantiated in the future
        int seeded = 0;
        foreach (Transform child in groundTilemap.transform)
        {
            // WorldToCell handles XZY swizzle internally
            Vector3Int cell   = groundTilemap.WorldToCell(child.position);
            Vector2Int coords = new Vector2Int(cell.x, cell.y);

            if (!_tiles.ContainsKey(coords))
            {
                _tiles[coords] = new TileData(coords, TileType.Normal);
                seeded++;
            }
        }

        Debug.Log($"[TileDataManager] Seeded {seeded} ground tiles.");

        // 2: scan Tilemap_Crops objects and override tile type
        // Objects on the crops layer are identified by tag.
        OverrideTilesFromObjects("Rock",        TileType.Rock);
        OverrideTilesFromObjects("CropFruit",   TileType.Fruit);
        OverrideTilesFromObjects("CropVeg",     TileType.Vegetable);
        OverrideTilesFromObjects("CropBerry",   TileType.Berry);

        DebugTilemap(); // DEBUGGING
    }

    private void OverrideTilesFromObjects(string tag, TileType type)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            Vector3Int cell   = cropTilemap.WorldToCell(obj.transform.position);
            Vector2Int coords = new Vector2Int(cell.x, cell.y);

            if (_tiles.TryGetValue(coords, out TileData tile))
            {
                tile.Type     = type;
                tile.Occupant = type != TileType.Rock ? OccupantType.Crop : OccupantType.None;
            }
            else
            {
                Debug.LogWarning($"[TileDataManager] {tag} at {coords} " +
                    $"(cell:{cell}, world:{obj.transform.position}) has no ground tile.");
            }
        }
    }

    // PUBLIC QUERY API

    public bool TryGetTile(Vector2Int coords, out TileData tile)
        => _tiles.TryGetValue(coords, out tile);

    public TileData GetTile(Vector2Int coords)
    {
        _tiles.TryGetValue(coords, out TileData tile);
        return tile;   // null if coords not in map
    }

    public bool IsWalkable(Vector2Int coords)
        => _tiles.TryGetValue(coords, out TileData tile) && tile.IsWalkable;

    // Called by Farmer.cs after each move
    public void SetFarmerPosition(Vector2Int previous, Vector2Int current)
    {
        if (_tiles.TryGetValue(previous, out TileData prev)) prev.IsFarmerPresent = false;
        if (_tiles.TryGetValue(current,  out TileData curr)) curr.IsFarmerPresent = true;
    }

    // Called by CropManager when a crop is planted or removed
    public void SetCrop(Vector2Int coords, CropData crop, TileType type)
    {
        if (!_tiles.TryGetValue(coords, out TileData tile)) return;
        tile.Type     = type;
        tile.Crop     = crop;
        tile.Occupant = crop != null ? OccupantType.Crop : OccupantType.None;
    }


    // TEMPORARY METHOD
    private void DebugTilemap()
    {
        Debug.Log("=== GROUND TILEMAP BOUNDS ===");
        Debug.Log($"cellBounds: {groundTilemap.cellBounds}");
        Debug.Log($"Cell size: {groundTilemap.cellSize}");
        Debug.Log($"Tilemap origin: {groundTilemap.origin}");

        // Print the first 5 cells that actually exist
        int printed = 0;
        foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (!groundTilemap.HasTile(cell)) continue;
            Vector3 world = groundTilemap.CellToWorld(cell);
            Debug.Log($"Ground cell {cell} → world {world}");
            if (++printed >= 5) break;
        }

        Debug.Log("=== CROP OBJECT POSITIONS ===");
        string[] tags = { "Rock", "CropFruit", "CropVeg", "CropBerry" };
        foreach (string tag in tags)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objs)
            {
                Vector3    world    = obj.transform.position;
                Vector3Int fromXY   = cropTilemap.WorldToCell(new Vector3(world.x, world.y, 0));
                Vector3Int fromXZ   = cropTilemap.WorldToCell(new Vector3(world.x, world.z, 0));
                Debug.Log($"[{tag}] world:{world}  cell_via_XY:{fromXY}  cell_via_XZ:{fromXZ}");
            }
        }

        Debug.Log($"Tilemap name: {groundTilemap.name}");
        Debug.Log($"Tilemap component: {groundTilemap.GetType()}");

        // Check compressed bounds — this is more reliable than cellBounds for sparse tilemaps
        groundTilemap.CompressBounds();
        Debug.Log($"Compressed bounds: {groundTilemap.cellBounds}");

        // Count tiles manually
        int count = 0;
        foreach (Vector3Int pos in groundTilemap.cellBounds.allPositionsWithin)
            if (groundTilemap.HasTile(pos)) count++;
        Debug.Log($"Tile count after compress: {count}");
    }
}