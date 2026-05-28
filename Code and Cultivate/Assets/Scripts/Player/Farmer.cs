using System.Collections;
using UnityEngine;
using TMPro; // added for error text

public class Farmer : MonoBehaviour, IFarmerActions
{
    // Note for later: If you add multiple farmers, this Singleton (Instance) pattern will need to be removed or refactored!
    public static Farmer Instance { get; private set; }
    public static event System.Action<Vector2Int> OnTileChanged;

    // Visual fields added for handling the 2D character sheets and flip container
    [Header("Visuals")]
    [SerializeField] private Animator farmerAnimator;
    [SerializeField] private Transform visualContainer;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f; // units per second during tween

    // newly added error feedback settings
    [Header("Error Feedback")]
    [SerializeField] private TMP_Text errorTextUI; // changed to TMP_Text to support 3D floating text inside the prefab
    [SerializeField] private AudioSource errorAudioSource;
    [SerializeField] private float stunDuration = 2f;

    // newly added farming settings
    [Header("Farming")]
    [SerializeField] private int seedCost = 1;
    [SerializeField] private float cropSpawnHeight = 0f; // Set this to match WorldGenerator's (groundSpawnHeight + cropHeightOffset)
    [SerializeField] private GameObject carrotPrefab; // index 0
    [SerializeField] private GameObject applePrefab;  // index 1
    [SerializeField] private GameObject berryPrefab;  // index 2
    
    // Automatically found in Awake, no longer in the inspector
    private Transform cropsContainer; 

    public bool IsBusy { get; private set; }
    
    // Init the singleton in Awake to ensure it happens before any Start methods try to access it
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Auto-find the UI text in the scene if it hasn't been assigned
        if (errorTextUI == null)
        {
            // IMPORTANT: The GameObject must be named exactly "ErrorText" and be ACTIVE in the scene
            GameObject textObj = GameObject.Find("ErrorText");
            if (textObj != null)
            {
                errorTextUI = textObj.GetComponent<TMP_Text>();
            }
            else
            {
                Debug.LogWarning("[Farmer] Could not find 'ErrorText' in the scene! Make sure it is active and spelled correctly.");
            }
        }
        
        // hide the error text by default on awake
        if (errorTextUI != null) errorTextUI.gameObject.SetActive(false);

