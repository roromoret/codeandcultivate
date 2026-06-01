using UnityEngine;
using UnityEngine.SceneManagement; 




        // This is for toggeling main menu - settings and load select. Super basic functions.


public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuUI;   
    public GameObject settingsUI;
    public GameObject LoadSelectUI;

    void Start()
    {
        settingsUI.SetActive(false);
        LoadSelectUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void OpenLoadSelect()
    {
        mainMenuUI.SetActive(false);
        LoadSelectUI.SetActive(true);
    }

    public void CloseLoadSelect()
    {
        LoadSelectUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void GoToFarm()
    {
        SceneManager.LoadScene("FarmScene"); 
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
   public void QuitGame()
    {
        Application.Quit();
    }
}