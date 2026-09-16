using UnityEngine;
public class AnxietyManager : MonoBehaviour
{
    [Header("Meter")]
    [Tooltip("0 = calm, 1 = fatal anxiety attack.")]
    [Range(0f, 1f)] public float anxietyLevel = 0f;

    [Tooltip("How much the meter rises with each food item that's dropped.")]
    public float increasePerDrop = 0.15f;

    [Tooltip("toggle for recovery over time.")]
    public bool recoverOverTime = true;

    [Tooltip("How much the bar recovers by per second.")]
    public float recoveryPerSecond = 0.02f;

    [Header("Screen Overlay")]
    [Tooltip("The canvas overlay that will fade in as the anxiety bar rises.")]
    public CanvasGroup overlayCanvasGroup;

    [Tooltip("The overlay's maximum opaque level (max is 1).")]
    [Range(0f, 1f)] public float maxOverlayAlpha = 0.8f;

    [Header("Background Audio")]
    public AudioSource backgroundAudioSource;

    [Range(0f, 1f)] public float minVolume = 0.1f;
    [Range(0f, 1f)] public float maxVolume = 1f;

    [Header("Lose Condition")]
    [Tooltip("The UI screen that'll appear when the bar reaches its maximum.")]
    public GameObject loseScreen;

    private bool hasLost = false;

    void Update()
    {
        if (hasLost) return;

        if (recoverOverTime && anxietyLevel > 0f)
        {
            anxietyLevel -= recoveryPerSecond * Time.deltaTime;
            if (anxietyLevel < 0f) anxietyLevel = 0f;
        }

        ApplyEffects();

        if (anxietyLevel >= 0.7f)
        {
            TriggerLose();
        }
    }

    // This part should get called anytime the food hits the ground
    public void OnFoodDropped()
    {
        if (hasLost) return;

        anxietyLevel += increasePerDrop;
        if (anxietyLevel > 1f) anxietyLevel = 1f;
    }

    private void ApplyEffects()
    {
        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.alpha = anxietyLevel * maxOverlayAlpha;
        }

        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.volume = minVolume + (maxVolume - minVolume) * anxietyLevel;
        }
    }

    private void TriggerLose()
    {
        hasLost = true;

        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
        }
    }

    // This part should be called on any round reset
    public void ResetAnxiety()
    {
        anxietyLevel = 0f;
        hasLost = false;

        if (loseScreen != null)
        {
            loseScreen.SetActive(false);
        }
    }
}
