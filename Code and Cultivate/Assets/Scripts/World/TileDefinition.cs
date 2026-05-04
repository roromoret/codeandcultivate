using UnityEngine;

// per-tile data: type + prefab
[System.Serializable]
public class TileDefinition
{
    public TileType     type;
    public GameObject   prefab; // 3D model to spawn at this tile
}
