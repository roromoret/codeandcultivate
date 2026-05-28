using UnityEngine;

/// ScriptableObject that defines all items available to buy and sell in the shop
/// Create via: Assets > Create > Code and Cultivate > Shop Config
/// Add new buy items or sell entries here without touching any other code.
[CreateAssetMenu(fileName = "ShopConfig", menuName = "Code and Cultivate/Shop Config")]
public class ShopConfig : ScriptableObject
{
    public enum BuyItemEffect
    {
        None,
        SpawnFarmer
    }

    [System.Serializable]
    public class BuyEntry
    {
        public string displayName;
        public Sprite icon;
        public int    buyPrice;
        public BuyItemEffect effect = BuyItemEffect.None;
        // TODO: Add ItemType enum field and wire to inventory manager when inventory system is implemented
    }
 
    [System.Serializable]
    public class SellEntry
    {
        public ResourceType resourceType;
        public int          sellPricePerUnit;
    }
 
    [Header("Items available to purchase")]
    public BuyEntry[] buyItems;
 
    [Header("Crops available to sell")]
    public SellEntry[] sellItems;
}

