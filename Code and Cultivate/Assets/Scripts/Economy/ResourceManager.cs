using System;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private ResourceConfig config;
    private Dictionary<ResourceType, int> _resources = new();
    public event Action<ResourceType, int> OnResourceChanged;
    private Dictionary<ResourceType, ResourceConfig.ResourceEntry> _configLookup;

    // Init
    private void Awake()
    {
        Debug.Log("[ResourceManager] Awake called");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[ResourceManager] Instance set successfully");

        InitialiseResources();
    }

    private void InitialiseResources()
    {
        _resources.Clear();

        if (config != null) _configLookup = config.BuildLookup();
        else
        {
            Debug.LogWarning("[ResourceManager] No ResourceConfig assigned");
            _configLookup = new();
        }

        // Seed every ResourceType to 0 so keys exist
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            _resources[type] = 0;

        // Apply starting amounts from ResourceConfig
        if (config == null)
        {
            Debug.LogWarning("[ResourceManager] No ResourceConfig assigned - default is 0");
            return;
        }

        var lookup = config.BuildLookup();
        foreach (var kvp in lookup)
            _resources[kvp.Key] = Mathf.Max(0, kvp.Value.startingAmount);

        Debug.Log("[ResourceManager] Resources initialised");
    }

    // Public API
    public int Get(ResourceType type)
    {
        _resources.TryGetValue(type, out int amount);
        return amount;
    }

    public void Add(ResourceType type, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[ResourceManager] Add called with non-positive amount {amount} for {type}");
            return;
        }

        _resources[type] += amount;
        Debug.Log($"[ResourceManager] +{amount} {type} -> total: {_resources[type]}");
        OnResourceChanged?.Invoke(type, _resources[type]);
    }
    
    public bool Spend(ResourceType type, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[ResourceManager] Spend called with non-positive amount {amount} for {type}");
            return false;
        }

        if (_resources[type] < amount)
        {
            Debug.Log($"[ResourceManager] Spend called with insiffucient {type} - have {_resources[type]}, need {amount}");
            return false;
        }

        _resources[type] -= amount;
        Debug.Log($"[ResourceManager] -{amount} {type} -> total: {_resources[type]}");
        OnResourceChanged?.Invoke(type, _resources[type]);
        return true;
    }

    public bool CanAfford(ResourceType type, int amount)
        => _resources.TryGetValue(type, out int current) && current >= amount;


    public Sprite GetIcon(ResourceType type)
    {
        _configLookup.TryGetValue(type, out ResourceConfig.ResourceEntry entry);
        return entry?.icon;
    }

    public string GetDisplayName(ResourceType type)
    {
        _configLookup.TryGetValue(type, out ResourceConfig.ResourceEntry entry);
        return entry?.displayName ?? type.ToString();
    }
}
