using UnityEngine;
using UnityEngine.InputSystem;

// Put this on gamemanager
public class ToggleUIScreen : MonoBehaviour
{
    public GameObject uiScreen;

    [Tooltip("If true, Time.timeScale is set to 0 while the screen is open, pausing gameplay physics/animations, and restored to 1 when closed.")]
    public bool pauseGameWhileOpen = false;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleScreen();
        }
    }

    private void ToggleScreen()
    {
        if (uiScreen == null) return;

        bool isOpening = !uiScreen.activeSelf;
        uiScreen.SetActive(isOpening);

        if (pauseGameWhileOpen)
        {
            Time.timeScale = isOpening ? 0f : 1f;
        }
    }
}
