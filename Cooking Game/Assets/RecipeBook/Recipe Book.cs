using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


/// IMPORTANT: put this script on an object that stays active (e.g. the Canvas or an empty
/// manager object) - NOT on the book panel itself, or the Q key stops working once it's hidden.

public class RecipeBook : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The root book object. It's hidden at start and shown when the book opens.")]
    public GameObject bookPanel;

    [Tooltip("One entry per spread, in order. Each is a UI object holding the content for both the left and right page.")]
    public GameObject[] spreads;

    public Button previousButton;
    public Button nextButton;

    [Tooltip("Optional - shows e.g. 'Page 1 / 4'")]
    public Text pageCounterText;

    [Header("Input")]
    public Key toggleKey = Key.Q;
    [Tooltip("Also close the book with Escape")]
    public bool closeWithEscape = true;

    [Header("While Open")]
    [Tooltip("Scripts to switch off while the book is open, e.g. your mouse look, PickUp, HoverHighlight, AdviceDisplay")]
    public Behaviour[] disableWhileOpen;
    [Tooltip("Freeze the game (Time.timeScale = 0) while the book is open")]
    public bool pauseGameWhileOpen = false;

    private int currentSpread = 0;
    private bool isOpen = false;

    private CursorLockMode previousLockState;
    private bool previousCursorVisible;
    private float previousTimeScale = 1f;

    void Start()
    {
        if (previousButton != null) previousButton.onClick.AddListener(PreviousSpread);
        if (nextButton != null) nextButton.onClick.AddListener(NextSpread);

        SetOpen(false);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            SetOpen(!isOpen);
        }
        else if (isOpen && closeWithEscape && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetOpen(false);
        }
    }

    public void NextSpread()
    {
        ShowSpread(currentSpread + 1);
    }

    public void PreviousSpread()
    {
        ShowSpread(currentSpread - 1);
    }

    private void SetOpen(bool open)
    {
        // Remember the cursor/time state only when opening, so we can restore it on close
        if (open && !isOpen)
        {
            previousLockState = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            previousTimeScale = Time.timeScale;
        }

        isOpen = open;

        if (bookPanel != null)
            bookPanel.SetActive(open);

        if (open)
        {
            // The buttons need a free mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowSpread(currentSpread);

            if (pauseGameWhileOpen)
                Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = previousLockState;
            Cursor.visible = previousCursorVisible;

            if (pauseGameWhileOpen)
                Time.timeScale = previousTimeScale;
        }

        foreach (Behaviour script in disableWhileOpen)
        {
            if (script != null)
                script.enabled = !open;
        }
    }

    private void ShowSpread(int index)
    {
        if (spreads == null || spreads.Length == 0) return;

        currentSpread = Mathf.Clamp(index, 0, spreads.Length - 1);

        for (int i = 0; i < spreads.Length; i++)
        {
            if (spreads[i] != null)
                spreads[i].SetActive(i == currentSpread);
        }

        if (previousButton != null)
            previousButton.interactable = currentSpread > 0;

        if (nextButton != null)
            nextButton.interactable = currentSpread < spreads.Length - 1;

        if (pageCounterText != null)
            pageCounterText.text = "Page " + (currentSpread + 1) + " / " + spreads.Length;
    }
}
