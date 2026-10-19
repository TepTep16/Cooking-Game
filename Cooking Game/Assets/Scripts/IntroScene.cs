using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

// add this to player input or game manager
public class IntroVideoPlayer : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Tooltip("The GameObject displaying the video")]
    public GameObject videoScreen;

    [Header("Skip")]
    public bool allowSkip = true;
    public Key skipKey = Key.Space;

    [Header("Gameplay Lockout")]
    [Tooltip("Scripts that'll be disabled while the video plays. Include the player scripts AND EnemyChef.")]
    public MonoBehaviour[] scriptsToDisableDuringVideo;

    [Header("After the video")]
    [Tooltip("Starts the tutorial dialogue straight after the video")]
    public TutorialManager tutorial;

    private bool hasEnded = false;

    void Start()
    {
        if (videoScreen != null) videoScreen.SetActive(true);
        SetScriptsEnabled(false);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
        else
        {
            // if no video is assigned the game autostarts
            EndIntro();
        }
    }

    void Update()
    {
        if (hasEnded || !allowSkip) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current[skipKey].wasPressedThisFrame)
        {
            EndIntro();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        EndIntro();
    }

    private void EndIntro()
    {
        if (hasEnded) return;
        hasEnded = true;

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.Stop();
        }

        if (videoScreen != null) videoScreen.SetActive(false);
        SetScriptsEnabled(true);

        // The tutorial immediately re-freezes the player and the enemy,
        // and only releases the enemy once the introduction is finished.
        if (tutorial != null) tutorial.Begin();
    }

    private void SetScriptsEnabled(bool enabledState)
    {
        foreach (var script in scriptsToDisableDuringVideo)
        {
            if (script != null) script.enabled = enabledState;
        }
    }
}
