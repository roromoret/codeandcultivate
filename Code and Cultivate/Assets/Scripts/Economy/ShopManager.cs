using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
    public event Action<bool, string> OnTransactionResult;
    
    // Event to notify blocks that they have just been unlocked
    public event Action<string> OnBlockUnlocked;

    // Memory list of blocks unlocked during this session
    private HashSet<string> _unlockedBlocks = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Allows blocks to check if they are unlocked at startup
    public bool IsBlockUnlocked(string blockId)
    {
        return _unlockedBlocks.Contains(blockId);
    }

    // Buy
    public bool TryBuy(ShopConfig.BuyEntry entry, int quantity)
    {
        if (entry.isOneTimePurchase && entry.effect == ShopConfig.BuyItemEffect.UnlockBlock)
        {
            if (_unlockedBlocks.Contains(entry.blockUnlockId))
            {
                OnTransactionResult?.Invoke(false, $"{entry.displayName} is already unlocked!");
                return false;
            }
            quantity = 1; 
        }

        int totalCost = entry.buyPrice * quantity;

        if (!ResourceManager.Instance.CanAfford(ResourceType.Money, totalCost))
        {
            int current = ResourceManager.Instance.Get(ResourceType.Money);
            OnTransactionResult?.Invoke(false, $"Not enough money! Required: {totalCost}, Owned: {current}.");
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
            
            case ShopConfig.BuyItemEffect.GiveResource:
                int totalAmount = entry.amountToGivePerPurchase * quantity;
                ResourceManager.Instance.Add(entry.resourceToGive, totalAmount);
                effectMsg = $"Bought {totalAmount}x {entry.resourceToGive}!";
                return true;

            case ShopConfig.BuyItemEffect.UnlockBlock:
                if (!_unlockedBlocks.Contains(entry.blockUnlockId))
                {
                    _unlockedBlocks.Add(entry.blockUnlockId);
                    OnBlockUnlocked?.Invoke(entry.blockUnlockId); // Notifies the UI block!
                }
                effectMsg = $"New block unlocked: {entry.displayName}!";
                return true;

            case ShopConfig.BuyItemEffect.None:
            default:
                // TODO: wire to an inventory manager when that system is implemented
                effectMsg = $"Bought {quantity}x {entry.displayName} for {entry.buyPrice * quantity} money.";
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