using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Plays the opening tutorial: MC speech + instruction labels.
// Started by IntroVideoPlayer the moment the intro video ends.
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public static bool DialogueOpen { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private string mainCharacterName = "MC";

    [Header("Scripts frozen while a label is open")]
    [Tooltip("FirstPersonController + any pick-up / chop script on the player")]
    [SerializeField] private MonoBehaviour[] playerScripts;
    [Tooltip("EnemyChef. Stays frozen until the introduction finishes")]
    [SerializeField] private MonoBehaviour[] enemyScripts;

    private class Step
    {
        public string speaker;      // empty = instruction label (no name shown)
        public string text;
        public string waitBefore;   // event to wait for BEFORE showing this step
        public string waitAfter;    // event the player must cause AFTER dismissing it
        public bool releaseAfter;   // unfreeze the enemy once this step is dismissed

        public Step(string speaker, string text, string waitAfter = null, string waitBefore = null, bool releaseAfter = false)
        {
            this.speaker = speaker;
            this.text = text;
            this.waitAfter = waitAfter;
            this.waitBefore = waitBefore;
            this.releaseAfter = releaseAfter;
        }
    }

    private List<Step> steps;
    private string waitingKey;
    private bool eventFired;
    private bool started;

    void Awake()
    {
        Instance = this;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        BuildSteps();
    }

    void OnEnable()
    {
        Ingredient.PickedUp += OnPickedUp;
        Ingredient.Dropped += OnDropped;
        Ingredient.FullyChopped += OnFullyChopped;
    }

    void OnDisable()
    {
        Ingredient.PickedUp -= OnPickedUp;
        Ingredient.Dropped -= OnDropped;
        Ingredient.FullyChopped -= OnFullyChopped;
    }

    private void OnPickedUp(Ingredient i) { Notify("Picked"); }
    private void OnDropped(Ingredient i) { Notify("Dropped"); }
    private void OnFullyChopped(Ingredient i) { Notify("Chopped"); }

    // ---- EDIT THE STORY HERE ----
    private void BuildSteps()
    {
        string mc = mainCharacterName;
        steps = new List<Step>
        {
            new Step(mc, "Alright, I've got this. Let's see what ingredients I need."),
            new Step("", "Walk over to the cheese ingredient on the round-table.", waitAfter: "NearCheese"),
            new Step("", "To pick up an ingredient, use left click on the mouse (or press LT on the Xbox controller).", waitAfter: "Picked"),
            new Step("", "Place the cheese on cutting board near the window.", waitAfter: "NearCuttingBoard"),
            new Step("", "To drop an ingredient, simply use left click on the mouse again (or press LT on the Xbox controller).", waitAfter: "Dropped"),
            new Step("", "Once an ingredient is on a surface, you will be able to chop it. Right click 5 times close to an ingredient to chop it.", waitAfter: "Chopped"),
            new Step("", "Note, only certain ingredients can be chopped. Make sure not to waste your time trying to cut every ingredient you find."),
            new Step("", "Move towards the oven.", waitAfter: "NearOven"),
            new Step(mc, "I'll make 2 Pizza's in the blink of an eye!"),
            new Step("", "To cook a meal, you will need to place the ingredients on the oven."),
            new Step("", "Once all the ingredients are on the oven, it will start cooking automatically."),
            new Step("", "In the recipe book (Q), you are able to see the ingredients needed to create the meal displayed."),
            new Step("", "To find more ingredients, you will need to explore the area to find the pantry."),
            new Step("", "You are ready to start cooking. All the best!", releaseAfter: true),
            // Plays later, when your oven script calls TutorialManager.Instance.Notify("MealCooked")
            new Step(mc, "Pepperoni Pizza... This brings back memories.", waitBefore: "MealCooked"),
        };
    }

    // Called by IntroVideoPlayer when the intro ends
    public void Begin()
    {
        if (started) return;
        started = true;
        StartCoroutine(Run());
    }

    // Other scripts / trigger zones call this to say "something happened"
    public void Notify(string key)
    {
        if (key == waitingKey) eventFired = true;
    }

    public bool IsWaitingFor(string key) { return waitingKey == key; }

    private IEnumerator Run()
    {
        SetFrozen(enemyScripts, true);
        SetFrozen(playerScripts, true);

        for (int i = 0; i < steps.Count; i++)
        {
            Step s = steps[i];

            if (!string.IsNullOrEmpty(s.waitBefore))
                yield return WaitForEvent(s.waitBefore);

            SetFrozen(playerScripts, true);
            SetCursor(true);
            Show(s);

            yield return new WaitForSecondsRealtime(0.2f);   // stops a stray click skipping instantly
            yield return new WaitUntil(ClickPressed);
            Hide();

            bool chain = i + 1 < steps.Count
                         && string.IsNullOrEmpty(steps[i + 1].waitBefore)
                         && string.IsNullOrEmpty(s.waitAfter)
                         && !s.releaseAfter;

            if (!chain)
            {
                yield return null; // wait a frame so this click isn't also read as a pick-up click
                SetCursor(false);
                SetFrozen(playerScripts, false);
            }

            if (s.releaseAfter) SetFrozen(enemyScripts, false);

            if (!string.IsNullOrEmpty(s.waitAfter))
                yield return WaitForEvent(s.waitAfter);
        }
    }

    private IEnumerator WaitForEvent(string key)
    {
        waitingKey = key;
        eventFired = false;
        yield return new WaitUntil(() => eventFired);
        waitingKey = null;
    }

    private void Show(Step s)
    {
        DialogueOpen = true;
        dialoguePanel.SetActive(true);
        bodyText.text = string.IsNullOrEmpty(s.speaker) ? s.text : "<b>" + s.speaker + ":</b> " + s.text;
        continueText.text = "Left click anywhere to continue (or press LT)";
    }

    private void Hide()
    {
        DialogueOpen = false;
        dialoguePanel.SetActive(false);
    }

    private bool ClickPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        if (Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame) return true;
        return false;
    }

    private void SetFrozen(MonoBehaviour[] scripts, bool frozen)
    {
        if (scripts == null) return;
        foreach (var s in scripts)
            if (s != null) s.enabled = !frozen;
    }

    private void SetCursor(bool showCursor)
    {
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;
    }

    void Update()
    {
        // gently pulse the "click to continue" text
        if (DialogueOpen && continueText != null)
        {
            Color c = continueText.color;
            c.a = Mathf.Lerp(0.4f, 1f, (Mathf.Sin(Time.unscaledTime * 4f) + 1f) * 0.5f);
            continueText.color = c;
        }
    }
}
