using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Character Position
    public Vector3Data farmerPosition;
    public List<ColumnSaveData> workspaceColumns;
    
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
    public string saveName; // Optional: lets players name their saves
    
    public SaveData()
    {
        resources = new List<ResourceData>();
        saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveName = "Save " + saveDate;
    }
}




    [System.Serializable]
    public class BlockSaveData
    {
        public string blockType;        //C# class name, eg "LoopBlock"
        public List<string> blockParams;    //Dropdown values, eg loop counts, toggle states.
        public List<BlockSaveData> innerBlocks;     //inner content of blocks like loop / while.
        public List<BlockSaveData> innerBlocksAlt;      //IfElseBlock "else" branch.

        public BlockSaveData()
            {
                blockParams = new List<string>();
                innerBlocks = new List<BlockSaveData>();
                innerBlocksAlt = new List<BlockSaveData>();
            }
    }

    [System.Serializable]
    public class ColumnSaveData
    {
        public List<BlockSaveData> blocks;
        public ColumnSaveData()
        {
            blocks = new List<BlockSaveData>();
        }
    }









// Vector3 isn't serializable by default, so needs a wrapper
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