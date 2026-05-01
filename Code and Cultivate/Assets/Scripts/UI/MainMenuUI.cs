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
        // Auto-find buttons if not assigned in Inspector
        if (newGameButton == null) newGameButton = FindButton("NewGameButton");
        if (loadGameButton == null) loadGameButton = FindButton("LoadGameButton");
        if (quitButton == null) quitButton = FindButton("QuitButton");
        if (saveInfoText == null) saveInfoText = FindObjectOfType<TextMeshProUGUI>();
        
        // Check for null references
        if (newGameButton == null || loadGameButton == null || quitButton == null || saveInfoText == null)
        {
            Debug.LogError("[MainMenuUI] UI fields not found! Please assign them in Inspector or name buttons correctly.");
            return;
        }
        
        // Subscribe to button clicks
        newGameButton.onClick.AddListener(OnNewGameClicked);
        loadGameButton.onClick.AddListener(OnLoadGameClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        
        // Check if save exists and update UI
        UpdateSaveStatus();
    }
    
    private Button FindButton(string buttonName)
    {
        GameObject btnObj = GameObject.Find(buttonName);
        return btnObj != null ? btnObj.GetComponent<Button>() : null;
    }
    
    private void UpdateSaveStatus()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[MainMenuUI] SaveManager not ready yet");
            saveInfoText.text = "Checking save...";
            saveInfoText.color = Color.yellow;
        return;
        }
   
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