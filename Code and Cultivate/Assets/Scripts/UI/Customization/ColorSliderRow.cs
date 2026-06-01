using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorSliderRow : MonoBehaviour
{
    public TextMeshProUGUI partNameText;
    public Image partPreviewImage; 
    
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    private SpriteRenderer _targetSprite;

    public void Setup(SpriteRenderer spriteRenderer)
    {
        _targetSprite = spriteRenderer;
        
        partNameText.text = spriteRenderer.gameObject.name; 

        Color currentColor = _targetSprite.color;
        
        redSlider.value = currentColor.r;
        greenSlider.value = currentColor.g;
        blueSlider.value = currentColor.b;
        
        if (partPreviewImage != null)
        {
            partPreviewImage.color = currentColor;
        }

        redSlider.onValueChanged.RemoveAllListeners();
        redSlider.onValueChanged.AddListener(OnSliderChanged);

        greenSlider.onValueChanged.RemoveAllListeners();
        greenSlider.onValueChanged.AddListener(OnSliderChanged);

        blueSlider.onValueChanged.RemoveAllListeners();
        blueSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        ApplyRGBColor();
    }

    private void Update()
    {
        if (_targetSprite != null && partPreviewImage != null)
        {
            partPreviewImage.sprite = _targetSprite.sprite;
        }
    }

    private void ApplyRGBColor()
    {
        if (_targetSprite != null)
        {
            Color finalColor = new Color(redSlider.value, greenSlider.value, blueSlider.value, 1f);
            
            _targetSprite.color = finalColor;
            
             if (partPreviewImage != null)
            {
                partPreviewImage.color = finalColor;
            }
        }
    }
}