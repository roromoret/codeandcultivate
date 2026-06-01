using UnityEngine;
using System.Collections;

public class IfElseBlock : ConditionalBlock
{
    public Transform ifContent;
    public Transform elseContent;

    //Override the basic execution method ibn order to check for the condition
    public override IEnumerator Execute()
    {
        highlightOutline.enabled = true;
        yield return new WaitForSeconds(executionTime);

        bool isConditionMet = EvaluateCondition();
        Transform targetContent = isConditionMet ? ifContent : elseContent;

        for (int j = 0; j < targetContent.childCount; j++)
        {
            ExecutableBlock childBlock = targetContent.GetChild(j).GetComponent<ExecutableBlock>();
            if (childBlock != null)
            {
                yield return StartCoroutine(childBlock.Execute());
            }
        }

        highlightOutline.enabled = false;
    }

    public override void ResetBlock()
    {
        base.ResetBlock();
        ResetChildren(ifContent);
        ResetChildren(elseContent);
    }

    private void ResetChildren(Transform content)
    {
        for (int j = 0; j < content.childCount; j++)
        {
            ExecutableBlock childBlock = content.GetChild(j).GetComponent<ExecutableBlock>();
            if (childBlock != null) childBlock.ResetBlock();
        }
    }

    // IfElseBlock saves: condition dropdown, expected state toggle,

public override BlockSaveData GetBlockSaveData()
{
    var data = new BlockSaveData { blockType = GetType().Name };

    // Param 0: which condition is selected ("is tile farmable?", etc.).
    data.blockParams.Add(conditionDropdown != null ? conditionDropdown.value.ToString() : "0");

    // Param 1: expected-state toggle (e.g. "true" or "false" for the condition).
    data.blockParams.Add(expectedStateToggle != null ? expectedStateToggle.isOn.ToString() : "True");

    // Save everything inside the "if" branch.
    if (ifContent != null)
        foreach (Transform child in ifContent)
        {
            ExecutableBlock block = child.GetComponent<ExecutableBlock>();
            if (block != null) data.innerBlocks.Add(block.GetBlockSaveData());
        }

    // Save everything inside the "else" branch — into innerBlocksAlt this time.
    if (elseContent != null)
        foreach (Transform child in elseContent)
        {
            ExecutableBlock block = child.GetComponent<ExecutableBlock>();
            if (block != null) data.innerBlocksAlt.Add(block.GetBlockSaveData());
        }
    return data;
}

public override void RestoreFromBlockSaveData(BlockSaveData data)
{
    // Restore the condition dropdown selection.
    if (data.blockParams.Count > 0 && conditionDropdown != null
        && int.TryParse(data.blockParams[0], out int cond))
        conditionDropdown.value = cond;

    // Restore the toggle. Stored as the string "True" or "False" — compare directly.
    if (data.blockParams.Count > 1 && expectedStateToggle != null)
        expectedStateToggle.isOn = data.blockParams[1] == "True";

    // Rebuild the "if" branch blocks.
    if (ifContent != null)
        foreach (var blockData in data.innerBlocks)
            WorkspaceManager.Instance?.InstantiateBlock(blockData, ifContent);

    // Rebuild the "else" branch blocks.
    if (elseContent != null)
        foreach (var blockData in data.innerBlocksAlt)
            WorkspaceManager.Instance?.InstantiateBlock(blockData, elseContent);
}




}