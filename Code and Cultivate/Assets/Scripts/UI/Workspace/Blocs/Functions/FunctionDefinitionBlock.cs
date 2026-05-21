using UnityEngine;
using TMPro;
using System.Collections;

public class FunctionDefinitionBlock : ExecutableBlock
{
    public TMP_InputField nameInputField;
    private string currentFunctionName;
    private ColumnExecutor columnExecutor;

    public static bool isTyping = false;

    void Start()
    {
        countAsInstruction = false;
        columnExecutor = GetComponentInParent<ColumnExecutor>();
        
        if (nameInputField != null)
        {
            nameInputField.onEndEdit.AddListener(OnNameChanged);
            
            nameInputField.onSelect.AddListener(delegate { isTyping = true; });
            nameInputField.onDeselect.AddListener(delegate { isTyping = false; });
        }
        
        CheckAndEnforceRules();
    }

    void Update()
    {
        if (transform.parent != null && transform.parent.name == "Content")
        {
            if (transform.GetSiblingIndex() != 0)
            {
                transform.SetSiblingIndex(0);
            }
        }
    }

    void OnTransformParentChanged()
    {
        CheckAndEnforceRules();
    }

    //Function created in order to keep only one definition at the top
    private void CheckAndEnforceRules()
    {
        if (transform.parent != null && transform.parent.name == "Content")
        {
            transform.SetSiblingIndex(0);
            
            FunctionDefinitionBlock[] definitions = transform.parent.GetComponentsInChildren<FunctionDefinitionBlock>();
            foreach (FunctionDefinitionBlock def in definitions)
            {
                if (def != this)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    //Change the name in the call in case we change the definition
    private void OnNameChanged(string newName)
    {
        if (!string.IsNullOrEmpty(currentFunctionName))
        {
            FunctionRegistry.UnregisterFunction(currentFunctionName);
        }

        columnExecutor = GetComponentInParent<ColumnExecutor>();

        if (FunctionRegistry.RegisterFunction(newName, columnExecutor))
        {
            currentFunctionName = newName;
        }
        else
        {
            nameInputField.text = currentFunctionName;
        }
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(currentFunctionName))
        {
            FunctionRegistry.UnregisterFunction(currentFunctionName);
        }
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
    }
}