        // Auto-find the TileParent in the scene to keep hierarchy clean when spawning crops
        GameObject parentObj = GameObject.Find("TileParent");
        if (parentObj != null)
        {
            cropsContainer = parentObj.transform;
        }
        else
        {
            Debug.LogWarning("[Farmer] Could not find 'TileParent' in the scene! New crops will be spawned at the root of the hierarchy.");
        }
    }

    // IFarmerActions Implementation
    public void MoveNorth() => TryMove(Vector3.forward);
    public void MoveSouth() => TryMove(Vector3.back);
    public void MoveEast()  => TryMove(Vector3.right);
    public void MoveWest()  => TryMove(Vector3.left);

    public void Plant(int cropDropdownIndex)
    {
        if (IsBusy) return;
        StartCoroutine(PlantRoutine(cropDropdownIndex));
    }

    public void Harvest()
    {
        if (IsBusy) return;
        StartCoroutine(HarvestRoutine());
    }

    // Error feedback routine (Stun)
    private IEnumerator TriggerStun(string errorMessage)
    {
        IsBusy = true; // lock the farmer

        if (errorTextUI != null)
        {
            errorTextUI.text = errorMessage;
            errorTextUI.color = Color.red;
            errorTextUI.gameObject.SetActive(true);
        }

        if (errorAudioSource != null)
        {
            errorAudioSource.Play();
        }

        // wait for the stun to finish
        yield return new WaitForSeconds(stunDuration);

        if (errorTextUI != null)
        {
            errorTextUI.gameObject.SetActive(false);
        }

        IsBusy = false; // unlock the farmer
    }
    
    // Internal movement
    private void TryMove(Vector3 direction)
    {
        if (IsBusy) return;

        Vector3 targetWorld    = WorldGrid.Instance.SnapToGrid(transform.position + direction);
        Vector2Int targetTile  = WorldGrid.Instance.WorldToTile(targetWorld);
        
        Debug.Log($"[Farmer] Attempting to move to tile {targetTile}");

        if (!TileDataManager.Instance.IsWalkable(targetTile)) 
        {   
            Debug.Log($"[Farmer] Tile {targetTile} is not walkable - move blocked");
            // trigger stun instead of just returning silently
            StartCoroutine(TriggerStun("Path Blocked!"));
            return;
        }

        // Pass the heading direction to the movement coroutine to drive animations
        StartCoroutine(MoveRoutine(targetWorld, direction));
    }

    private IEnumerator MoveRoutine(Vector3 target, Vector3 direction)
    {
        IsBusy = true;

        // Start walking animation right before moving
        UpdateAnimator(direction, true);

        Vector2Int previousTile = WorldGrid.Instance.WorldToTile(transform.position);
        Vector3 start = transform.position;
        float   dist  = Vector3.Distance(start, target);
        float   duration = dist / moveSpeed;
        float   elapsed  = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        transform.position = target;

        // Switch back to idle state once the tile destination is reached
        UpdateAnimator(Vector3.zero, false);

        // Notify tile manager when the farmer moves
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        TileDataManager.Instance.SetFarmerPosition(previousTile, currentTile);
        Debug.Log($"[Farmer] In MoveRoutine - OnTileChanged firing for tile {currentTile}");
        OnTileChanged?.Invoke(currentTile);

        yield return new WaitForSeconds(0.2f);
        IsBusy = false;
    }

    // Handles passing state variables to the blend trees and flips the sprite renderers
    private void UpdateAnimator(Vector3 direction, bool isMoving)
    {
        if (farmerAnimator == null) return;

        farmerAnimator.SetBool("IsMoving", isMoving);

        if (direction != Vector3.zero)
        {
            // Flips the whole container along the X-axis for left/right directions
            if (visualContainer != null)
            {
                if (direction.x < -0.1f) visualContainer.localScale = new Vector3(-1, 1, 1);
                else if (direction.x > 0.1f) visualContainer.localScale = new Vector3(1, 1, 1);
            }

            farmerAnimator.SetFloat("InputX", direction.x);
            farmerAnimator.SetFloat("InputY", direction.z); 
        }
    }

    private IEnumerator PlantRoutine(int cropIndex)
    {
        IsBusy = true;
        
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        Debug.Log($"[Farmer] Attempting to plant at tile {currentTile}");

        // Get tile data
        if (!TileDataManager.Instance.TryGetTile(currentTile, out TileData tile))
        {
            Debug.Log($"[Farmer] Plant failed - no tile data found at {currentTile}");
            yield return StartCoroutine(TriggerStun("Cannot plant here!"));
            yield break;
        }

        Debug.Log($"[Farmer] Tile type at {currentTile}: {tile.Type}");

        // map the dropdown index to the correct resource type and prefab
        // assuming based on UI prefab: 0 = Carrots, 1 = Apple, 2 = Berry
        ResourceType targetResource = ResourceType.Vegetables;
        TileType cropTileType = TileType.Vegetable;
        GameObject prefabToSpawn = carrotPrefab;

        if (cropIndex == 1) 
        {
            targetResource = ResourceType.Fruits;
            cropTileType = TileType.Fruit;
            prefabToSpawn = applePrefab;
        }
        else if (cropIndex == 2) 
        {
            targetResource = ResourceType.Berries;
            cropTileType = TileType.Berry;
            prefabToSpawn = berryPrefab;
        }

        // Check player can afford to plant
        if (!ResourceManager.Instance.CanAfford(targetResource, seedCost))
        {
            Debug.Log($"[Farmer] Plant failed - insufficient {targetResource}");
            yield return StartCoroutine(TriggerStun($"Not enough {targetResource}!"));
            yield break;
        }

        yield return new WaitForSeconds(0.5f); // placeholder animation time

        bool spent = ResourceManager.Instance.Spend(targetResource, seedCost);

        if (spent)
        {
            Debug.Log($"[Farmer] Planted at {currentTile} - spent {seedCost} {targetResource}. " +
                    $"Remaining: {ResourceManager.Instance.Get(targetResource)}");

            // instantiate the correct crop visual model on the tile
            CropVisual spawnedVisual = null;
            if (prefabToSpawn != null)
            {
                // Rely on WorldGrid to get the exact tile center and apply our custom spawn height
                Vector3 spawnPosition = WorldGrid.Instance.TileToWorld(currentTile.x, currentTile.y, cropSpawnHeight);
                
                // Spawn the crop AND set its parent to keep the hierarchy clean
                GameObject newObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, cropsContainer);
                spawnedVisual = newObj.GetComponent<CropVisual>();
            }

            // Register it with the system (this will also update the sprite based on growth stage)
            CropManager.Instance.RegisterCrop(currentTile, cropTileType, spawnedVisual);
        }
        else 
        {
            Debug.Log($"[Farmer] Plant failed - Spend returned false for {targetResource}");
            yield return StartCoroutine(TriggerStun("Failed to use seeds!"));
            yield break;
        }

        IsBusy = false;
    }

    private IEnumerator HarvestRoutine()
    {
        IsBusy = true;
        
        Vector2Int currentTile  = WorldGrid.Instance.WorldToTile(transform.position);
        CropData   cropData     = CropManager.Instance.GetCropData(currentTile);

        Debug.Log($"[Farmer] Attempting to harvest at tile {currentTile}");

        // Get tile data
        if (!TileDataManager.Instance.TryGetTile(currentTile, out TileData tile))
        {
            Debug.Log($"[Farmer] Harvest failed - no tile data found at {currentTile}");
            yield return StartCoroutine(TriggerStun("Nothing here!"));
            yield break;
        }

        if (tile.Occupant != OccupantType.Crop)
        {
            Debug.Log($"[Farmer] Harvest failed - {currentTile} is not a crop tile");
            yield return StartCoroutine(TriggerStun("Not a crop tile!"));
            yield break;
        }

        if (cropData == null)
        {
            Debug.Log($"[Farmer] Harvest failed - CropManager has no data at {currentTile}");
            yield return StartCoroutine(TriggerStun("Error reading crop!"));
            yield break;
        }

        if (!cropData.IsMature)
        {
            Debug.Log($"[Farmer] Harvesting immature crop at {currentTile} - no yield expected");
            yield return StartCoroutine(TriggerStun("Crop is not ready!"));
            yield break;
        }
        
        yield return new WaitForSeconds(0.5f); // placeholder animation time
        
        int yield_amount = CropManager.Instance.Harvest(currentTile);
        ResourceType? resource = TileTypeToResource(cropData.CropType);

        if (resource.HasValue && yield_amount > 0) ResourceManager.Instance.Add(resource.Value, yield_amount);

        IsBusy = false;
    }

    // Private helper - Maps TileType to corresponding ResourceType - returns null if normal or rock
    private ResourceType? TileTypeToResource(TileType tileType)
    {
        return tileType switch
        {
            TileType.Fruit      => ResourceType.Fruits,
            TileType.Vegetable  => ResourceType.Vegetables,
            TileType.Berry      => ResourceType.Berries,
            _                   => null
        };
    }

    // Init
    void Start()
    {
        // Snap to grid immediately on spawn (handles any imprecise placement)
        if (WorldGrid.Instance != null)
            transform.position = WorldGrid.Instance.SnapToGrid(transform.position);
    }
}