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
                    yield return StartCoroutine(childBlock.Execute());
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
}