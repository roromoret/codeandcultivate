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

    // Start a fresh game in the given slot
    public void NewGame(int slot)
    {
        Debug.Log($"[MenuManager] Starting new game in slot {slot}");
        SaveManager.Instance.SetCurrentSlot(slot);
        SceneManager.LoadScene("FarmScene");
    }

    // Load an existing save from the given slot
    public void LoadGame(int slot)
    {
        if (!SaveManager.Instance.SlotExists(slot))
        {
            Debug.LogWarning($"[MenuManager] No save found in slot {slot}");
            return;
        }

        Debug.Log($"[MenuManager] Loading slot {slot}");
        SaveManager.Instance.Load(slot);
        SceneManager.LoadScene("FarmScene");
    }

    // Return to main menu
    public void ReturnToMenu()
    {
        Debug.Log("[MenuManager] Returning to main menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }

    // Quit the game
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