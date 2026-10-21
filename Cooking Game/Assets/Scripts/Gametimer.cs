using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
   
    public float timeLimit = 180f;
    public bool startAutomatically = true;
    public Platform platform;
    public AnxietyManager anxietyManager;
    public TMP_Text timerText;
    public GameObject winScreen;
    public GameObject failScreen;
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

        
        if (IsPaused) return;

       
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
