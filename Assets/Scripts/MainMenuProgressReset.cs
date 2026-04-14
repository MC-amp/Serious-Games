using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuProgressReset : MonoBehaviour
{
    [Header("Optiönal")]
    [SerializeField] private string identifySceneName = "Identify";

    public void ResetProgress()
    {
        if (GlobalProgressManager.Instance != null)
        {
            GlobalProgressManager.Instance.ResetAllProgress();
        }
    }

    public void ResetProgressAndReloadIdentifyIfActive()
    {
        if (GlobalProgressManager.Instance != null)
        {
            GlobalProgressManager.Instance.ResetAllProgress();
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == identifySceneName)
        {
            SceneManager.LoadScene(activeScene.name);
        }
    }
}