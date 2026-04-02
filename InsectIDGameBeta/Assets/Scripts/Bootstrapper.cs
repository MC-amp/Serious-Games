using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance;

    [Header("Scenes")]
    public string persistentUISceneName = "PersistentUI";
    public string mainMenuSceneName = "MainMenu";
    public string initializingSceneName = "Initializing";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        yield return LoadIfNeeded(persistentUISceneName);
        yield return LoadIfNeeded(mainMenuSceneName);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenuSceneName));

        if (SceneManager.GetSceneByName(initializingSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(initializingSceneName);
        }
    }

    private IEnumerator LoadIfNeeded(string sceneName)
    {
        if (IsSceneLoaded(sceneName))
            yield break;

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!op.isDone) yield return null;
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
                return true;
        }
        return false;
    }
}
