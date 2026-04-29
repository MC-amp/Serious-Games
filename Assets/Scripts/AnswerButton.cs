using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Scene References")]
    public GameObject insectsSmallParent;
    public GameObject insectsLargeParent;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    [Header("On Correct: FlyAway Selected Icon")]
    public bool flyAwayUsesAnimationEvent = true;

    [Header("UI")]
    public Animator listAnimator;
    public string exitAnimationName = "Exit";

    [Header("Rank System")]
    public RankSystem rankSystem;

    [Header("Toggle Reset")]
    public List<Toggle> exemptToggles = new List<Toggle>();

    [Header("Compendium Reminder")]
    public CompendiumReminderController compendiumReminder;

    private Button button;

    private const float LOCK_ANSWER_SECONDS = 2f;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(CheckAnswer);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (correctGroup != null) correctGroup.alpha = 0f;
        if (wrongGroup != null) wrongGroup.alpha = 0f;
    }

    void TurnOffAllToggles()
    {
        Toggle[] toggles = FindObjectsOfType<Toggle>(true);

        foreach (Toggle toggle in toggles)
        {
            if (exemptToggles.Contains(toggle))
                continue;

            toggle.isOn = false;
        }
    }

    void CheckAnswer()
    {
        if (InsectSelectionManager.Instance == null)
            return;

        string selectedTag = InsectSelectionManager.Instance.GetSelectedTag();
        if (string.IsNullOrEmpty(selectedTag))
            return;

        StopAllCoroutines();

        bool isCorrect = (selectedTag == answerTag);

        if (isCorrect)
        {
            if (compendiumReminder != null)
                compendiumReminder.RegisterCorrect();

            PlaySfx(correctClip);

            GameObject selected = InsectSelectionManager.Instance.currentlySelected;
            if (selected != null)
            {
                IdentifyBugState bugState = selected.GetComponent<IdentifyBugState>();
                if (bugState != null)
                {
                    bugState.MarkSolved();
                }
            }

            if (rankSystem != null)
                rankSystem.AddCorrectAnswer();

            if (correctGroup != null)
                StartCoroutine(ShowAndFade(correctGroup));

            StartCoroutine(CorrectFlowRoutine());
        }
        else
        {
            if (compendiumReminder != null)
                compendiumReminder.RegisterWrong();

            PlaySfx(wrongClip);

            if (wrongGroup != null)
                StartCoroutine(ShowAndFade(wrongGroup));
        }
    }

    IEnumerator CorrectFlowRoutine()
    {
        yield return new WaitForSecondsRealtime(LOCK_ANSWER_SECONDS);

        TurnOffAllToggles();

        if (listAnimator != null)
            listAnimator.Play(exitAnimationName);

        if (insectsSmallParent != null)
            insectsSmallParent.SetActive(true);

        if (insectsLargeParent != null)
        {
            for (int i = 0; i < insectsLargeParent.transform.childCount; i++)
                insectsLargeParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return null;

        FlyAwaySelectedIcon();
    }

    void FlyAwaySelectedIcon()
    {
        GameObject selected = InsectSelectionManager.Instance.currentlySelected;
        if (selected == null) return;

        InsectButton insectBtn = selected.GetComponent<InsectButton>();
        if (insectBtn != null)
            insectBtn.PlayFlyAwayAndDisable(flyAwayUsesAnimationEvent);
    }

    private float GetSavedUIVolume()
    {
        return PlayerPrefs.GetFloat("UIVolume", 1f);
    }

    void PlaySfx(AudioClip clip)
    {
        if (UIButtonSFX.suppressSfx)
            return;

        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, GetSavedUIVolume());
    }

    IEnumerator ShowAndFade(CanvasGroup group)
    {
        if (correctGroup != null) correctGroup.alpha = 0f;
        if (wrongGroup != null) wrongGroup.alpha = 0f;

        group.alpha = 1f;

        yield return new WaitForSecondsRealtime(visibleTime);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        group.alpha = 0f;
    }
}