using UnityEngine;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Mono.Cecil;

// instantiates prefabs, seeds TileDataManager
public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private WorldData  worldData;
    [SerializeField] private Transform  tileParent; // empty GameObject to hold spawned tiles
    [SerializeField] private Transform  farmer;

    [SerializeField] private float      groundSpawnHeight   = -1f;
    [SerializeField] private float      cropHeightOffset    = 1f;
    
    // NEW : Added a specific height for farmer spawning to avoid underground spawns
    [SerializeField] private float      farmerSpawnHeight   = 0.96f; 

    [Header("Farmers")]
    [SerializeField] private List<GameObject>   farmerPrefab;

    // generated layout built procedurally then passed to TileDataManager
    private TileType[,] _grid;

    // center tile coords (reused by FarmerSpawner)
    private int _centreCol;
    private int _centreRow;

    private void Awake()
    {
        GenerateLayout();
        SpawnTiles();
        SpawnInitialFarmer();
        TileDataManager.Instance.InitialiseFromGrid(_grid, worldData.width, worldData.height);
    }

    // UPDATED : Now uses farmerSpawnHeight instead of a hardcoded 0f
    public Vector3 GetCenterWorldPosition()
    => GetWorldPosition(_centreCol, _centreRow, farmerSpawnHeight);


    //
    // Step 1 - Building the tile type layout
    //
    private void GenerateLayout()
    {
        _grid = new TileType[worldData.width, worldData.height];
        _centreCol = worldData.width  / 2;
        _centreRow = worldData.height / 2;


        // Initialise all tiles as normal
        for (int row = 0; row < worldData.height; row++)
            for (int col = 0; col < worldData.width; col++)
                _grid[col, row] = TileType.Normal;
        
        // Build a shuffled list of all tile coords to pick from randomly
        List<Vector2Int> available = GetShuffledCoords();

        // Place special tiles up to its max
        PlaceSpecialTiles(available, TileType.Rock,      worldData.maxRocks);
        PlaceSpecialTiles(available, TileType.Fruit,     worldData.maxFruits);
        PlaceSpecialTiles(available, TileType.Vegetable, worldData.maxVegetables);
        PlaceSpecialTiles(available, TileType.Berry,     worldData.maxBerries);

        // ensure center tile is always walkable
        _grid[_centreCol, _centreRow] = TileType.Normal;
    }

    private void PlaceSpecialTiles(List<Vector2Int> available, TileType type, int max)
    {
        int placed = 0;

        for (int i = 0; i < available.Count && placed < max; i++)
        {
            Vector2Int coord = available[i];

            if (_grid[coord.x, coord.y] != TileType.Normal) continue;

            _grid[coord.x, coord.y] = type;
            placed++;
        }
        // remove used coords so subsequent tile types don't overlap
        available.RemoveAll(c => _grid[c.x, c.y] == type);

        Debug.Log($"[WorldGenerator] Placed {placed} {type} tiles.");
    }
    
    private List<Vector2Int> GetShuffledCoords()
    {
        List<Vector2Int> coords = new();

        for (int row = 0; row < worldData.height; row++)
            for (int col = 0; col < worldData.width; col++)
                coords.Add(new Vector2Int(col, row));
        
        // I'm using Fisher-Yates shuffle for this
        for (int i = coords.Count - 1; i > 0; i--)
        {
            int j               = Random.Range(0, i + 1);
            Vector2Int temp     = coords[i];
            coords[i]           = coords[j];
            coords[j]           = temp;
        }

        return coords;
    }


    // 
    // Step 2 - Spawn tile GameObjects from the layout
    //
    private void SpawnTiles()
    {
        for (int row = 0; row < worldData.height; row++)
        {
            for (int col = 0; col < worldData.width; col++)
            {
                TileType    type    = _grid[col, row];
                Vector3     basePos = GetWorldPosition(col, row, groundSpawnHeight); 

                // always spawn ground beneath every tile
                if (worldData.groundPrefab != null)
                {
                    GameObject ground = Instantiate(worldData.groundPrefab, basePos, Quaternion.identity, tileParent);
                    ground.name = $"Tile_{col}_{row}_Ground";
                }

                // Spawn special tile on top if applicable
                GameObject specialPrefab = GetPrefabForType(type);
                if (specialPrefab != null)
                {
                    Vector3 specialPos = new Vector3(basePos.x, basePos.y + cropHeightOffset, basePos.z);
                    GameObject special = Instantiate(specialPrefab, specialPos, Quaternion.identity, tileParent);
                    special.name = $"Tile_{col}_{row}_{type}";

                    // register crop with CropManager if this is a crop tile
                    if (type != TileType.Rock)
                    {
                        CropVisual visual = special.GetComponent<CropVisual>();
                        if (visual != null) CropManager.Instance.RegisterCrop(new Vector2Int(col, row), type, visual);
                        else Debug.LogWarning($"[WorldGenerator] No CropVisual on {special.name} - add CropVisual component to the prefab");
                    }
                }
            }
        }

        Debug.Log("[WorldGenerator] Spawned tiles for a " + $"{worldData.width}x{worldData.height} grid");
    }

    private GameObject GetPrefabForType(TileType type)
    {
        return type switch
        {
            TileType.Rock       => worldData.rockPrefab,
            TileType.Fruit      => worldData.fruitPrefab,
            TileType.Vegetable  => worldData.vegetablePrefab,
            TileType.Berry      => worldData.berryPrefab,
            _                   => null
        };
    }
    

    //
    // Step 3 - Delegate first farmer spawn to FarmerSpawner
    //
    private void SpawnInitialFarmer()
    {
        if (FarmerSpawner.Instance == null)
        {
            Debug.LogError("[WorldGenerator] FarmerSpawner Instance is null - check FarmerManager in the scene and its script");
            return;
        }

        FarmerSpawner.Instance.SpawnInitialFarmer(GetCenterWorldPosition());
        Debug.Log($"[WorldGenerator] Center tile is ({_centreCol}, {_centreRow})");
    }

    
    // World position helpers
    public static Vector3 GetWorldPosition(int col, int row, float y, int gridWidth, int gridHeight)
    {   
        // shift the origin so the world is centred on (0,0) rather than building only +X and +Z
        float offsetX = -(gridWidth  / 2f);
        float offsetZ = -(gridHeight / 2f);

        float x = col + offsetX;
        float z = row + offsetZ;

        return new Vector3(x, y, z);
    }

    private Vector3 GetWorldPosition(int col, int row, float y)
    => GetWorldPosition(col, row, y, worldData.width, worldData.height);
}