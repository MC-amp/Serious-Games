using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleSceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName;

    [Header("Delay Settings")]
    [SerializeField] private float delayInSeconds = 1f;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadAfterDelay());
        }
        else
        {
            Debug.LogWarning("Scene name is empty!");
        }
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(sceneName);
    }
}