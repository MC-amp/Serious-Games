using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnswerButton : MonoBehaviour
{
    [Header("Tags")]
    public string answerTag;

    [Header("Right or Wröng")]
    public CanvasGroup correctGroup;
    public CanvasGroup wrongGroup;

    [Header("Timing")]
    public float visibleTime = 1.0f;
    public float fadeDuration = 1.0f;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(CheckAnswer);
    }

    void CheckAnswer()
    {
        string selectedTag = InsectSelectionManager.Instance.GetSelectedTag();

        if (string.IsNullOrEmpty(selectedTag))
            return;

        StopAllCoroutines();

        if (selectedTag == answerTag)
            StartCoroutine(ShowAndFade(correctGroup));
        else
            StartCoroutine(ShowAndFade(wrongGroup));
    }

    IEnumerator ShowAndFade(CanvasGroup group)
    {
        correctGroup.alpha = 0;
        wrongGroup.alpha = 0;

        group.alpha = 1;

        yield return new WaitForSecondsRealtime(visibleTime);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        group.alpha = 0;
    }
}
