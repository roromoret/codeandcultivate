using UnityEngine;
using System.Collections;

public class TutorialConditionChecker : MonoBehaviour
// attach to TutorialManager GameObject
// add new condition checks as new TutorialTrigger types are introduced
{
    [Header("Shop tutorial threshold (matches TutorialData asset setting)")]
    [SerializeField] private int moneyThresholdForShop = 10;

    private bool _shopTutorialChecked;
    private bool _farmingTutorialChecked;

    private void OnEnable() => StartCoroutine(SubscribeWhenReady());
    private void OnDisable()
    {
        if (ResourceManager.Instance != null) ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
        Farmer.OnTileChanged -= WhenFarmerIsOnResourceTile;
    }

    private IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() =>
            ResourceManager.Instance  != null &&
            TutorialManager.Instance  != null);

        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
        Debug.Log("[TutorialConditionChecker] Subscribed to ResourceManager.Instance.OnResourceChanged");
        Farmer.OnTileChanged += WhenFarmerIsOnResourceTile;
        Debug.Log("[TutorialConditionChecker] Subscribed to Farmer.OnTileChanged");
    }

    private void OnResourceChanged(ResourceType type, int newAmount)
    {
        // Shop tutorial - fires once when money first reaches the threshold
        if (_shopTutorialChecked) return;
        if (type != ResourceType.Money) return;
        if (newAmount < moneyThresholdForShop) return;

        _shopTutorialChecked = true;
        TutorialManager.Instance.TryTrigger(
            TutorialTrigger.OnResourceThreshold,
            ResourceType.Money,
            newAmount);
    }
    
    private void WhenFarmerIsOnResourceTile(Vector2Int tile)
    {
        // Farming tutorial - fires once when farmer is on top of a resource tile
        if (_farmingTutorialChecked) return;
        if (!TileDataManager.Instance.TryGetTile(tile, out TileData tileData)) return;
        if (tileData.Occupant != OccupantType.Crop) return;

        _farmingTutorialChecked = true;
        TutorialManager.Instance.TryTrigger(
            TutorialTrigger.WhenFarmerIsOnResourceTile
        );
    }

    // Add further threshold checks here as new tutorials are added
}
