using UnityEngine;
using System.Collections;

public class WhileBlock : ConditionalBlock
{
    public Transform innerBlocksContent;
    public int maxIterations = 50;

    public override IEnumerator Execute()
    {
        highlightOutline.enabled = true;
        yield return new WaitForSeconds(executionTime);

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

    public override void ResetBlock()
    {
        base.ResetBlock();
        for (int j = 0; j < innerBlocksContent.childCount; j++)
        {
            ExecutableBlock childBlock = innerBlocksContent.GetChild(j).GetComponent<ExecutableBlock>();
            if (childBlock != null) childBlock.ResetBlock();
        }
    }
}