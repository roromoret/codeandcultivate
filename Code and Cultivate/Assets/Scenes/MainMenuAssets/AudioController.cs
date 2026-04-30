using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    public AudioMixer myMixer;

   
    public Slider musicSlider;
    public Slider sfxSlider;

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
        }
    }

    void Start()
    {
        
        if (musicSlider != null) musicSlider.value = 0.5f;
        if (sfxSlider != null) sfxSlider.value = 0.5f;

        SetMusicVolume(0.5f);
        SetSFXVolume(0.5f);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        myMixer.SetFloat("Music", volume);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        myMixer.SetFloat("SFX", volume);
    }
}