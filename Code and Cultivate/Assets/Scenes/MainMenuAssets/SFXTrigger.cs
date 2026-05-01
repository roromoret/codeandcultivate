using UnityEngine;
using UnityEngine.InputSystem;

public class SFXTrigger : MonoBehaviour
{
   
    public AudioSource[] sfxSources;
    public GameObject SoundPanel; 

    void Update()
    {
        //right button sound
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            PlayFromList(0);
        }

        if (Keyboard.current != null)
        {
            //wasd sounds
            bool wasdPressed = Keyboard.current.wKey.wasPressedThisFrame || 
                               Keyboard.current.aKey.wasPressedThisFrame || 
                               Keyboard.current.sKey.wasPressedThisFrame || 
                               Keyboard.current.dKey.wasPressedThisFrame;

            if (wasdPressed)
            {
                PlayFromList(1);
            }

            // arrow sounds
            bool arrowPressed = Keyboard.current.upArrowKey.wasPressedThisFrame || 
                                Keyboard.current.downArrowKey.wasPressedThisFrame || 
                                Keyboard.current.leftArrowKey.wasPressedThisFrame || 
                                Keyboard.current.rightArrowKey.wasPressedThisFrame;

            if (arrowPressed)
            {
                PlayFromList(2);
            }

           
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (SoundPanel != null)
                {
                    SoundPanel.SetActive(!SoundPanel.activeSelf);
                }
            }
        }
    }

 
    void PlayFromList(int index)
    {
        if (sfxSources.Length > index && sfxSources[index] != null)
        {
            sfxSources[index].Play();
        }
    }
}