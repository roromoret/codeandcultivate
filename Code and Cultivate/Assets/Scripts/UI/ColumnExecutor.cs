using UnityEngine;
using System.Collections;

public class ColumnExecutor : MonoBehaviour
{
    public Transform blocksContent; 

    public void StartExecution()
    {
        StopAllCoroutines(); 
        
        ResetAllBlocks();

        StartCoroutine(ExecuteBlocksSequence());
    }

    private void ResetAllBlocks()
    {
        for (int i = 0; i < blocksContent.childCount; i++)
        {
            ExecutableBlock block = blocksContent.GetChild(i).GetComponent<ExecutableBlock>();
            if (block != null)
            {
                block.ResetBlock();
            }
        }
    }

    private IEnumerator ExecuteBlocksSequence()
    {
        for (int i = 0; i < blocksContent.childCount; i++)
        {
            Transform child = blocksContent.GetChild(i);
            ExecutableBlock block = child.GetComponent<ExecutableBlock>();
            
            if (block != null)
            {
                yield return StartCoroutine(block.Execute());
            }
        }
    }
}