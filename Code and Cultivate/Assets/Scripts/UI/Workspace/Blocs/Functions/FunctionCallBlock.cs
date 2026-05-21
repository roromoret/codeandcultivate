using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FunctionCallBlock : ExecutableBlock
{
    public TMP_Dropdown functionDropdown;

    void OnEnable()
    {
        FunctionRegistry.OnRegistryChanged += RefreshDropdown;
        RefreshDropdown();
    }

    void OnDisable()
    {
        FunctionRegistry.OnRegistryChanged -= RefreshDropdown;
    }

    private void RefreshDropdown()
    {
        if (functionDropdown == null) return;

        string currentSelection = "";
        if (functionDropdown.options.Count > 0)
        {
            currentSelection = functionDropdown.options[functionDropdown.value].text;
        }

        functionDropdown.ClearOptions();
        List<string> availableFunctions = FunctionRegistry.GetFunctionNames();
        
        if (availableFunctions.Count == 0)
        {
            functionDropdown.AddOptions(new List<string> { "No Function" });
            return;
        }

        functionDropdown.AddOptions(availableFunctions);

        int index = availableFunctions.IndexOf(currentSelection);
        if (index >= 0)
        {
            functionDropdown.value = index;
        }
        else
        {
            functionDropdown.value = 0;
        }
    }

    public override IEnumerator Execute()
    {
        if (highlightOutline != null) highlightOutline.enabled = true;

        if (functionDropdown != null && functionDropdown.options.Count > 0)
        {
            string targetName = functionDropdown.options[functionDropdown.value].text;
            ColumnExecutor targetExecutor = FunctionRegistry.GetExecutor(targetName);

            if (targetExecutor != null)
            {
                if (FunctionRegistry.currentCallDepth >= FunctionRegistry.MAX_CALL_DEPTH)
                {
                    if (highlightOutline != null) highlightOutline.effectColor = Color.red;
                    yield return new WaitForSeconds(executionTime);
                }
                else
                {
                    FunctionRegistry.currentCallDepth++;
                    yield return StartCoroutine(targetExecutor.ExecuteSequenceOnly());
                    FunctionRegistry.currentCallDepth--;
                }
            }
            else
            {
                if (highlightOutline != null) highlightOutline.effectColor = Color.red;
                yield return new WaitForSeconds(executionTime);
            }
        }

        if (highlightOutline != null) 
        {
            highlightOutline.enabled = false;
            highlightOutline.effectColor = highlightColor; 
        }
    }
}