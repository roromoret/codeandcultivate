using TMPro;
using UnityEngine;

public class CodingUIController : MonoBehaviour
{
    public CanvasGroup codingUICanvasGroup; 
    private bool isVisible = false; 
    
    [Header("Toggle Button")]
    public TextMeshProUGUI buttonText;
    public string showText = "Show Code";
    public string hideText = "Hide Code";

    public GameObject previousButton;
    public GameObject nextButton;

    void Start()
    {
        SetUIVisibility(false);
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        SetUIVisibility(isVisible);
    }

    private void SetUIVisibility(bool show)
    {
        if (codingUICanvasGroup != null)
        {
            codingUICanvasGroup.alpha = show ? 1f : 0f;
            codingUICanvasGroup.interactable = show;
            codingUICanvasGroup.blocksRaycasts = show;
        }

        if (buttonText != null) { buttonText.text = show ? hideText : showText; }

        if (previousButton != null) previousButton.SetActive(show);
        if (nextButton != null) nextButton.SetActive(show);
    }
}