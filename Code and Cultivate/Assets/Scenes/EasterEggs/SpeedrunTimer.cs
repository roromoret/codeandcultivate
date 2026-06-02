using UnityEngine;
using TMPro; 
using UnityEngine.InputSystem;

public class SpeedrunTimer : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text tauntText;      
    public TMP_Text cheatTauntText; 

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool eggTriggered = false; 

    private int tabPressCount = 0;
    private const int MAX_TAB_PRESSES = 5; 

    void Start()
    {
        if (tauntText != null) tauntText.gameObject.SetActive(false);
        if (cheatTauntText != null) cheatTauntText.gameObject.SetActive(false);
        
        StartTimer(); 
    }

    void Update()
    {
        // Tab key pause system for timer specifically
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            tabPressCount++; 

            // Trigger Cheat Taunt
            if (tabPressCount >= MAX_TAB_PRESSES && cheatTauntText != null && !cheatTauntText.gameObject.activeSelf)
            {
                TriggerCheatTaunt();
            }

            if (isRunning) StopTimer();
            else StartTimer();
        }

        if (!isRunning) return;

        elapsedTime += Time.unscaledDeltaTime; 
        UpdateTimerDisplay();

        // easter egg once you hit the 1st minute mark
        if (elapsedTime >= 60f && !eggTriggered)
        {
            TriggerOneMinuteTaunt();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", hours, minutes, seconds, milliseconds);
    }

    //Taunting player message
    void TriggerOneMinuteTaunt()
    {
        eggTriggered = true;
        if (tauntText != null)
        {
            tauntText.gameObject.SetActive(true);
            Invoke("HideOneMinuteTaunt", 4f); // Disappears after 4 seconds
        }
    }

    void HideOneMinuteTaunt()
    {
        if (tauntText != null) tauntText.gameObject.SetActive(false);
    }

    void TriggerCheatTaunt()
    {
        cheatTauntText.gameObject.SetActive(true);
        Invoke("HideCheatTaunt", 4f); // Disappears after 4 seconds
    }

    void HideCheatTaunt()
    {
        if (cheatTauntText != null) cheatTauntText.gameObject.SetActive(false);
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public void ResetTimer() => elapsedTime = 0f;

    // Helper method to clear out UI alerts during manual pauses
    public void HideActiveMessages()
    {
        CancelInvoke("HideOneMinuteTaunt");
        CancelInvoke("HideCheatTaunt");
        if (tauntText != null) tauntText.gameObject.SetActive(false);
        if (cheatTauntText != null) cheatTauntText.gameObject.SetActive(false);
    }

}