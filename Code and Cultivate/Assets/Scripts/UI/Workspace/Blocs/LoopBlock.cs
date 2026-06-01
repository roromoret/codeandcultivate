using UnityEngine;
using System.Collections;
using TMPro;

public class LoopBlock : ExecutableBlock
{
    
    //Base loop of 3 :
    [Header("Loop Settings")]
    public int loopCount = 3; 
    public Transform innerBlocksContent;
    
    [Header("UI Reference")]
    public TMP_InputField loopInputField;

    public override IEnumerator Execute()
    {
        //Same outline as for generic blocks
        highlightOutline.enabled = true;
        yield return new WaitForSeconds(executionTime);

        if (loopInputField != null && !string.IsNullOrEmpty(loopInputField.text))
        {
            int.TryParse(loopInputField.text, out loopCount);
        }

        for (int i = 0; i < loopCount; i++)
        {
            for (int j = 0; j < innerBlocksContent.childCount; j++)
            {
                ExecutableBlock childBlock = innerBlocksContent.GetChild(j).GetComponent<ExecutableBlock>();
                
                if (childBlock != null)
                {
                    yield return StartCoroutine(childBlock.TriggerExecute());
                }
            }
        }

        highlightOutline.enabled = false;
    }
    
    //Visual reset in case of forced stop
    public override void ResetBlock()
    {
        base.ResetBlock();

        for (int j = 0; j < innerBlocksContent.childCount; j++)
        {
            ExecutableBlock childBlock = innerBlocksContent.GetChild(j).GetComponent<ExecutableBlock>();
            if (childBlock != null)
            {
                childBlock.ResetBlock();
            }
        }
    }

    //LoopBlock saves its loop count ANDDDD every block inside it.

    public override BlockSaveData GetBlockSaveData()
    {
        var data = new BlockSaveData { blockType = GetType().Name };
        data.blockParams.Add(loopCount.ToString());

        //recursively save inner blocks
        if (innerBlocksContent != null)
        foreach (Transform child in innerBlocksContent)
            {
                ExecutableBlock childBlock = child.GetComponent<ExecutableBlock>();
                if (childBlock != null)
                {
                    data.innerBlocks.Add(childBlock.GetBlockSaveData());
                }
            }
        return data;
    }


    public override void RestoreFromBlockSaveData(BlockSaveData data)
    {
        //Restires tge kiio ciybt (both the internal vield and the visible input field in the gui)
        if (data.blockParams.Count > 0  && int.TryParse(data.blockParams[0], out int count))
        {
            loopCount = count;
            if (loopInputField != null)
                loopInputField.text = count.ToString();
        }






        //Rebuilds every nested block inside the loop (if any)
        //Note: The WorkspaceManager handles recursion through InstantiateBlock.
        if (innerBlocksContent != null)
            foreach (var blockData in data.innerBlocks)
                WorkspaceManager.Instance?.InstantiateBlock(blockData, innerBlocksContent);
    }
}








