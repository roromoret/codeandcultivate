using UnityEngine;
using System.Collections;

public class TutorialConditionChecker : MonoBehaviour
// attach to TutorialManager GameObject
// add new condition checks as new TutorialTrigger types are introduced
{
    [Header("Shop tutorial threshold (matches TutorialData asset setting)")]
    [SerializeField] private int moneyThresholdForShop = 10;

    private bool _shopTutorialChecked;

    private void OnEnable() => StartCoroutine(SubscribeWhenReady());
    private void OnDisable()
    {
        if (ResourceManager.Instance != null) ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
    }

    private IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() =>
            ResourceManager.Instance  != null &&
            TutorialManager.Instance  != null);

        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
    }

    private void OnResourceChanged(ResourceType type, int newAmount)
    {
        // Shop tutorial - fires once when money first reaches the threshold
        if (!_shopTutorialChecked &&
            type      == ResourceType.Money &&
            newAmount >= moneyThresholdForShop)
        {
            _shopTutorialChecked = true;
            TutorialManager.Instance.TryTrigger(
                TutorialTrigger.OnResourceThreshold,
                ResourceType.Money,
                newAmount);
        }

        // Add further threshold checks here as new tutorials are added
    }
}
