using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Base file for the while and if blocks 


//All the conditions between wich the player can choose
public enum ConditionType 
{ 
    AlwaysTrue, 
    AlwaysFalse, 
    FacingWall, 
    CropReady 
}

public abstract class ConditionalBlock : ExecutableBlock
{
    [Header("UI Elements")]
    public TMP_Dropdown conditionDropdown;
    public Toggle expectedStateToggle;
    
    //Call to all the functions to check conditions
    protected bool EvaluateCondition()
    {
        if (conditionDropdown == null) return false;

        ConditionType currentCondition = (ConditionType)conditionDropdown.value;
        
        bool rawConditionResult = false;

        switch (currentCondition)
        {
            case ConditionType.AlwaysTrue: 
                rawConditionResult = true; break;
            case ConditionType.AlwaysFalse: 
                rawConditionResult = false; break;
            case ConditionType.FacingWall:
                rawConditionResult = false; break;
            case ConditionType.CropReady:
                rawConditionResult = true; break;
        }

        if (expectedStateToggle == null) 
        {
            return rawConditionResult;
        }
        // Compare the result wich what the player want (output or !output)
        return rawConditionResult == expectedStateToggle.isOn;
    }
}