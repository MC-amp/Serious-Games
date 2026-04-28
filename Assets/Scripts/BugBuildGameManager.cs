using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class BugPrompt
{
    public Sprite promptImage;
    public BugType correctBugType;
}

public class BugBuildGameManager : MonoBehaviour
{
    [Header("Prompt UI")]
    public Image promptDisplay;

    [Header("Result UI")]
    public Image resultImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    [Header("Star UI")]
    public Image starDisplay;
    public List<Sprite> starSprites;
    private int starsEarned = 0;

    [Header("Star Milestone Audio")]
    public AudioClip threeStarClip;
    public AudioClip sixStarClip;
    public AudioClip nineStarClip;

    [Header("Tutorial")]
    public GameObject tutorialPanel;
    private static bool tutorialShownThisPlaySession = false;

    [Header("Result Timing")]
    public float wrongResultDuration = 2f;
    public float correctResultDuration = 2.5f;

    [Header("Finish Button")]
    public Button finishButton;

    [Header("Completion")]
    public GameObject completionAsset;

    [Header("Bug Slots")]
    public BugSlot headSlot;
    public BugSlot bodySlot;
    public BugSlot legSlot;
    public BugSlot wingSlot;

    [Header("Wheels")]
    public BugPartWheel headWheel;
    public BugPartWheel bodyWheel;
    public BugPartWheel legWheel;
    public BugPartWheel wingWheel;

    [Header("Audio")]
    public SFXPlayer sfxPlayer;

    [Header("Bug Prompts")]
    public List<BugPrompt> prompts;

    private int currentPromptIndex = 0;
    private bool isChecking = false;

    private static int savedStarsEarned = 0;
    private static int savedPromptIndex = 0;

    private static List<string> savedHeadIDs = new List<string>();
    private static List<string> savedBodyIDs = new List<string>();
    private static List<string> savedLegIDs = new List<string>();
    private static List<string> savedWingIDs = new List<string>();

    private List<string> usedHeadIDs = new List<string>();
    private List<string> usedBodyIDs = new List<string>();
    private List<string> usedLegIDs = new List<string>();
    private List<string> usedWingIDs = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSessionProgress()
    {
        savedStarsEarned = 0;
        savedPromptIndex = 0;

        savedHeadIDs = new List<string>();
        savedBodyIDs = new List<string>();
        savedLegIDs = new List<string>();
        savedWingIDs = new List<string>();

        tutorialShownThisPlaySession = false;
    }

    void Start()
    {
        LoadProgress();

        if (finishButton != null)
            finishButton.onClick.AddListener(CheckBuild);

        if (completionAsset != null)
            completionAsset.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        ApplySavedRemovedParts();

        HideResult();
        UpdatePrompt();
        UpdateStarDisplay();

        if (currentPromptIndex >= prompts.Count)
        {
            ShowCompletionState();
        }
    }

void Update()
{
    if (!tutorialShownThisPlaySession)
    {
        if (UnityEngine.InputSystem.Mouse.current != null &&
            UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            tutorialShownThisPlaySession = true;

            if (tutorialPanel != null)
                tutorialPanel.SetActive(true);
        }
    }
}

    public void CheckBuild()
    {
        if (isChecking) return;
        if (currentPromptIndex >= prompts.Count) return;

        if (!AllSlotsFilled())
        {
            ShowResult(false, wrongResultDuration);
            return;
        }

        BugType targetType = prompts[currentPromptIndex].correctBugType;

        bool headCorrect = headSlot.EquippedPart != null && headSlot.EquippedPart.bugType == targetType;
        bool bodyCorrect = bodySlot.EquippedPart != null && bodySlot.EquippedPart.bugType == targetType;
        bool legCorrect = legSlot.EquippedPart != null && legSlot.EquippedPart.bugType == targetType;
        bool wingCorrect = wingSlot.EquippedPart != null && wingSlot.EquippedPart.bugType == targetType;

        bool allCorrect = headCorrect && bodyCorrect && legCorrect && wingCorrect;

        if (!allCorrect)
        {
            ShowResult(false, wrongResultDuration);
            ClearIncorrectSlots(targetType);
            return;
        }

        StartCoroutine(HandleCorrectBuild());
    }

    private IEnumerator HandleCorrectBuild()
    {
        isChecking = true;

        ShowResult(true, correctResultDuration);

        BugPartOption usedHead = headSlot.EquippedPart;
        BugPartOption usedBody = bodySlot.EquippedPart;
        BugPartOption usedLeg = legSlot.EquippedPart;
        BugPartOption usedWing = wingSlot.EquippedPart;

        starsEarned++;
        UpdateStarDisplay();
        PlayStarMilestoneSound();

        SaveUsedPart(PartType.Head, usedHead.partID);
        SaveUsedPart(PartType.Body, usedBody.partID);
        SaveUsedPart(PartType.Leg, usedLeg.partID);
        SaveUsedPart(PartType.Wing, usedWing.partID);

        if (GlobalProgressManager.Instance != null)
            GlobalProgressManager.Instance.AddBuildABugCorrect();

        SaveProgress();

        yield return new WaitForSeconds(correctResultDuration);

        headWheel.RemovePart(usedHead);
        bodyWheel.RemovePart(usedBody);
        legWheel.RemovePart(usedLeg);
        wingWheel.RemovePart(usedWing);

        ClearAllSlots();

        currentPromptIndex++;
        SaveProgress();

        if (currentPromptIndex >= prompts.Count)
        {
            ShowCompletionState();
            isChecking = false;
            yield break;
        }

        UpdatePrompt();
        isChecking = false;
    }

