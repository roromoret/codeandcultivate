using System.Collections.Generic;
using System.Net;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;

public class TileDataManager : MonoBehaviour
{
    public static TileDataManager Instance { get; private set; }
    private Dictionary<Vector2Int, TileData> _tiles = new();

    private float _gridOffsetX;
    private float _gridOffsetZ;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
    }

    // INIT FROM GRID

    public void InitialiseFromGrid(TileType[,] grid, int width, int height)
    {
        _tiles.Clear();

        _gridOffsetX = -(width / 2f);
        _gridOffsetZ = -(height / 2f);

        WorldGrid.Instance.SetGridOffset(_gridOffsetX, _gridOffsetZ);

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                TileType    type    = grid[col, row];
                var         coords  = new Vector2Int(col, row);
                TileData    tile    = new TileData(coords, type);

                if (type != TileType.Normal && type != TileType.Rock) tile.Occupant = OccupantType.Crop;
                _tiles[coords] = tile;
            }
        }

        Debug.Log($"[TileDataManager] Initialised {_tiles.Count} tiles");
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
}