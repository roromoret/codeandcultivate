// Tracks what non-farmer object is on a tile
// Farmer presence is stored in TileData
// I'm using an enum in case we need to reserve other occupant types for future use
public enum OccupantType
{
    None,
    Crop    // Managed by CropManager
}