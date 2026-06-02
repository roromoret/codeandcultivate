using UnityEngine;
using UnityEngine.InputSystem;

public class EasterEggManager : MonoBehaviour
{
    [SerializeField] private GameObject easterEggMenuUI;

    private bool easterEggOpen = false;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            easterEggOpen = !easterEggOpen;
            
            if (easterEggMenuUI != null)
            {
                easterEggMenuUI.SetActive(easterEggOpen);
            }
        }
    }
}