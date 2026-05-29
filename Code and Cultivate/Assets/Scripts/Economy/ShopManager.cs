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
        
        bool effectSucceeded = ApplyEffect(entry, quantity, out string effectMsg);

        if (!effectSucceeded)
        {
            OnTransactionResult?.Invoke(false, effectMsg);
            return false;
        }
        
        ResourceManager.Instance.Spend(ResourceType.Money, totalCost);
        OnTransactionResult?.Invoke(true, effectMsg);
        return true;
    }

    private bool ApplyEffect(ShopConfig.BuyEntry entry, int quantity, out string effectMsg)
    {
        switch (entry.effect)
        {
            case ShopConfig.BuyItemEffect.SpawnFarmer:
                return TryApplySpawnFarmer(entry, out effectMsg);
            
            case ShopConfig.BuyItemEffect.None:
            default:
                // TODO: wire to an inventory manager when that system is implemented
                effectMsg = $"Bought {quantity}x {entry.displayName} for {entry.buyPrice*quantity} money.";
                return true;
        }
    }

    private bool TryApplySpawnFarmer(ShopConfig.BuyEntry entry, out string effectMsg)
    {
        if (FarmerSpawner.Instance == null)
        {
            effectMsg = "Cannot hire a farmer right now - the farmer spawner is unavailable";
            return false;
        }

        WorldGenerator wg = UnityEngine.Object.FindFirstObjectByType<WorldGenerator>();
        if (wg == null)
        {
            effectMsg = "Cannot hire a farmer right now - the world was not found";
            return false;
        }

        Vector3 spawnPos = wg.GetCenterWorldPosition();
        bool    spawned  = FarmerSpawner.Instance.TrySpawnAdditionalFarmer(spawnPos); 

        if (spawned)
        {
            effectMsg = $"Hired a new farmer for ${entry.buyPrice}!";
            return true;
        }

        else
        {
            effectMsg = "Cannot hire a farmer right now - the center tile is occupied. Move your farmer(s) off the center tile!";
            return false;
        }
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
