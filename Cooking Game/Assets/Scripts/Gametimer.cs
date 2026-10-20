using UnityEngine;
using TMPro;

// Put this on an empty GameObject (e.g. your GameManager).
// Counts down; if the platform finishes in time the player wins, otherwise the fail screen shows.
public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("How long (seconds) the player has to get every dish onto the platform.")]
    public float timeLimit = 180f;

    [Tooltip("If true, the timer starts as soon as the scene loads. Turn off and call StartTimer() yourself if you want a countdown-in or menu first.")]
    public bool startAutomatically = true;

    [Header("References")]
    [Tooltip("The Platform the player serves food onto. The timer checks this to see if the round is finished.")]
    public Platform platform;

    [Tooltip("Optional. If the AnxietyManager ends the round first, the timer will stop checking rather than also firing the fail screen afterward.")]
    public AnxietyManager anxietyManager;

    [Header("UI")]
    [Tooltip("Optional. Text element showing the remaining time.")]
    public TMP_Text timerText;

    [Tooltip("Shown when all food is placed before time runs out.")]
    public GameObject winScreen;

    [Tooltip("Shown when time runs out before all food is placed.")]
    public GameObject failScreen;

    [Header("Pause")]
    [Tooltip("While ANY of these menus/screens is open (active in the scene), the countdown pauses. Drag in your pause menu, recipe book, controls screen, etc. They should start inactive.")]
    public GameObject[] menusThatPauseTimer;

    private bool manuallyPaused = false;
    private float timeRemaining;
    private bool isRunning = false;
    private bool isFinished = false;

    public float TimeRemaining => timeRemaining;
    public bool IsFinished => isFinished;

    void Start()
    {
        timeRemaining = timeLimit;
        UpdateTimerText();

        if (startAutomatically)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        timeRemaining = timeLimit;
        isRunning = true;
        isFinished = false;
    }

    void Update()
    {
        if (!isRunning || isFinished) return;
        if (anxietyManager != null && anxietyManager.HasLost) return;

        // Hold the clock still while a menu is open (or something paused it by script).
        if (IsPaused) return;

        // Player got everything plated before the clock ran out.
        if (platform != null && platform.IsComplete)
        {
            EndGame(true);
            return;
        }

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        UpdateTimerText();

        if (timeRemaining <= 0f)
        {
            EndGame(false);
        }
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;

        int minutes = (int)(timeRemaining / 60f);
        int seconds = (int)(timeRemaining % 60f);
        timerText.text = minutes.ToString("0") + ":" + seconds.ToString("00");
    }

    private void EndGame(bool won)
    {
        isFinished = true;
        isRunning = false;

        if (won && winScreen != null)
        {
            winScreen.SetActive(true);
        }
        else if (!won && failScreen != null)
        {
            failScreen.SetActive(true);
        }
    }

    // True while any listed menu is open, or after PauseTimer() has been called.
    public bool IsPaused => manuallyPaused || IsAnyMenuOpen();

    private bool IsAnyMenuOpen()
    {
        foreach (GameObject menu in menusThatPauseTimer)
        {
            if (menu != null && menu.activeInHierarchy) return true;
        }

        return false;
    }

    // Optional: other scripts can pause/resume the clock directly (e.g. during a cutscene).
    public void PauseTimer()
    {
        manuallyPaused = true;
    }

    public void ResumeTimer()
    {
        manuallyPaused = false;
    }

    // Call this to stop the clock without showing either screen (e.g. the anxiety meter maxed out first).
    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timeRemaining = timeLimit;
        isRunning = false;
        isFinished = false;

        if (winScreen != null) winScreen.SetActive(false);
        if (failScreen != null) failScreen.SetActive(false);
    }
}
