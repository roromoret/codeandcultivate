using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceHUD : MonoBehaviour
{
    [System.Serializable]
    private class ResourceDisplay
    {
        public ResourceType type;
        public TMP_Text     label;      // e.g. "Fruits: 0"
        public Image        icon;       // assign in inspector
    }

    [SerializeField] private List<ResourceDisplay> displays;

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
            if (display.type == type)
            {
                UpdateLabel(display, newAmount);
                return;
            }
        }
    }

    private void RefreshAll()
    {
        foreach (ResourceDisplay display in displays)
        { 
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
}
