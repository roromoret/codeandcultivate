using UnityEngine;

[CreateAssetMenu(
        fileName = "WorldData",
        menuName = "Code and Cultivate/World Data" 
        )]

// ScriptableObject defining the grid
public class WorldData : ScriptableObject
{
    [Header("Grid dimensions")]
    public int width    = 10;
    public int height   = 10;

    // Flat array representing a 2D grid - index with [row * width + col]
    // Assign in Inspector
    public TileDefinition[] tiles;

    public TileDefinition GetTile(int col, int row)
    {
        int index = row * width + col;
        if (index < 0 || index >= tiles.Length) return null;
        return tiles[index];
    }
}
