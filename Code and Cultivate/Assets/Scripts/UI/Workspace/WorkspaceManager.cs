using System.Collections.Generic;
using UnityEngine;

public class WorkspaceManager : MonoBehaviour
{
    public GameObject columnPrefab;
    public Transform workspaceContent;
    public GameObject addColumnButton;
    public ColumnPaginator paginator;

    // Singleton so other scripts (blocks, SaveManager) can easily reach this.
    public static WorkspaceManager Instance { get; private set; }

    // Helper struct used by the Inspector — pairs a class name with its prefab.
    [System.Serializable]
    public struct BlockPrefabEntry
    {
        public string     typeName;   // Must match the block's C# class name exactly.
        public GameObject prefab;     // The prefab to instantiate when restoring this block.
    }

    [Header("Block Prefab Registry — assign one entry per block type")]
    [SerializeField] private BlockPrefabEntry[] blockPrefabs;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Method to instantiate a column prefab
    public void AddNewColumn()
    {
        GameObject newColumn = Instantiate(columnPrefab, workspaceContent);

        // Place the new column just before the Add Column button (always last sibling)
        int lastIndex = workspaceContent.childCount - 1;
        newColumn.transform.SetSiblingIndex(lastIndex - 1);

        this.transform.SetAsLastSibling();

        ColumnController scriptColonne = newColumn.GetComponent<ColumnController>();
        if (scriptColonne != null)
        {
            scriptColonne.paginator = this.paginator;
        }

        if (paginator != null)
        {
            paginator.RefreshPagination();
        }
    }



        // Collects save data for every column in the workspace.
        // Captures block structure and the farmer assigned to each column.
    public List<ColumnSaveData> GetSaveData()
    {
        var result = new List<ColumnSaveData>();

        foreach (Transform child in workspaceContent)
        {
            ColumnExecutor executor = child.GetComponent<ColumnExecutor>();
            if (executor == null || executor.blocksContent == null) continue;

            var colData = new ColumnSaveData();

            // Save assigned farmer name
            Farmer assigned = FarmerAssignment.Instance?.GetAssigned(executor);
            colData.assignedFarmerName = assigned != null ? assigned.FarmerName : null;

            // Walk every top-level block inside the column
            foreach (Transform blockTransform in executor.blocksContent)
            {
                ExecutableBlock block = blockTransform.GetComponent<ExecutableBlock>();
                if (block != null) colData.blocks.Add(block.GetBlockSaveData());
            }

            result.Add(colData);
        }

        return result;
    }

    public void RestoreFromSaveData(List<ColumnSaveData> columns)
    {
        if (columns == null || columns.Count == 0) return;

        // Wipe the current workspace
        var toDestroy = new List<GameObject>();
        foreach (Transform child in workspaceContent)
            if (child.GetComponent<ColumnController>() != null)
                toDestroy.Add(child.gameObject);
        foreach (var go in toDestroy) Destroy(go);

        // Rebuild each saved column
        foreach (var colData in columns)
        {
            GameObject     colObj   = Instantiate(columnPrefab, workspaceContent);
            ColumnExecutor executor = colObj.GetComponent<ColumnExecutor>();
            if (executor == null || executor.blocksContent == null) continue;

            ColumnController ctrl = colObj.GetComponent<ColumnController>();

            // Restore blocks
            foreach (var blockData in colData.blocks)
                InstantiateBlock(blockData, executor.blocksContent);

            // Restore farmer assignment
            if (!string.IsNullOrEmpty(colData.assignedFarmerName))
            {
                foreach (Farmer farmer in FarmerSpawner.Instance.Farmers)
                {
                    if (farmer.FarmerName == colData.assignedFarmerName)
                    {
                        FarmerAssignment.Instance.Assign(executor, farmer);
                        break;
                    }
                }
            }
        }
    }

    public void InstantiateBlock(BlockSaveData data, Transform parent)
    {
        GameObject prefab = GetBlockPrefab(data.blockType);
        if (prefab == null)
        {
            Debug.LogWarning($"[WorkspaceManager] No prefab registered for block type '{data.blockType}'");
            return;
        }

        GameObject blockObj = Instantiate(prefab, parent);

        DraggableBlock draggable = blockObj.GetComponent<DraggableBlock>();
        if (draggable != null) draggable.isPaletteBlock = false;

        ExecutableBlock block = blockObj.GetComponent<ExecutableBlock>();
        block?.RestoreFromBlockSaveData(data);
    }

    public GameObject GetBlockPrefab(string typeName)
    {
        if (blockPrefabs == null) return null;
        foreach (var entry in blockPrefabs)
            if (entry.typeName == typeName) return entry.prefab;
        return null;
    }
}
