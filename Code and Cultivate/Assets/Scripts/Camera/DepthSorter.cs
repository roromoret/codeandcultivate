using UnityEngine;
using UnityEngine.Rendering;

// In order to show the player right with all the crops
public class DepthSorter : MonoBehaviour
{
    private SortingGroup sortingGroup;
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] private float yOffset = 0f;

    void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        int order = Mathf.RoundToInt(-(transform.position.z + yOffset) * 100f);

        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = order;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}