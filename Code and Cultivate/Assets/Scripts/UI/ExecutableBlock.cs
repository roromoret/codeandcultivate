using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExecutableBlock : MonoBehaviour
{
    private Outline highlightOutline;
    
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
    public virtual void ResetBlock()
    {
        if (highlightOutline != null)
        {
            highlightOutline.enabled = false;
        }
    }
}