using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeBook : MonoBehaviour
{
    
    public GameObject bookPanel;

   
    public GameObject[] spreads;

    public Button previousButton;
    public Button nextButton;

    [Tooltip("e.g. 'Page 1 / 4'")]
    public Text pageCounterText;

    
    public Key toggleKey = Key.Q;
    
    public bool closeWithEscape = true;

  
    [Tooltip(" This is to pause The mouse look, PickUp, HoverHighlight, AdviceDisplay")]
    public Behaviour[] disableWhileOpen;
    
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
