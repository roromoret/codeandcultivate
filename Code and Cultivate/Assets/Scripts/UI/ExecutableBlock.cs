using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExecutableBlock : MonoBehaviour
{
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

        //We will need to add the actions here to link between the game and the code
        yield return new WaitForSeconds(executionTime);
        
        highlightOutline.enabled = false;
    }
    
    //just reset in case of hard stop
    public virtual void ResetBlock()
    {
        if (highlightOutline != null)
        {
            highlightOutline.enabled = false;
        }
    }
}