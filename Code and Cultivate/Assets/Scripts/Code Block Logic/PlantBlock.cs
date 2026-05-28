using UnityEngine;
using TMPro; // added for the dropdown
using System.Collections;

public class PlantBlock : ExecutableBlock
{
    [Header("Crop Selection")]
    public TMP_Dropdown cropDropdown;

    public override IEnumerator Execute()
    {   
        Debug.Log($"[PlantBlock] Execute called. Farmer is null: {Farmer == null}");

        if (Farmer == null)
        {
            Debug.LogWarning("[PlantBlock] No farmer registered.");
            yield break;
        }

        highlightOutline.enabled = true;

        yield return WaitForFarmer();
        
        // get the selected index (0 = Carrots, 1 = Apple, 2 = Berry based on prefab)
        int selectedCrop = cropDropdown != null ? cropDropdown.value : 0;
        
        Farmer.Plant(selectedCrop);
        
        yield return WaitForFarmer();

        highlightOutline.enabled = false;
    }
}