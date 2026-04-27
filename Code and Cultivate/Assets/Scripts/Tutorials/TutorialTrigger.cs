using UnityEngine;

public enum TutorialTrigger
{
    OnGameStart,            // shown immediately after first boot: basic controls
    OnResourceThreshold,    // shown when a resource hits a target amount
    // future entries: OnFirstHarvest, OnShopUnlocked, etc
}
