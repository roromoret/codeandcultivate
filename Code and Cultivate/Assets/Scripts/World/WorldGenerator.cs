using UnityEngine;

// instantiates prefabs, seeds TileDataManager
public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private WorldData  worldData;
    [SerializeField] private Transform  tileParent; // empty GameObject to hold spawned tiles
    [SerializeField] private GameObject defaultGroundPrefab;

    [SerializeField] private float      groundSpawnHeight   = -1f;
    [SerializeField] private float      cropHeightOffset    = 1f;

    private void Awake()
    {
        GenerateWorld();
    }
    
    private void GenerateWorld()
    {
        if (worldData == null)
        {
            Debug.LogError("[WorldGenerator] No WorldData assigned.");
            return;
        }

        for (int row = 0; row < worldData.height; row++)
        {
            for (int col = 0; col < worldData.width; col++)
            {
                TileDefinition  tileDef = worldData.GetTile(col, row);
                TileType        type    = tileDef != null ? tileDef.type : TileType.Normal;
                Vector3         basePos = WorldGrid.Instance.TileToWorld(col, row, groundSpawnHeight);

                // Always spawn a ground tile first at every position
                if (defaultGroundPrefab != null)
                {
                    GameObject ground = Instantiate(defaultGroundPrefab, basePos, Quaternion.identity, tileParent);
                    ground.name = $"Tile_{col}_{row}_Ground";
                }

                // If this tile has a special prefab (crop, rock), spawn it on top
                if (tileDef?.prefab != null && type != TileType.Normal)
                {
                    Vector3 cropPos = new Vector3(
                        basePos.x, basePos.y + cropHeightOffset, basePos.z);

                    GameObject special = Instantiate(tileDef.prefab, cropPos, Quaternion.identity, tileParent);
                    special.name = $"Tile_{col}_{row}_{type}";
                }
            }
        }

        Debug.Log($"[WorldGenerator] World generated ({worldData.width} x {worldData.height})");
        TileDataManager.Instance.InitialiseFromGrid(worldData);
    }
}
