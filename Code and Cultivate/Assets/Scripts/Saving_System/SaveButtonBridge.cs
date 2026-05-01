using UnityEngine;

public class SaveButtonBridge : MonoBehaviour
{
    // No need to assign anything in Inspector - finds managers dynamically
    
    public void OnSaveClicked()
    {
        Debug.Log("[SaveButtonBridge] Save clicked");
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Save();
        }
        else
        {
            Debug.LogError("[SaveButtonBridge] SaveManager.Instance is null!");
        }
    }
    
    public void OnMainMenuClicked()
    {
        Debug.Log("[SaveButtonBridge] Main menu clicked");
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ReturnToMenu();
        }
        else
        {
            Debug.LogError("[SaveButtonBridge] MenuManager.Instance is null!");
        }
    }
}