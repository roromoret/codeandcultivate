using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioMixer myMixer;
  
    

  public void SetMusicVolume(float sliderValue)
{
    // If the slider is exactly at 0 itll kill the sound
    if (sliderValue == 0) 
    {
        myMixer.SetFloat("Music", -80f);
    }
    else
    {
  
        float volume = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        myMixer.SetFloat("Music", volume);
    }
}

    public void SetSFXVolume(float sliderValue)
    {
     
        if (sliderValue <= 0.0001f) 
        {
            myMixer.SetFloat("SFX", -80f);
        }
        else
        {
            float volume = Mathf.Log10(sliderValue) * 20;
            myMixer.SetFloat("SFX", volume);
        }
    }
    public Slider musicSlider; 
void Start()
{
  
    musicSlider.value = 0.5f; 
    
   
    SetMusicVolume(0.5f);
}
}