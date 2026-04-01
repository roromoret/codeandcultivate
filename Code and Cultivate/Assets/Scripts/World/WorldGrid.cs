using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public static WorldGrid Instance { get; private set; }

    [Header("Tile dimensions (match Tilemap cell size)")]
    public float CellWidth = 1f;
    public float CellHeight = 1f;

    // The 0.5 offsets keep the farmer centered inside each tile.
    private const float OffsetX = 0.5f;
    private const float OffsetZ = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Snap any world position to the nearest tile center
    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        float snappedX = Mathf.Floor(worldPos.x / CellWidth)  * CellWidth  + OffsetX;
        float snappedZ = Mathf.Floor(worldPos.z / CellHeight) * CellHeight + OffsetZ;
        return new Vector3(snappedX, worldPos.y, snappedZ);
    }

    // Convert a tile coordinate (int col, int row) to its world center
    public Vector3 TileToWorld(int col, int row, float y = 0f)
    {
        return new Vector3(col * CellWidth + OffsetX, y, row * CellHeight + OffsetZ);
    }

    // Convert a world position back to tile indices
    public Vector2Int WorldToTile(Vector3 worldPos)
    {
        int col = Mathf.FloorToInt(worldPos.x / CellWidth);
        int row = Mathf.FloorToInt(worldPos.z / CellHeight);
        return new Vector2Int(col, row);
    }
}
