using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        FindAndLinkUI();
    }

    void FindAndLinkUI()
    {
        GameObject musicObj = GameObject.Find("MusicSlider");
        if (musicObj != null)
        {
            musicSlider = musicObj.GetComponent<Slider>();
            musicSlider.onValueChanged.RemoveAllListeners(); 
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            musicSlider.value = preMuteMusicVolume;
        }

        GameObject sfxObj = GameObject.Find("SFXSlider");
        if (sfxObj != null)
        {
            sfxSlider = sfxObj.GetComponent<Slider>();
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            sfxSlider.value = preMuteSFXVolume;
        }

        GameObject muteObj = GameObject.Find("MuteButton");
        if (muteObj != null)
        {
            muteButton = muteObj.GetComponent<Button>();
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(ToggleMute);
        }
        
        SetMusicVolume(preMuteMusicVolume);
        SetSFXVolume(preMuteSFXVolume);
    }

    public void SetMusicVolume(float sliderValue)
    {
        preMuteMusicVolume = sliderValue;
        if (!isMuted)
        {
            float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
            myMixer.SetFloat("Music", volume);
        }
    }

    public void SetSFXVolume(float sliderValue)
    {
        preMuteSFXVolume = sliderValue;
        if (!isMuted)
        {
            float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
            myMixer.SetFloat("SFX", volume);
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            myMixer.SetFloat("Music", -80f);
            myMixer.SetFloat("SFX", -80f);
        }
        else
        {
            SetMusicVolume(preMuteMusicVolume);
            SetSFXVolume(preMuteSFXVolume);
        }
    }
}