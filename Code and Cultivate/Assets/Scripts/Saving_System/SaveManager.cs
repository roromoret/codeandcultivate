using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public const int MAX_SLOTS = 3;
    private const string SAVE_FILENAME_FORMAT = "save_slot_{0}.json";

    private SaveData _lastLoadedData;

    private int _currentSlot = -1;
    public int CurrentSlot => _currentSlot;
    public void SetCurrentSlot(int slot) => _currentSlot = slot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[SaveManager] Save directory: {Application.persistentDataPath}");
    }

    private string GetSlotPath(int slot) =>
        Path.Combine(Application.persistentDataPath, string.Format(SAVE_FILENAME_FORMAT, slot));

    private bool IsValidSlot(int slot) => slot >= 1 && slot <= MAX_SLOTS;

    // Save character position, resources, and workspace blocks to a slot
    public void Save(int slot)
    {
        if (!IsValidSlot(slot))
        {
            Debug.LogError($"[SaveManager] Invalid slot: {slot}");
            return;
        }

        if (Farmer.Instance == null || ResourceManager.Instance == null)
        {
            Debug.LogError("[SaveManager] Cannot save - Farmer or ResourceManager not found");
            return;
        }

        SaveData data = new SaveData();

        data.farmerPosition = new Vector3Data(Farmer.Instance.transform.position);
        Debug.Log($"[SaveManager] Saving position: {Farmer.Instance.transform.position}");

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            int amount = ResourceManager.Instance.Get(type);
            data.resources.Add(new SaveData.ResourceData { type = type, amount = amount });
            Debug.Log($"[SaveManager] Saving {type}: {amount}");
        }

        if (WorkspaceManager.Instance != null)
        {
            data.workspaceColumns = WorkspaceManager.Instance.GetSaveData();
            Debug.Log($"[SaveManager] Saving {data.workspaceColumns.Count} workspace columns");
        }

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(GetSlotPath(slot), json);
        _currentSlot = slot;
        Debug.Log($"[SaveManager] Game saved to slot {slot}");
    }

    // Load a slot into memory — call before loading the game scene
    public void Load(int slot)
    {
        if (!IsValidSlot(slot))
        {
            Debug.LogError($"[SaveManager] Invalid slot: {slot}");
            return;
        }

        string path = GetSlotPath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] Save file not found: {path}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            _lastLoadedData = JsonConvert.DeserializeObject<SaveData>(json);
            _currentSlot = slot;
            Debug.Log($"[SaveManager] Slot {slot} loaded into memory");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Error loading slot {slot}: {e.Message}");
        }
    }

    // Apply previously loaded data to the game (called after scene loads)
    public void ApplyLoadedData()
    {
        if (_lastLoadedData == null)
        {
            Debug.LogWarning("[SaveManager] No loaded data to apply");
            return;
        }

        if (Farmer.Instance == null || ResourceManager.Instance == null)
        {
            Debug.LogError("[SaveManager] Cannot apply data - Farmer or ResourceManager not found");
            return;
        }

        Vector3 loadedPosition = _lastLoadedData.farmerPosition.ToVector3();
        Farmer.Instance.transform.position = loadedPosition;
        Debug.Log($"[SaveManager] Applied position: {loadedPosition}");

        foreach (var resourceData in _lastLoadedData.resources)
        {
            int currentAmount = ResourceManager.Instance.Get(resourceData.type);
            if (currentAmount > resourceData.amount)
                ResourceManager.Instance.Spend(resourceData.type, currentAmount - resourceData.amount);
            else if (currentAmount < resourceData.amount)
                ResourceManager.Instance.Add(resourceData.type, resourceData.amount - currentAmount);
            Debug.Log($"[SaveManager] Applied {resourceData.type}: {resourceData.amount}");
        }

        if (WorkspaceManager.Instance != null)
        {
            WorkspaceManager.Instance.RestoreFromSaveData(_lastLoadedData.workspaceColumns);
            Debug.Log($"[SaveManager] Applied workspace columns");
        }

        _lastLoadedData = null;
    }

    public bool HasPendingLoadData() => _lastLoadedData != null;

    // Returns true if a save file exists for the given slot
    public bool SlotExists(int slot) => IsValidSlot(slot) && File.Exists(GetSlotPath(slot));

    // Returns true if any slot has a save
    public bool AnySaveExists()
    {
        for (int i = 1; i <= MAX_SLOTS; i++)
            if (SlotExists(i)) return true;
        return false;
    }

    // Returns display info for a slot, or "Empty" if no file
    public string GetSlotInfo(int slot)
    {
        if (!SlotExists(slot))
            return "Empty";

        try
        {
            string json = File.ReadAllText(GetSlotPath(slot));
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
            return $"{data.saveName}  —  {data.saveDate}";
        }
        catch
        {
            return "Corrupted";
        }
    }

    // Delete a specific slot
    public void DeleteSlot(int slot)
    {
        if (!IsValidSlot(slot)) return;

        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveManager] Slot {slot} deleted");
        }
    }
}
