using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopOpenButton : MonoBehaviour
{
    [SerializeField] private ShopUI shopUI;
    
    private void Awake() 
    => GetComponent<Button>().onClick.AddListener(shopUI.OpenShop);
}
