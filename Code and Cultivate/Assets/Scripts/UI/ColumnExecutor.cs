using UnityEngine;
using System.Collections;

//Just call all the coroutine for each interface executableBlock of the column

public class ColumnExecutor : MonoBehaviour
{
    public Transform blocksContent; 

    public void StartExecution()
    {
        StopAllCoroutines(); 
        
        ResetAllBlocks();

        StartCoroutine(ExecuteBlocksSequence());
    }
    
    //Reset function in case the player run the column before it was finished
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