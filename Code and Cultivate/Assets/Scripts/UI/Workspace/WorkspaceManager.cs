using System.Collections.Generic;
using UnityEngine;

public class WorkspaceManager : MonoBehaviour
{
    public GameObject columnPrefab;
    public Transform workspaceContent;
    public GameObject addColumnButton;
    public ColumnPaginator paginator;
    
    //Method to instantiate a colum prefab 
    public void AddNewColumn()
    {
        GameObject newColumn = Instantiate(columnPrefab, workspaceContent);
        
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

    // Singleton so other scripts (blocks, SaveManager) can easily reach this.
    public static WorkspaceManager Instance { get; private set; }

    // Helper struct used by the Inspector — pairs a class name with its prefab.
    // The Inspector shows this as an array you fill in by hand.
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
        // Standard singleton pattern. Destroys duplicates to keep one canonical instance.
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }


    public List<ColumnSaveData> GetSaveData()
{
    var result = new List<ColumnSaveData>();

    // Iterate every column GameObject inside the workspace.
    foreach (Transform child in workspaceContent)
    {
        // Each column has a ColumnExecutor whose blocksContent transform
        // is the actual parent of the dragged-in blocks.
        ColumnExecutor executor = child.GetComponent<ColumnExecutor>();
        if (executor == null || executor.blocksContent == null) continue;

        var colData = new ColumnSaveData();

        // Walk every top-level block inside the column and save it.
        // Each block recursively saves its own nested blocks via GetBlockSaveData().
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
    // Nothing to restore — bail out without modifying anything.
    if (columns == null || columns.Count == 0) return;

    // First, wipe the current workspace by destroying every existing column.
    // We collect them into a list first because modifying a transform's children
    // mid-iteration is unsafe.
    var toDestroy = new List<GameObject>();
    foreach (Transform child in workspaceContent)
        if (child.GetComponent<ColumnController>() != null)
            toDestroy.Add(child.gameObject);
    foreach (var go in toDestroy) Destroy(go);

    // Now instantiate each saved column from scratch.
    foreach (var colData in columns)
    {
        GameObject     colObj   = Instantiate(columnPrefab, workspaceContent);
        ColumnExecutor executor = colObj.GetComponent<ColumnExecutor>();
        if (executor == null || executor.blocksContent == null) continue;

        // Hook up the paginator so the new column participates in pagination.
        ColumnController ctrl = colObj.GetComponent<ColumnController>();


        // For each saved block in this column, instantiate it into the column.
        // InstantiateBlock handles recursion into nested blocks.
        foreach (var blockData in colData.blocks)
            InstantiateBlock(blockData, executor.blocksContent);
    }

}


    public void InstantiateBlock(BlockSaveData data, Transform parent)
{
    // Look up the prefab whose typeName matches the saved block's class name.
    GameObject prefab = GetBlockPrefab(data.blockType);
    if (prefab == null)
    {
        // Means someone forgot to register this block type in the Inspector array.
        Debug.LogWarning($"[WorkspaceManager] No prefab registered for block type '{data.blockType}'");
        return;
    }

    // Spawn the prefab as a child of the requested parent
    // (could be a column, or a Loop/If/While's inner content).
    GameObject blockObj = Instantiate(prefab, parent);

    // Mark this as a placed block, NOT a palette block.
    // Without this, it would behave like a draggable template from the toolbox
    // (i.e. cloning itself when dragged instead of moving).
    DraggableBlock draggable = blockObj.GetComponent<DraggableBlock>();
    if (draggable != null) draggable.isPaletteBlock = false;

    // Hand control to the block itself to reapply its params + spawn its children.
    // This is where recursion happens: LoopBlock.RestoreFromBlockSaveData calls
    // WorkspaceManager.Instance.InstantiateBlock for every block in innerBlocks.
    ExecutableBlock block = blockObj.GetComponent<ExecutableBlock>();
    block?.RestoreFromBlockSaveData(data);
}

// Simple lookup over the Inspector-filled registry.
public GameObject GetBlockPrefab(string typeName)
{
    if (blockPrefabs == null) return null;
    foreach (var entry in blockPrefabs)
        if (entry.typeName == typeName) return entry.prefab;
    return null;
}



}