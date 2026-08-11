using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // NEW: if the wiring puzzle is currently open, ESC closes it
            // (with the same punishment reset as the Cancel button) instead
            // of opening the pause menu on top of it.
            if (BookSortingManager.Instance != null
                && BookSortingManager.Instance.sortingUIPanel != null
                && BookSortingManager.Instance.sortingUIPanel.activeSelf)
            {
                BookSortingManager.Instance.CancelPuzzle();
                return;
            }

            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // SAFE APPROACH: Check if EventSystem exists before clearing it
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Locks the cursor back to the center and hides it for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Unlocks the cursor and makes it visible so you can click the UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadSettings()
    {
        // SAFE APPROACH
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Settings Menu would open here!");
    }

    public void QuitToTitle()
    {
        // SAFE APPROACH
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // We still need to unfreeze time before loading the Main Menu!
        Time.timeScale = 1f;

        // NEW: reset the paused state too — this is a static field, so it
        // survives the scene reload and would otherwise cause the next ESC
        // press in a new play session to call Resume() instead of Pause().
        GameIsPaused = false;

        SceneManager.LoadScene(0);
    }
}