using UnityEngine;

public class SceneButton : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneLoader.Instance is null.");
        }
    }
}
