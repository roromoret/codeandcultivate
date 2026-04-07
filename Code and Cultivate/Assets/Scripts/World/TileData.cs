using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData
{
    public Vector2Int       Coords      { get; private set; }
    public TileType         Type        { get; set; }
    public OccupantType     Occupant    { get; set; }

    // Farmer.cs writes this when it enters/leaves a tile
    public bool IsFarmerPresent         { get; set; }

    public bool IsWalkable => Type != TileType.Rock;

    // Reference to crop data if Occupant == OccupantType.Crop.
    public CropData Crop                { get; set; } // null if no crop

    public TileData(Vector2Int coords, TileType type)
    {
        Coords   = coords;
        Type     = type;
        Occupant = OccupantType.None;
    }
}
