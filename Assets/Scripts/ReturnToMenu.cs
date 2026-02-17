using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    public void ReturnToMenu()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(menuSceneName);
        else
            Debug.LogError("SceneLoader.Instance is null (did Initializing scene run?)");
    }
}
