using UnityEngine;
using UnityEngine.SceneManagement; // This is the required library for loading levels

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // This tells Unity to load the next scene in the list
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Button Pressed!");
        Application.Quit();
    }
}