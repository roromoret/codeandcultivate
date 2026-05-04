using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
  
    /// Start a new game (fresh start)

    public void NewGame()
    {
        Debug.Log("[MenuManager] Starting new game");
        SceneManager.LoadScene("FarmScene");
    }
    

    /// Load existing game from save file
    public void LoadGame()
    {
        if (!SaveManager.Instance.SaveExists())
        {
            Debug.LogWarning("[MenuManager] No save file found!");
            return;
        }
        
        Debug.Log("[MenuManager] Loading saved game");
        
        // Load save data into memory BEFORE switching scenes
        // SaveManager will apply it when FarmScene finishes loading
        SaveManager.Instance.Load();
        
        SceneManager.LoadScene("FarmScene"); 
    }
    

    /// Return to main menu
    public void ReturnToMenu()
    {
        Debug.Log("[MenuManager] Returning to main menu");
        Time.timeScale = 1f; // Unpause if paused
        SceneManager.LoadScene("Main_Menu"); 
    }
    

    /// Quit the game
    public void QuitGame()
    {
        Debug.Log("[MenuManager] Quitting game");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}