using System.Collections;
using TMPro;
using UnityEngine;

public class WalkBlock : ExecutableBlock
{
    public enum WalkDirection { North, South, East, West }

    [Header("Direction")]
    public TMP_Dropdown directionDropdown;

    public override IEnumerator Execute()
    {
        Debug.Log($"[MoveBlock] Execute called. Farmer is null: {Farmer == null}");

        if (Farmer == null)
        {
            Debug.LogWarning("[WalkBlock] No farmer registered.");
            yield break;
        }

        highlightOutline.enabled = true;

        // Read direction from dropdown before yielding
        WalkDirection direction = (WalkDirection)directionDropdown.value;
        Debug.Log($"[WalkBlock] Walking {direction}");

        yield return WaitForFarmer();

        switch (direction)
        {
            case WalkDirection.North: Farmer.MoveNorth(); break;
            case WalkDirection.South: Farmer.MoveSouth(); break;
            case WalkDirection.East:  Farmer.MoveEast();  break;
            case WalkDirection.West:  Farmer.MoveWest();  break;
        }

        yield return WaitForFarmer();

        highlightOutline.enabled = false;
    }

    // Saves the direction in the code block for later restoration

    public override BlockSaveData GetBlockSaveData()
    {
        var data = new BlockSaveData { blockType = GetType().Name };
        data.blockParams.Add(directionDropdown != null ? directionDropdown.value.ToString() : "0");
        return data;
    }

    public override void RestoreFromBlockSaveData(BlockSaveData data)
    {
        if (directionDropdown != null && data.blockParams.Count > 0
            && int.TryParse(data.blockParams[0], out int val))
            directionDropdown.value = val;
    }











}