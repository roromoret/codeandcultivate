using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI saveInfoText;
    
    private void Start()
    {
        // Subscribe to button clicks
        newGameButton.onClick.AddListener(OnNewGameClicked);
        loadGameButton.onClick.AddListener(OnLoadGameClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        
        // Check if save exists and update UI
        UpdateSaveStatus();
    }
    
    private void UpdateSaveStatus()
    {
        if (SaveManager.Instance.SaveExists())
        {
            loadGameButton.interactable = true;
            saveInfoText.text = $"Save found: {SaveManager.Instance.GetSaveInfo()}";
            saveInfoText.color = Color.green;
        }
        else
        {
            loadGameButton.interactable = false;
            saveInfoText.text = "No save file found";
            saveInfoText.color = Color.red;
        }
    }
    
    private void OnNewGameClicked()
    {
        Debug.Log("[MainMenuUI] New Game button pressed");
        MenuManager.Instance.NewGame();
    }
    
    private void OnLoadGameClicked()
    {
        Debug.Log("[MainMenuUI] Load Game button pressed");
        MenuManager.Instance.LoadGame();
    }
    
    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Quit button pressed");
        MenuManager.Instance.QuitGame();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (newGameButton != null) newGameButton.onClick.RemoveListener(OnNewGameClicked);
        if (loadGameButton != null) loadGameButton.onClick.RemoveListener(OnLoadGameClicked);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}