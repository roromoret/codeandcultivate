using UnityEngine;
using UnityEngine.EventSystems;

public class GrowingApples : MonoBehaviour, IPointerClickHandler
{
    [Header("Growth Settings")]
    public float growthMultiplier = 1.2f; 
    public float maxScale = 5.0f;         
    
    [Header("Juice Settings")]
    public float returnSpeed = 5f;        
    
    private Vector3 targetScale;
    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * returnSpeed);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetScale.x < maxScale)
        {
            targetScale *= growthMultiplier;
            transform.Rotate(0, 0, Random.Range(-15f, 15f));
        }
    }
    
    public void ResetApple()
    {
        targetScale = baseScale;
    }
}