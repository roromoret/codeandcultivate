using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuUI;   
    public GameObject settingsUI;   

    void Start()
    {
        settingsUI.SetActive(false);
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