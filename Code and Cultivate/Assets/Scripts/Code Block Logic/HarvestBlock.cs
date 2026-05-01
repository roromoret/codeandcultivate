using UnityEngine;
using System.Collections;

public class HarvestBlock : ExecutableBlock
{
    public override IEnumerator Execute()
    {   
        Debug.Log($"[HarvestBlock] Execute called. Farmer is null: {Farmer == null}");

        if (Farmer == null)
        {
            Debug.LogWarning("[HarvestBlock] No farmer registered.");
            yield break;
        }

        highlightOutline.enabled = true;

        yield return WaitForFarmer();
        Farmer.Harvest();
        yield return WaitForFarmer();

        highlightOutline.enabled = false;
    }
}
