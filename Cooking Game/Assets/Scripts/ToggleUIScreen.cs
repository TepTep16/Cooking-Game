using UnityEngine;
using UnityEngine.InputSystem;

// Put this on gamemanager
public class ToggleUIScreen : MonoBehaviour
{
    public GameObject uiScreen1;
    public GameObject uiScreen2;
    //enable to pause gamplay while screen is open
    public bool pauseGameWhileOpen = false;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleScreen1();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleScreen2();
        }
    }

    private void ToggleScreen1()
    {
        if (uiScreen1 == null) return;

        bool isOpening = !uiScreen1.activeSelf;
        uiScreen1.SetActive(isOpening);

        if (pauseGameWhileOpen)
        {
            Time.timeScale = isOpening ? 0f : 1f;
        }
    }

    private void ToggleScreen2()
    {
        if (uiScreen2 == null) return;
        bool isOpening = !uiScreen2.activeSelf;
        uiScreen2.SetActive(isOpening);

            if (pauseGameWhileOpen)
        {
            Time.timeScale = isOpening ? 0f : 1f;
        }
    }
}
