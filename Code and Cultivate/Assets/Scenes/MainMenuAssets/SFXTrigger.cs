using UnityEngine;
using UnityEngine.InputSystem;//Mock Code just to check out the sound.

public class SFXTrigger : MonoBehaviour
{
    public AudioSource sfxSource;

    void Update()
    {
        
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            sfxSource.Play();
            Debug.Log("Right click detected with New Input System!");
        }
    }
}