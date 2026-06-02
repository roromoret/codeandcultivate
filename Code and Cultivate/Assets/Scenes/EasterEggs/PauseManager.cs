using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 
    [SerializeField] private SpeedrunTimer speedrunTimer; // References speedrun timer script

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

        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                lastToggleTime = Time.unscaledTime;
                TogglePause();
            }
        }
    }

    private void TogglePause()
    {
        menuOpen = !menuOpen;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(menuOpen);

            if (menuOpen)
            {
                Time.timeScale = 0f;
                AudioListener.pause = true; //Pauses Game audio

                if (speedrunTimer != null)
                {
                    speedrunTimer.StopTimer();
                    speedrunTimer.HideActiveMessages();
                }
            }
            else
            {
                Time.timeScale = 1f;
                AudioListener.pause = false; //unpauses Game audio

                if (speedrunTimer != null)
                {
                    speedrunTimer.StartTimer();
                }
            }
        }

        // Cursor always visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()   
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        menuOpen = false;
        
        Time.timeScale = 1f;
        AudioListener.pause = false; //unpauses Game audio

        if (speedrunTimer != null)
        {
            speedrunTimer.StartTimer();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Quit() => Application.Quit();
}