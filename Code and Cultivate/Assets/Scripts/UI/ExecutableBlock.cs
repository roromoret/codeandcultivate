using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExecutableBlock : MonoBehaviour
{
    // giving the blocks access to the farmer
    public static IFarmerActions Farmer { get; private set; }
    public static void RegisterFarmer(IFarmerActions farmer)
    {   
        Debug.Log("[ExecutableBlock] RegisterFarmer called.");  // TEMP
        Farmer = farmer;
        Debug.Log("[ExecutableBlock] Farmer registered");
    }

    protected Outline highlightOutline;
    
    public Color highlightColor = Color.yellow;
    public float executionTime = 0.5f;
    [Range(1f, 10f)]
    public float outlineThickness = 4f;

    void Awake()
    {
        highlightOutline = GetComponent<Outline>();
        if (highlightOutline == null)
        {
            highlightOutline = gameObject.AddComponent<Outline>();
            highlightOutline.effectDistance = new Vector2(outlineThickness, -outlineThickness);
        }
        
        highlightOutline.effectColor = highlightColor;
        highlightOutline.enabled = false;
    }
    
    
    public virtual IEnumerator Execute()
    {
        highlightOutline.enabled = true;
        yield return new WaitForSeconds(executionTime);
        highlightOutline.enabled = false;
    }

    protected IEnumerator WaitForFarmer()
    {
        if (Farmer == null) yield break;
        yield return new WaitUntil(() => !Farmer.IsBusy);
    }
    
    //just reset in case of hard stop
    public virtual void ResetBlock()
    {
        if (highlightOutline != null)
        {
            highlightOutline.enabled = false;
        }
    }

    //Basic Serilization for saving/loading blocks.

    public virtual BlockSaveData GetBlockSaveData()
    => new BlockSaveData { blockType = GetType().Name };

    public virtual void RestoreFromBlockSaveData(BlockSaveData data) { }






}