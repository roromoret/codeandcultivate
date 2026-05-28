using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSellRowUI : MonoBehaviour
{
    [SerializeField] private Image      iconImage;
    [SerializeField] private TMP_Text   nameText;
    [SerializeField] private TMP_Text   amountText;
    [SerializeField] private TMP_Text   priceText;
    [SerializeField] private Button     sell1Button;
    [SerializeField] private Button     sell10Button;
    [SerializeField] private Button     sell100Button;

    private ShopConfig.SellEntry _entry;

    public void Initialise(ShopConfig.SellEntry entry)
    {
        _entry = entry;

        string  displayName = ResourceManager.Instance.GetDisplayName(entry.resourceType);
        Sprite  icon        = ResourceManager.Instance.GetIcon(entry.resourceType);
        int     amount      = ResourceManager.Instance.Get(entry.resourceType);

        nameText.text   = displayName;
        amountText.text = $"x{amount}";
        priceText.text  = $"${entry.sellPricePerUnit} each";
        if (iconImage != null && icon != null) iconImage.sprite = icon;

        sell1Button.  onClick.AddListener(() => ShopManager.Instance.TrySell(_entry, 1));
        sell10Button. onClick.AddListener(() => ShopManager.Instance.TrySell(_entry, 10));
        sell100Button.onClick.AddListener(() => ShopManager.Instance.TrySell(_entry, 100));
    }
}
