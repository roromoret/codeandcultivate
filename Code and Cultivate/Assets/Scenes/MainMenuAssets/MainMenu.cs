using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuUI;   
    public GameObject settingsUI;   

    void Start()
    {
        // When the game starts, makes sure the settings is off.
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}