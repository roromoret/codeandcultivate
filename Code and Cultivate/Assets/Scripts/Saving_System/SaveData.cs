using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Character Position
    public Vector3Data farmerPosition;
    
    // Resources
    [System.Serializable]
    public class ResourceData
    {
        public ResourceType type;
        public int amount;
    }
    
    public List<ResourceData> resources;
    
    // Metadata
    public string saveDate;
    public string saveName; // Optional: let players name their saves
    
    public SaveData()
    {
        resources = new List<ResourceData>();
        saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveName = "Save " + saveDate;
    }
}

// Vector3 isn't serializable by default, so we need a wrapper
[System.Serializable]
public class Vector3Data
{
    public float x, y, z;
    
    public Vector3Data(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    
    public Vector3 ToVector3() => new Vector3(x, y, z);
    
    // Constructor for empty initialization
    public Vector3Data()
    {
        x = 0;
        y = 0;
        z = 0;
    }
}