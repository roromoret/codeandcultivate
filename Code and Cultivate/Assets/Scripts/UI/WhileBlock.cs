using UnityEngine;
using System.Collections;

public class WhileBlock : ConditionalBlock
{
    public Transform innerBlocksContent;
    //Just a security in case of infinity loop :
    public int maxIterations = 50;

    public override IEnumerator Execute()
    {
        //Execution outline around the block
        highlightOutline.enabled = true;
        yield return new WaitForSeconds(executionTime); 
        //May switch with an execution time depending on the real execution of the block
        //when it will be linked to the gameplay

        
        int safetyCounter = 0;

        while (EvaluateCondition() && safetyCounter < maxIterations)
        {
            for (int j = 0; j < innerBlocksContent.childCount; j++)
            {
                ExecutableBlock childBlock = innerBlocksContent.GetChild(j).GetComponent<ExecutableBlock>();
                if (childBlock != null)
                {
                    yield return StartCoroutine(childBlock.Execute());
                }
            }

            safetyCounter++;
            yield return new WaitForSeconds(0.1f); 
        }

        if (safetyCounter >= maxIterations)
        {
            Debug.LogWarning("While loop forced stop (set max iterations in inspector)");
        }

        highlightOutline.enabled = false;
    }

    //In case of force stop
    public override void ResetBlock()
    {
        base.ResetBlock();
        for (int j = 0; j < innerBlocksContent.childCount; j++)
        {
            ExecutableBlock childBlock = innerBlocksContent.GetChild(j).GetComponent<ExecutableBlock>();
            if (childBlock != null) childBlock.ResetBlock();
        }
    }


// Same pattern as IfElseBlock, but with only one branch

public override BlockSaveData GetBlockSaveData()
{
    var data = new BlockSaveData { blockType = GetType().Name };

    // Param 0: condition selection.
    data.blockParams.Add(conditionDropdown != null ? conditionDropdown.value.ToString() : "0");

    // Param 1: expected-state toggle.
    data.blockParams.Add(expectedStateToggle != null ? expectedStateToggle.isOn.ToString() : "True");

    // Save every block inside the while loop's body.
    if (innerBlocksContent != null)
        foreach (Transform child in innerBlocksContent)
        {
            ExecutableBlock block = child.GetComponent<ExecutableBlock>();
            if (block != null) data.innerBlocks.Add(block.GetBlockSaveData());
        }
    return data;
}

public override void RestoreFromBlockSaveData(BlockSaveData data)
{
    // Restore dropdown.
    if (data.blockParams.Count > 0 && conditionDropdown != null
        && int.TryParse(data.blockParams[0], out int cond))
        conditionDropdown.value = cond;

    // Restore toggle.
    if (data.blockParams.Count > 1 && expectedStateToggle != null)
        expectedStateToggle.isOn = data.blockParams[1] == "True";

    // Rebuild the body blocks.
    if (innerBlocksContent != null)
        foreach (var blockData in data.innerBlocks)
            WorkspaceManager.Instance?.InstantiateBlock(blockData, innerBlocksContent);
}





}