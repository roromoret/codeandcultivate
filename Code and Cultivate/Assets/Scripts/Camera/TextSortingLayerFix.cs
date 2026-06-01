using UnityEngine;
using TMPro;

public class TextSortingLayerFix : MonoBehaviour
{
    public int superPriorityOrder = 10000; 

    void Start()
    {
        TextMeshPro textComponent = GetComponent<TextMeshPro>();
        
        if (textComponent != null)
        {
            textComponent.sortingOrder = superPriorityOrder;
        }
        else
        {
            Debug.LogWarning($"No TextMeshPro found on {gameObject.name}.");
        }
    }
}