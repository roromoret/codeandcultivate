using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 

    private bool menuOpen = false;
    private float lastToggleTime = 0f;
    private const float TOGGLE_COOLDOWN = 0.35f;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Time.unscaledTime - lastToggleTime < TOGGLE_COOLDOWN) return;

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            lastToggleTime = Time.unscaledTime;
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        menuOpen = !menuOpen;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(menuOpen);
            Debug.Log("Pause menu " + (menuOpen ? "OPENED" : "CLOSED"));
        }
        else
        {
            Debug.LogError("❌ Pause Menu UI is NOT assigned in the Inspector!");
        }

        // Cursor always visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    
   
    public void Resume()   
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        menuOpen = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Quit() => Application.Quit();

  
    public void PauseGame()  //Pauses Game
    {
        Time.timeScale = 0f;
        AudioListener.pause = true; //Pauses Game audio
    }

    public void ResumeGame()  //Resumes Game
    {
        Time.timeScale = 1f;
        AudioListener.pause = false; //unpauses Game audio
    }
}