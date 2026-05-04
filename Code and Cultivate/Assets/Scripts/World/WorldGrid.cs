using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public static WorldGrid Instance { get; private set; }

    [Header("Tile dimensions (match Tilemap cell size)")]
    public float CellWidth = 1f;
    public float CellHeight = 1f;

    public float GridOffsetX { get; private set; }
    public float GridOffsetZ { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetGridOffset(float offsetX, float offsetZ)
    {
        GridOffsetX = offsetX;
        GridOffsetZ = offsetZ;
    }

    // Snap any world position to the nearest tile
    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        float snappedX = Mathf.Floor((worldPos.x - GridOffsetX) / CellWidth) * CellWidth  + GridOffsetX;
        float snappedZ = Mathf.Floor((worldPos.z - GridOffsetZ) / CellHeight) * CellHeight + GridOffsetZ;
        return new Vector3(snappedX, worldPos.y, snappedZ);
    }

    // Convert a tile coordinate (int col, int row) to its world center
    public Vector3 TileToWorld(int col, int row, float y = 0f)
    {
        return new Vector3(col * CellWidth + GridOffsetX, y, row * CellHeight + GridOffsetZ);
    }

    // Convert a world position back to tile indices
    public Vector2Int WorldToTile(Vector3 worldPos)
    {
        int col = Mathf.FloorToInt((worldPos.x - GridOffsetX) / CellWidth);
        int row = Mathf.FloorToInt((worldPos.z - GridOffsetZ) / CellHeight);
        return new Vector2Int(col, row);
    }
}
