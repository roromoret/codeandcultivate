using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.Rendering;
using UnityEngine.Playables;

public class ResourceHUD : MonoBehaviour
{
    [System.Serializable]
    private class ResourceDisplay
    {
        public ResourceType type;
        public TMP_Text     label;      // e.g. "Fruits: 0"
        public Image        icon;       // assign in inspector
        public RectTransform notificationSpawnPoint;

        // Tracked internally
        [HideInInspector] public int previousAmount;
    }

    [SerializeField] private List<ResourceDisplay>     displays;
    [SerializeField] private ResourceDeltaNotification notificationPrefab;

    private void OnEnable() => StartCoroutine(SubscribeWhenReady());

    private void OnDisable()
    {
        // Always unsubscribe to avoid ghost listeners after scene reload
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
    }

    private IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() => ResourceManager.Instance != null);
        ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
        RefreshAll();
    }

    private void HandleResourceChanged(ResourceType type, int newAmount)
    {
        foreach (ResourceDisplay display in displays)
        {
            if (display.type != type) continue; 
            
            int delta = newAmount - display.previousAmount;
            display.previousAmount = newAmount;

            UpdateLabel(display, newAmount);
            SpawnNotification(display, delta);
            return;
        }
    }

    private void RefreshAll()
    {
        foreach (ResourceDisplay display in displays)
        {
            int amount = ResourceManager.Instance.Get(display.type);
            display.previousAmount = amount;
            UpdateLabel(display, ResourceManager.Instance.Get(display.type));
            ApplyIcon(display);
        }
    }

    private void UpdateLabel(ResourceDisplay display, int amount)
    {
        string name = ResourceManager.Instance.GetDisplayName(display.type);
        display.label.text = $"{name}: {amount}";
    }

    private void ApplyIcon(ResourceDisplay display)
    {
        if (display.icon == null) return;

        Sprite icon = ResourceManager.Instance.GetIcon(display.type);
        if (icon != null) display.icon.sprite = icon;
        else display.icon.gameObject.SetActive(false);
    }

    private void SpawnNotification(ResourceDisplay display, int delta)
    {
        if (notificationPrefab == null) return;
        if (display.notificationSpawnPoint == null) return;

        ResourceDeltaNotification notif = Instantiate(
            notificationPrefab,
            display.notificationSpawnPoint.position,
            Quaternion.identity,
            display.notificationSpawnPoint);

        notif.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        notif.Play(delta);
    }
}
