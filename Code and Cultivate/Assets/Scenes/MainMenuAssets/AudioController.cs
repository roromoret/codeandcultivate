using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    public AudioMixer myMixer;

    private Slider musicSlider;
    private Slider sfxSlider;
    private Button muteButton;

    private bool isMuted = false;
    private float preMuteMusicVolume = 0.5f;
    private float preMuteSFXVolume = 0.5f;

    void Awake()
    {
        

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            preMuteMusicVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 0.5f);
            preMuteSFXVolume = PlayerPrefs.GetFloat("SavedSFXVolume", 0.5f);
            isMuted = PlayerPrefs.GetInt("SavedMuteState", 0) == 1;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindAndLinkUI();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(DelayedFindAndLinkUI());
    }

    private IEnumerator DelayedFindAndLinkUI()
    {
        yield return null;
        FindAndLinkUI();
    }

    void FindAndLinkUI()
    {
        Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
        foreach (Slider slider in allSliders)
        {
            if (slider.gameObject.name == "MusicSlider")
            {
                musicSlider = slider;
                musicSlider.onValueChanged.RemoveAllListeners(); 
                musicSlider.onValueChanged.AddListener(SetMusicVolume);
                musicSlider.value = preMuteMusicVolume;
                Debug.Log("🔊 Found and linked MusicSlider!");
            }
            else if (slider.gameObject.name == "SFXSlider")
            {
                sfxSlider = slider;
                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener(SetSFXVolume);
                sfxSlider.value = preMuteSFXVolume;
                Debug.Log("🔊 Found and linked SFXSlider!");
            }
        }

        Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            Transform foundMute = FindChildIncludingInactive(canvas.transform, "MuteButton");
            if (foundMute != null)
            {
                muteButton = foundMute.GetComponent<Button>();
                if (muteButton != null)
                {
                    muteButton.onClick.RemoveAllListeners();
                    muteButton.onClick.AddListener(ToggleMute);
                    Debug.Log("🔊 Successfully linked MuteButton deep inside Canvas child structure!");
                }
                break;
            }
        }
        
        ApplyMixerSettings();
    }

    private Transform FindChildIncludingInactive(Transform parent, string childName)
    {
        if (parent.name == childName) return parent;
        
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindChildIncludingInactive(parent.GetChild(i), childName);
            if (result != null) return result;
        }
        
        return null;
    }

    public void SetMusicVolume(float sliderValue)
    {
        preMuteMusicVolume = sliderValue;
        
        PlayerPrefs.SetFloat("SavedMusicVolume", preMuteMusicVolume);
        PlayerPrefs.Save();

        if (!isMuted)
        {
            float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
            myMixer.SetFloat("Music", volume);
        }
    }

    public void SetSFXVolume(float sliderValue)
    {
        preMuteSFXVolume = sliderValue;

        PlayerPrefs.SetFloat("SavedSFXVolume", preMuteSFXVolume);
        PlayerPrefs.Save();

        if (!isMuted)
        {
            float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
            myMixer.SetFloat("SFX", volume);
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        PlayerPrefs.SetInt("SavedMuteState", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMixerSettings();
    }

    private void ApplyMixerSettings()
    {
        if (isMuted)
        {
            myMixer.SetFloat("Music", -80f);
            myMixer.SetFloat("SFX", -80f);
        }
        else
        {
            float musicVol = (preMuteMusicVolume <= 0.0001f) ? -80f : Mathf.Log10(preMuteMusicVolume) * 20;
            myMixer.SetFloat("Music", musicVol);

            float sfxVol = (preMuteSFXVolume <= 0.0001f) ? -80f : Mathf.Log10(preMuteSFXVolume) * 20;
            myMixer.SetFloat("SFX", sfxVol);
        }
    }
}