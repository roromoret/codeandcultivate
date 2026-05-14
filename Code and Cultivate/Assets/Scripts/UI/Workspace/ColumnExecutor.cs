using UnityEngine;
using TMPro;
using System.Collections;

public class ColumnExecutor : MonoBehaviour
{
    public Transform blocksContent; 
    public TextMeshProUGUI blockCountText;
    public TextMeshProUGUI tickCountText;

    private int currentTicks = 0;

    public void StartExecution()
    {
        StopAllCoroutines(); 
        
        CalculateTotalBlocks();
        currentTicks = 0;
        UpdateTickUI();

        ResetAllBlocks();

        StartCoroutine(ExecuteBlocksSequence());
    }

    private void CalculateTotalBlocks()
    {
        if (blocksContent == null) return;

        ExecutableBlock[] allBlocks = blocksContent.GetComponentsInChildren<ExecutableBlock>(false);
        int validBlocksCount = 0;

        foreach (ExecutableBlock block in allBlocks)
        {
            if (block.countAsInstruction)
            {
                validBlocksCount++;
            }
        }
        
        if (blockCountText != null)
        {
            blockCountText.text = "Executing " + validBlocksCount.ToString() + " blocks";
        }
    }

    public void AddTick()
    {
        currentTicks++;
        UpdateTickUI();
    }

    private void UpdateTickUI()
    {
        if (tickCountText != null)
        {
            tickCountText.text = "In " + currentTicks.ToString() + " ticks";
        }
    }
    
    
    //Get called if the player Run the scipt while it's not finished, so the run start again from the begining
    private void ResetAllBlocks()
    {
        ExecutableBlock[] allBlocks = blocksContent.GetComponentsInChildren<ExecutableBlock>(true);
        foreach (ExecutableBlock block in allBlocks)
        {
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
            
            if (block != null && block.gameObject.activeSelf)
            {
                yield return StartCoroutine(block.TriggerExecute());
            }
        }
    }
}