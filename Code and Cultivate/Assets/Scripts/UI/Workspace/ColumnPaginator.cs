using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColumnPaginator : MonoBehaviour
{
    [Header("Réglages")]
    public RectTransform contentRect;
    public int maxVisibleColumns = 2;
    public float columnWidth = 300f;
    public float spacing = 20f;
    public float smoothTime = 0.2f;

    [Header("Boutons")]
    public Button previousButton;
    public Button nextButton;

    private int currentIndex = 0;
    private Coroutine moveCoroutine;

    void Start()
    {
        RefreshPagination();
    }

    public void MoveNext()
    {
        int maxIndex = Mathf.Max(0, contentRect.childCount - maxVisibleColumns);
        
        if (currentIndex < maxIndex)
        {
            currentIndex++;
            StartSmoothMove();
        }
    }

    public void MovePrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            StartSmoothMove();
        }
    }

    public void RefreshPagination()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(RefreshRoutine());
        }
    }

    private IEnumerator RefreshRoutine()
    {
        yield return new WaitForEndOfFrame();
        
        int maxIndex = Mathf.Max(0, contentRect.childCount - maxVisibleColumns);
        if (currentIndex > maxIndex)
        {
            currentIndex = maxIndex;
            StartSmoothMove(); 
        }
        
        UpdateButtons();
    }

    private void StartSmoothMove()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        
        float targetX = -(currentIndex * (columnWidth + spacing));
        
        moveCoroutine = StartCoroutine(SmoothMove(targetX));
        UpdateButtons();
    }

    private IEnumerator SmoothMove(float targetX)
    {
        float elapsedTime = 0;
        float startX = contentRect.anchoredPosition.x;

        while (elapsedTime < smoothTime)
        {
            float newX = Mathf.Lerp(startX, targetX, elapsedTime / smoothTime);
            contentRect.anchoredPosition = new Vector2(newX, contentRect.anchoredPosition.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        contentRect.anchoredPosition = new Vector2(targetX, contentRect.anchoredPosition.y);
    }

    private void UpdateButtons()
    {
        int maxIndex = Mathf.Max(0, contentRect.childCount - maxVisibleColumns);
        
        if (previousButton != null) previousButton.interactable = (currentIndex > 0);
        if (nextButton != null) nextButton.interactable = (currentIndex < maxIndex);
    }
}