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

    [Header("Tile prefabs")]
    public GameObject groundPrefab;
    public GameObject rockPrefab;
    public GameObject fruitPrefab;
    public GameObject vegetablePrefab;
    public GameObject berryPrefab;

    [Header("Max spawns per special tile type")]
    public int maxRocks      = 5;
    public int maxFruits = 4;
    public int maxVegetables = 4;
    public int maxBerries    = 4;
}
