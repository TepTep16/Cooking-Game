
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingAudio : MonoBehaviour
{
    public AudioSource footstepSound;

    void Update()
    {
        if (footstepSound == null || Keyboard.current == null) return;

        bool wasdHeld = Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed ||
                        Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed;

        
        bool shouldPlay = wasdHeld && Time.timeScale > 0f;

        if (shouldPlay)
        {
           
            if (!footstepSound.isPlaying)
            {
                footstepSound.Play();
            }
        }
        else
        {
            if (footstepSound.isPlaying)
            {
                footstepSound.Stop();
            }
        }
    }
}