    private void PlayStarMilestoneSound()
    {
        if (sfxPlayer == null) return;

        if (starsEarned == 3 && threeStarClip != null)
            sfxPlayer.PlayCustomSFX(threeStarClip);

        if (starsEarned == 6 && sixStarClip != null)
            sfxPlayer.PlayCustomSFX(sixStarClip);

        if (starsEarned == 9 && nineStarClip != null)
            sfxPlayer.PlayCustomSFX(nineStarClip);
    }

    private bool AllSlotsFilled()
    {
        return headSlot != null && headSlot.HasPart() &&
               bodySlot != null && bodySlot.HasPart() &&
               legSlot != null && legSlot.HasPart() &&
               wingSlot != null && wingSlot.HasPart();
    }

    private void ClearIncorrectSlots(BugType targetType)
    {
        if (headSlot != null && headSlot.HasPart() && headSlot.EquippedPart.bugType != targetType)
            headSlot.ClearSlot();

        if (bodySlot != null && bodySlot.HasPart() && bodySlot.EquippedPart.bugType != targetType)
            bodySlot.ClearSlot();

        if (legSlot != null && legSlot.HasPart() && legSlot.EquippedPart.bugType != targetType)
            legSlot.ClearSlot();

        if (wingSlot != null && wingSlot.HasPart() && wingSlot.EquippedPart.bugType != targetType)
            wingSlot.ClearSlot();
    }

    private void ClearAllSlots()
    {
        if (headSlot != null) headSlot.ClearSlot();
        if (bodySlot != null) bodySlot.ClearSlot();
        if (legSlot != null) legSlot.ClearSlot();
        if (wingSlot != null) wingSlot.ClearSlot();
    }

    private void UpdatePrompt()
    {
        if (promptDisplay != null && currentPromptIndex < prompts.Count)
        {
            promptDisplay.enabled = true;
            promptDisplay.sprite = prompts[currentPromptIndex].promptImage;
        }

        HideResult();
    }

    private void ShowResult(bool isCorrect, float duration)
    {
        if (resultImage != null)
        {
            resultImage.gameObject.SetActive(true);
            resultImage.sprite = isCorrect ? correctSprite : wrongSprite;
            resultImage.color = Color.white;
            resultImage.enabled = true;

            CancelInvoke(nameof(HideResult));
            Invoke(nameof(HideResult), duration);
        }

        if (sfxPlayer != null)
        {
            if (isCorrect)
                sfxPlayer.PlayCorrect();
            else
                sfxPlayer.PlayWrong();
        }
    }

    private void HideResult()
    {
        if (resultImage == null) return;

        resultImage.enabled = false;
        resultImage.sprite = null;
    }

    private void UpdateStarDisplay()
    {
        if (starDisplay == null || starSprites == null || starSprites.Count == 0) return;

        int index = Mathf.Clamp(starsEarned, 0, starSprites.Count - 1);
        starDisplay.sprite = starSprites[index];
        starDisplay.enabled = true;
    }

    private void ShowCompletionState()
    {
        if (promptDisplay != null)
        {
            promptDisplay.sprite = null;
            promptDisplay.enabled = false;
        }

        HideResult();

        if (completionAsset != null)
            completionAsset.SetActive(true);

        if (finishButton != null)
            finishButton.interactable = false;
    }

    private void SaveUsedPart(PartType partType, string partID)
    {
        if (string.IsNullOrEmpty(partID)) return;

        List<string> list = GetUsedList(partType);

        if (!list.Contains(partID))
            list.Add(partID);
    }

    private List<string> GetUsedList(PartType partType)
    {
        switch (partType)
        {
            case PartType.Head:
                return usedHeadIDs;
            case PartType.Body:
                return usedBodyIDs;
            case PartType.Leg:
                return usedLegIDs;
            case PartType.Wing:
                return usedWingIDs;
            default:
                return usedHeadIDs;
        }
    }

    private void ApplySavedRemovedParts()
    {
        if (headWheel != null) headWheel.RemovePartsByIDs(usedHeadIDs);
        if (bodyWheel != null) bodyWheel.RemovePartsByIDs(usedBodyIDs);
        if (legWheel != null) legWheel.RemovePartsByIDs(usedLegIDs);
        if (wingWheel != null) wingWheel.RemovePartsByIDs(usedWingIDs);
    }

    private void SaveProgress()
    {
        savedStarsEarned = starsEarned;
        savedPromptIndex = currentPromptIndex;

        savedHeadIDs = new List<string>(usedHeadIDs);
        savedBodyIDs = new List<string>(usedBodyIDs);
        savedLegIDs = new List<string>(usedLegIDs);
        savedWingIDs = new List<string>(usedWingIDs);
    }

    private void LoadProgress()
    {
        starsEarned = savedStarsEarned;
        currentPromptIndex = savedPromptIndex;

        usedHeadIDs = new List<string>(savedHeadIDs);
        usedBodyIDs = new List<string>(savedBodyIDs);
        usedLegIDs = new List<string>(savedLegIDs);
        usedWingIDs = new List<string>(savedWingIDs);
    }

    public static void ResetBuildABugProgress()
    {
        savedStarsEarned = 0;
        savedPromptIndex = 0;

        savedHeadIDs.Clear();
        savedBodyIDs.Clear();
        savedLegIDs.Clear();
        savedWingIDs.Clear();

        tutorialShownThisPlaySession = false;
    }
}