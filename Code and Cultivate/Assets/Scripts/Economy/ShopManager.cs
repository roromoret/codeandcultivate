using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
    public event Action<bool, string> OnTransactionResult;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }


    // Buy
    public bool TryBuy(ShopConfig.BuyEntry entry, int quantity)
    {
        int totalCost = entry.buyPrice * quantity;

        if (!ResourceManager.Instance.CanAfford(ResourceType.Money, totalCost))
        {
            int current = ResourceManager.Instance.Get(ResourceType.Money);
            OnTransactionResult?.Invoke(false, $"Not enough money! Need {totalCost}, have {current}.");
            return false;
        }

        ResourceManager.Instance.Spend(ResourceType.Money, totalCost);
        // TODO: add item to player inventory when limited items and inventory system is implemented

        OnTransactionResult?.Invoke(true, $"Bought {quantity}x {entry.displayName} for {totalCost} money.");
        return true;
    }


    // Sell
    public bool TrySell(ShopConfig.SellEntry entry, int quantity)
    {
        if (!ResourceManager.Instance.CanAfford(entry.resourceType, quantity)){
            int current = ResourceManager.Instance.Get(entry.resourceType);
            string name = ResourceManager.Instance.GetDisplayName(entry.resourceType);
            OnTransactionResult?.Invoke(false, $"Not enough {name}! Have {current}, need {quantity}.");
            return false;
        }

        ResourceManager.Instance.Spend(entry.resourceType, quantity);

        int totalEarned = entry.sellPricePerUnit * quantity;
        ResourceManager.Instance.Add(ResourceType.Money, totalEarned);

        string displayName = ResourceManager.Instance.GetDisplayName(entry.resourceType);
        OnTransactionResult?.Invoke(true, $"Sold {quantity}x {displayName} for {totalEarned} money");
        return true;
    }
}
