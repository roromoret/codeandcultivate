using UnityEngine;
using UnityEngine.InputSystem;

public class SFXTrigger : MonoBehaviour
{
    public AudioSource sfxSource;
    public GameObject SoundPanel; 

    void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            sfxSource.Play();
        }

        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (SoundPanel != null)
            {
                bool isActive = SoundPanel.activeSelf;
                SoundPanel.SetActive(!isActive);
            }
        }
    }
}