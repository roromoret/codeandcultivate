using UnityEngine;

public enum TutorialTrigger
{
    OnGameStart,            // shown immediately after first boot: basic controls
    OnResourceThreshold,    // shown when a resource hits a target amount
    WhenFarmerIsOnResourceTile,         // shown after the farmer first moves on top of a resource tile 
    // future entries: OnFirstHarvest, OnShopUnlocked, etc
}
