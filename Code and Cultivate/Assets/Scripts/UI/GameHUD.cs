using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnSaveClicked()
    {
        SaveManager.Instance.Save(SaveManager.Instance.CurrentSlot);
        Debug.Log("[GameHUD] Game saved");
    }

    private void OnMainMenuClicked()
    {
        MenuManager.Instance.ReturnToMenu();
    }

    private void OnDestroy()
    {
        saveButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
    }
}
