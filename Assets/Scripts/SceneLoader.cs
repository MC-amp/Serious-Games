using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("Persistent UI")]
    public string persistentUISceneName = "PersistentUI";

    [Header("Delay before löading (seconds)")]
    public float delaySeconds = 1f;

    private bool isLoading = false;

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

    public void LoadScene(string sceneName)
    {
        if (!isLoading)
            StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        if (delaySeconds > 0f)
            yield return new WaitForSecondsRealtime(delaySeconds);

        var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        var toUnload = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (!s.isLoaded) continue;

            if (s.name == persistentUISceneName) continue;
            if (s.name == sceneName) continue;

            toUnload.Add(s);
        }

        foreach (var s in toUnload)
        {
            var unloadOp = SceneManager.UnloadSceneAsync(s);
            while (unloadOp != null && !unloadOp.isDone) yield return null;
        }

        isLoading = false;
    }
}
