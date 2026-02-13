using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Delay before löading")]
    public float delaySeconds = 1f;

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadAfterDelay(sceneName));
    }

    public void LoadSceneByIndex(int index)
    {
        StartCoroutine(LoadAfterDelay(index));
    }

    private IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSecondsRealtime(delaySeconds);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadAfterDelay(int index)
    {
        yield return new WaitForSecondsRealtime(delaySeconds);
        SceneManager.LoadScene(index);
    }
}
