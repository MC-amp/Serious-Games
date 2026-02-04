using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}