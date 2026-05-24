using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyRowUI : MonoBehaviour
{
    [SerializeField] private Image      iconImage;
    [SerializeField] private TMP_Text   nameText;
    [SerializeField] private TMP_Text   priceText;
    [SerializeField] private Button     buy1Button;
    [SerializeField] private Button     buy10Button;
    [SerializeField] private Button     buy100Button;

    private ShopConfig.BuyEntry _entry;

    public void Initialise(ShopConfig.BuyEntry entry)
    {
        _entry = entry;

        if (iconImage != null && entry.icon != null) iconImage.sprite = entry.icon;
        nameText.text = entry.displayName;
        priceText.text = $"${entry.buyPrice} each";
        buy1Button.  onClick.AddListener(() => ShopManager.Instance.TryBuy(_entry, 1));
        buy10Button. onClick.AddListener(() => ShopManager.Instance.TryBuy(_entry, 10));
        buy100Button.onClick.AddListener(() => ShopManager.Instance.TryBuy(_entry, 100));
    }
}
