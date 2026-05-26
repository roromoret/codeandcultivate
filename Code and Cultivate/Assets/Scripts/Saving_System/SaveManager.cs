using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    private string _savePath;
    private const string SAVE_FILENAME = "gamesave.json";
    private SaveData _lastLoadedData; // Store data until GameScene initializes
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _savePath = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        Debug.Log($"[SaveManager] Save path: {_savePath}");
    }




    // Save character position and all resources to file
    public void Save()
    {
        Farmer mainFarmer = FarmerSpawner.Instance?.Farmers.Count > 0 ?
        FarmerSpawner.Instance.Farmers[0] : null;
        
        SaveData data = new SaveData();
        
        // Save character position
        data.farmerPosition = new Vector3Data(mainFarmer.transform.position);
        Debug.Log($"[SaveManager] Saving position: {mainFarmer.transform.position}");
        
        // Save all resources
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            int amount = ResourceManager.Instance.Get(type);
            data.resources.Add(new SaveData.ResourceData { type = type, amount = amount });
            Debug.Log($"[SaveManager] Saving {type}: {amount}");
        }
        
        // Write to file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"[SaveManager] Game saved successfully");
    }
    
    // Load character position and resources from file
    public void Load()
    {
        if (!File.Exists(_savePath))
        {
            Debug.LogWarning($"[SaveManager] Save file not found: {_savePath}");
            return;
        }
        
        try
        {
            string json = File.ReadAllText(_savePath);
            _lastLoadedData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[SaveManager] Save file loaded into memory");
            ApplyLoadedData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Error loading save file: {e.Message}");
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

        Farmer mainFarmer = FarmerSpawner.Instance?.Farmers.Count > 0 ?
        FarmerSpawner.Instance.Farmers[0] : null;
        
        if (mainFarmer == null || ResourceManager.Instance == null)
        {
            Debug.LogError("[SaveManager] Cannot apply data - Farmer or ResourceManager not found");
            return;
        }
        
        // Restore character position
        Vector3 loadedPosition = _lastLoadedData.farmerPosition.ToVector3();
        mainFarmer.transform.position = loadedPosition;
        Debug.Log($"[SaveManager] Applied position: {loadedPosition}");
        
        // Restore resources
        foreach (var resourceData in _lastLoadedData.resources)
        {
            int currentAmount = ResourceManager.Instance.Get(resourceData.type);
            if (currentAmount > resourceData.amount)
            {
                ResourceManager.Instance.Spend(resourceData.type, currentAmount - resourceData.amount);
            }
            else if (currentAmount < resourceData.amount)
            {
                ResourceManager.Instance.Add(resourceData.type, resourceData.amount - currentAmount);
            }
            Debug.Log($"[SaveManager] Applied {resourceData.type}: {resourceData.amount}");
        }
        
        _lastLoadedData = null; // Clear after applying
    }
    
    // Check if loaded data is pending application
    public bool HasPendingLoadData()
    {
        return _lastLoadedData != null;
    }
    
    // Check if a save file exists
    public bool SaveExists()
    {
        return File.Exists(_savePath);
    }
    

    // Get info about the save file
    public string GetSaveInfo()
    {
        if (!File.Exists(_savePath))
            return "No save found";
            
        try
        {
            string json = File.ReadAllText(_savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.saveDate;
        }
        catch
        {
            return "Error reading save";
        }
    }
    

    // Delete the save file
    public void DeleteSave()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            Debug.Log($"[SaveManager] Save file deleted");
        }
    }
}