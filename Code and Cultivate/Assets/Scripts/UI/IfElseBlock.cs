using UnityEngine;
using System.Collections;

public class IfElseBlock : ConditionalBlock
{
    public Transform ifContent;
    public Transform elseContent;

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
}