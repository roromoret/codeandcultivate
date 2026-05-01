using UnityEngine;
using System.Collections;

public class PlantBlock : ExecutableBlock
{
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
        Farmer.Plant();
        yield return WaitForFarmer();

        highlightOutline.enabled = false;
    }
}
