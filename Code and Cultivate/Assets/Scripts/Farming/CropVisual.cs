using System.Runtime.CompilerServices;
using UnityEngine;

public class CropVisual : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite youngSprite;
    [SerializeField] private Sprite matureSprite;
    [SerializeField] private Sprite defaultSprite; // this should not show

    private SpriteRenderer[] _renderers;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        if (_renderers.Length == 0) Debug.LogWarning($"[CropVisual] No SpriteRenderers found on {gameObject.name}");
    }

    public void UpdateVisual(GrowthStage stage)
    {
        if (_renderers == null || _renderers.Length == 0) return;

        Sprite target = (stage == GrowthStage.Mature ? matureSprite : youngSprite) ?? defaultSprite;
        foreach (SpriteRenderer sr in _renderers) if (sr != null) sr.sprite = target;
    }
}
