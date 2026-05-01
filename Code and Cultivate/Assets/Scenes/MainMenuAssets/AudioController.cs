using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    public AudioMixer myMixer;

    public Slider musicSlider;
    public Slider sfxSlider;

    
    private bool isMuted = false;
    private float preMuteMusicVolume;
    private float preMuteSFXVolume;

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
        // Set default start values
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;
        
        SetMusicVolume(0.5f);
        SetSFXVolume(0.5f);
    }

    public void SetMusicVolume(float sliderValue)
    {
       
        if (!isMuted)
        {
            float volume = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
            myMixer.SetFloat("Music", volume);
        }
    }

    public void SetSFXVolume(float sliderValue)
    {
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
           
            preMuteMusicVolume = musicSlider.value;
            preMuteSFXVolume = sfxSlider.value;

            
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