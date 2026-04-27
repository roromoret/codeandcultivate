using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TutorialPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text bodyLabel;
    [SerializeField] private Button   okButton;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 0.35f;

    private TutorialData    _data;
    private RectTransform   _rect;

    // Setup
    public void Show(TutorialData data)
    {
        _data   = data;
        _rect   = GetComponent<RectTransform>();

        titleLabel.text = data.title;
        bodyLabel.text  = data.body;

        okButton.onClick.AddListener(OnOKPressed);

        StartCoroutine(SlideIn());
    }

    private void OnOKPressed()
    {
        TutorialManager.Instance.MarkSeen(_data.id);
        okButton.interactable = false;  // to prevent double clicks happening
        StartCoroutine(SlideOut());
    }

    // Animation
    private IEnumerator SlideIn()
    {
        // Start offscreen to the right
        Vector2 offScreen = new Vector2(Screen.width, 0f);
        Vector2 onScreen  = GetOnScreenPosition();

        yield return Slide(offScreen, onScreen);
    }

    private IEnumerator SlideOut()
    {
        Vector2 offScreen = new Vector2(Screen.width, 0f);
        Vector2 onScreen  = GetOnScreenPosition();

        yield return Slide(onScreen, offScreen);

        Destroy(gameObject);
    }

    private IEnumerator Slide(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed/slideDuration);
            _rect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        _rect.anchoredPosition = to;
    }

    // Private helper for animation
    private Vector2 GetOnScreenPosition()
    {
        // Positions the panel so its right edge touches the right edge of the screen
        float canvasHalfWidth = Screen.width / 2f;
        float panelHalfWidth  = _rect.rect.width / 2f;
        return new Vector2(canvasHalfWidth - panelHalfWidth, 0f);
    }
}
