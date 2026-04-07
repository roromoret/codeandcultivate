using System.Collections;
using UnityEngine;

public class Farmer : MonoBehaviour, IFarmerActions
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f; // units per second during tween

    // IFarmerActions.IsBusy — block executor must check this before issuing the next command
    public bool IsBusy { get; private set; }
    
    // 
    // IFarmerActions Implementation
    //
    public void MoveNorth() => TryMove(Vector3.forward);
    public void MoveSouth() => TryMove(Vector3.back);
    public void MoveEast()  => TryMove(Vector3.right);
    public void MoveWest()  => TryMove(Vector3.left);

    public void Plant()
    {
        if (IsBusy) return;
        StartCoroutine(PlantRoutine());
    }

    public void Harvest()
    {
        if (IsBusy) return;
        StartCoroutine(HarvestRoutine());
    }

    //
    // Internal movement
    //
    private void TryMove(Vector3 direction)
    {
        if (IsBusy) return;

        Vector3 targetWorld    = WorldGrid.Instance.SnapToGrid(transform.position + direction);
        Vector2Int targetTile  = WorldGrid.Instance.WorldToTile(targetWorld);
        
        Debug.Log($"[Farmer] Attempting to move to tile {targetTile}");

        if (!TileDataManager.Instance.IsWalkable(targetTile)) 
        {   
            Debug.Log($"[Farmer] Tile {targetTile} is not walkable - move blocked");
            return;
        }

        StartCoroutine(MoveRoutine(targetWorld));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        IsBusy = true;

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

        // Notify tile manager when the farmer moves
        Vector2Int currentTile = WorldGrid.Instance.WorldToTile(transform.position);
        TileDataManager.Instance.SetFarmerPosition(previousTile, currentTile);

        yield return new WaitForSeconds(0.2f);
        IsBusy = false;
    }

    private IEnumerator PlantRoutine()
    {
        IsBusy = true;
        // TODO: implement planting routine
        Debug.Log($"Planting at {WorldGrid.Instance.WorldToTile(transform.position)}");
        yield return new WaitForSeconds(0.5f);
        IsBusy = false;
    }

    private IEnumerator HarvestRoutine()
    {
        IsBusy = true;
        // TODO: implement harvesting routine
        Debug.Log($"Harvesting at {WorldGrid.Instance.WorldToTile(transform.position)}");
        yield return new WaitForSeconds(0.5f);
        IsBusy = false;
    }

    //
    // Init
    //
    void Start()
    {
        // Snap to grid immediately on spawn (handles any imprecise placement)
        if (WorldGrid.Instance != null)
            transform.position = WorldGrid.Instance.SnapToGrid(transform.position);
    }
}
