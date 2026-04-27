using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

        return rawConditionResult == expectedStateToggle.isOn;
    }
}