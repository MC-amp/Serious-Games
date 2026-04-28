using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumReminderController : MonoBehaviour
{
    public enum PointerMoveDirection
    {
        Horizontal,
        Vertical
    }

    [Header("Reminder UI")]
    public RectTransform reminderPanel;
    public RectTransform pointerArrow;

    [Header("Panel Positions")]
    public Vector2 hiddenPosition = new Vector2(900f, 0f);
    public Vector2 shownPosition = new Vector2(0f, 0f);

    [Header("Pointer Settings")]
    public Vector2 pointerPosition;
    public float pointerRotationZ;
    public float pointerMoveDistance = 20f;
    public float pointerMoveSpeed = 2f;
    public PointerMoveDirection pointerMoveDirection = PointerMoveDirection.Horizontal;

    [Header("Reminder Settings")]
    public int wrongsNeeded = 3;
    public float delayBeforeShowing = 2f;
    public float showTime = 4f;
    public float slideSpeed = 8f;

    [Header("Optional")]
    public Button compendiumOpenButton;

    [Header("Debug")]
    public bool debugLogs = true;

    private int wrongStreak = 0;
    private bool isShowing = false;
    private Coroutine reminderRoutine;

    void Start()
    {
        if (!PlayerPrefs.HasKey("CompendiumRemindersEnabled"))
        {
            PlayerPrefs.SetInt("CompendiumRemindersEnabled", 1);
            PlayerPrefs.Save();
        }

        if (reminderPanel != null)
            reminderPanel.anchoredPosition = hiddenPosition;

        if (pointerArrow != null)
            pointerArrow.gameObject.SetActive(false);

        if (compendiumOpenButton != null)
            compendiumOpenButton.onClick.AddListener(HideNow);

        if (debugLogs)
            Debug.Log("Reminder enabled at start: " + RemindersEnabled());
    }

    void Update()
    {
        if (reminderPanel != null)
        {
            Vector2 target = isShowing ? shownPosition : hiddenPosition;

            reminderPanel.anchoredPosition = Vector2.Lerp(
                reminderPanel.anchoredPosition,
                target,
                Time.unscaledDeltaTime * slideSpeed
            );
        }

        if (pointerArrow != null && pointerArrow.gameObject.activeSelf)
        {
            float move = Mathf.Sin(Time.unscaledTime * pointerMoveSpeed) * pointerMoveDistance;

            if (pointerMoveDirection == PointerMoveDirection.Horizontal)
                pointerArrow.anchoredPosition = pointerPosition + new Vector2(move, 0f);
            else
                pointerArrow.anchoredPosition = pointerPosition + new Vector2(0f, move);

            pointerArrow.rotation = Quaternion.Euler(0f, 0f, pointerRotationZ);
        }
    }

    public void RegisterWrong()
    {
        if (!RemindersEnabled())
        {
            if (debugLogs) Debug.Log("Reminder blocked because reminders are OFF.");
            return;
        }

        wrongStreak++;

        if (debugLogs)
            Debug.Log("Wrong streak = " + wrongStreak + "/" + wrongsNeeded);

        if (wrongStreak >= wrongsNeeded)
        {
            wrongStreak = 0;
            ShowReminder();
        }
    }

    public void RegisterCorrect()
    {
        wrongStreak = 0;

        if (debugLogs)
            Debug.Log("Correct answer. Wrong streak reset.");
    }

    public void ShowReminder()
    {
        if (!RemindersEnabled()) return;

        if (reminderRoutine != null)
            StopCoroutine(reminderRoutine);

        reminderRoutine = StartCoroutine(ShowReminderRoutine());
    }

    IEnumerator ShowReminderRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeShowing);

        if (!RemindersEnabled())
            yield break;

        isShowing = true;

        if (pointerArrow != null)
            pointerArrow.gameObject.SetActive(true);

        if (debugLogs)
            Debug.Log("Reminder showing.");

        yield return new WaitForSecondsRealtime(showTime);

        HideNow();
    }

    public void HideNow()
    {
        isShowing = false;

        if (pointerArrow != null)
            pointerArrow.gameObject.SetActive(false);

        if (reminderRoutine != null)
        {
            StopCoroutine(reminderRoutine);
            reminderRoutine = null;
        }
    }

    bool RemindersEnabled()
    {
        return PlayerPrefs.GetInt("CompendiumRemindersEnabled", 1) == 1;
    }
}