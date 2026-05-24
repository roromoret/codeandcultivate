using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Rendering;

public class ShopUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ShopConfig config;

    [Header("Panel")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button     closeButton;

    [Header("Buy section")]
    [SerializeField] private Transform  buyContent;
    [SerializeField] private GameObject buyRowPrefab;

    [Header("Sell Section")]
    [SerializeField] private Transform  sellContent;
    [SerializeField] private GameObject sellRowPrefab;
    [SerializeField] private GameObject noSellItemsText;   // "Nothing to sell" label

    [Header("Feedback")]
    [SerializeField] private TMP_Text   feedbackText;
    [SerializeField] private float      feedbackDuration = 3f;

    private Coroutine _feedbackCoroutine;

    
    // Unity cycle
    private void Awake()
    {
        closeButton.onClick.AddListener(CloseShop);
        shopPanel.SetActive(false);
    }

    private void Start()
    {
        if (ShopManager.Instance != null) ShopManager.Instance.OnTransactionResult += OnTransactionResult;
    }

    private void OnDestroy()
    {
        if (ShopManager.Instance != null) ShopManager.Instance.OnTransactionResult -= OnTransactionResult;
    }


    // Open and close panel
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        PopulateBuySection();
        PopulateSellSection();
        ClearFeedback();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        ClearAllRows();
        ClearFeedback();
    }


    // Populate shop
    private void PopulateBuySection()
    {
        ClearRows(buyContent);

        foreach(ShopConfig.BuyEntry entry in config.buyItems)
        {
            var row = Instantiate(buyRowPrefab, buyContent).GetComponent<ShopBuyRowUI>();
            row.Initialise(entry);
        }
    }
    
    private void PopulateSellSection()
    {
        ClearRows(sellContent);

        bool anyAvailable = false;

        foreach (ShopConfig.SellEntry entry in config.sellItems)
        {
            if (ResourceManager.Instance.Get(entry.resourceType) <= 0) continue; // change to 1 to prevent softlocking?

            anyAvailable = true;
            var row = Instantiate(sellRowPrefab, sellContent).GetComponent<ShopSellRowUI>();
            row.Initialise(entry);
        }

        if (noSellItemsText != null) noSellItemsText.SetActive(!anyAvailable);
    }

    
    // Feedback
    private void OnTransactionResult(bool success, string msg)
    {
        feedbackText.text = msg;
        feedbackText.color = success ? Color.green : Color.red;

        if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);
        _feedbackCoroutine = StartCoroutine(ClearFeedbackAfterDelay());
        
        PopulateSellSection(); // Refresh sell section so amounts stay accurate after each transaction
    }

    private IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        ClearFeedback();
    }

    private void ClearFeedback()
    {
        if (_feedbackCoroutine == null) return;

        StopCoroutine(_feedbackCoroutine);
        _feedbackCoroutine = null;

        if (feedbackText.text == null) feedbackText.text = string.Empty;
    }


    // Clear row helpers
    private static void ClearRows(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--) Destroy(parent.GetChild(i).gameObject);
    }

    private void ClearAllRows()
    {
        ClearRows(buyContent);
        ClearRows(sellContent);
    }
}
