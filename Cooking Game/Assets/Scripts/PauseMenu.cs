using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public GameObject Container;
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
                Container.SetActive(true);
                // Show pause menu UI
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1;
                Container.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                // Hide pause menu UI
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Container.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
