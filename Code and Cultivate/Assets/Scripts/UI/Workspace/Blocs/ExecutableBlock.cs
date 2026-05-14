using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExecutableBlock : MonoBehaviour
{
    public static IFarmerActions Farmer { get; private set; }
    public static void RegisterFarmer(IFarmerActions farmer)
    {   
        Farmer = farmer;
    }

    public bool countAsInstruction = true;

    protected Outline highlightOutline;
    
    public Color highlightColor = Color.yellow;
    public float executionTime = 0.5f;
    [Range(1f, 10f)]
    public float outlineThickness = 4f;

    private ColumnExecutor cachedExecutor;

    //Setup add the addtition of a block
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

        cachedExecutor = GetComponentInParent<ColumnExecutor>();
    }
    
    // Same Trigger for every block, get called from loops also (usefull for the ticks)
    public IEnumerator TriggerExecute()
    {
        if (cachedExecutor == null) 
        { 
            cachedExecutor = GetComponentInParent<ColumnExecutor>(); 
        }

        if (cachedExecutor != null && countAsInstruction)
        {
            cachedExecutor.AddTick();
        }

        yield return StartCoroutine(Execute());
    }

    //Where we do the real code execution
    public virtual IEnumerator Execute()
    {
        if (highlightOutline != null) highlightOutline.enabled = true;
        yield return new WaitForSeconds(executionTime); //Temporary while there are no real actions
        if (highlightOutline != null) highlightOutline.enabled = false;
    }

    protected IEnumerator WaitForFarmer()
    {
        if (Farmer == null) yield break;
        yield return new WaitUntil(() => !Farmer.IsBusy);
    }
    
    public virtual void ResetBlock()
    {
        if (highlightOutline != null)
        {
            highlightOutline.enabled = false;
        }
    }
}