using UnityEngine;
using System.Collections;
using TMPro;

public class ResourceDeltaNotification : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideDistance = 200f;
    [SerializeField] private float fadeDuration = 0.15f;

    public void Play(int delta)
    {
        // Sign and color
        label.text  = delta >= 0 ? $"+{delta}" : $"{delta}";
        label.color = delta >= 0
        ? new Color(0.4f, 1f, 0.4f) // green text for gains
        : new Color(1f, 0.4f, 0.4f); // red text for losses

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        RectTransform rect    = GetComponent<RectTransform>();
        Vector2       startPos = rect.anchoredPosition;
        Vector2       endPos   = startPos + new Vector2(slideDistance, 0f);

        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;

            // Slide right
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            // Fade out during the latter portion
            float fadeStart = slideDuration - fadeDuration;
            if (elapsed >= fadeStart)
            {
                float fadeT = (elapsed - fadeStart) / fadeDuration;
                cg.alpha = Mathf.Lerp(1f, 0f, fadeT);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
