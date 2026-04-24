using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Result Timing")]
    public float wrongResultDuration = 2f;
    public float correctResultDuration = 2.5f;

    [Header("Finish Button")]
    public Button finishButton;

    [Header("Completion")]
    [Tooltip("Turns on after the final correct result finishes showing.")]
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

    [Header("Tutorial")]
    public BugTutorialController tutorialController;

    [Header("Audio")]
    public SFXPlayer sfxPlayer;

    [Header("Bug Prompts (9 total)")]
    public List<BugPrompt> prompts;

    private int currentPromptIndex = 0;
    private bool isChecking = false;

    void Start()
    {
        if (finishButton != null)
            finishButton.onClick.AddListener(CheckBuild);

        if (completionAsset != null)
            completionAsset.SetActive(false);

        HideResult();
        UpdatePrompt();
        UpdateStarDisplay();
    }

    void Update()
    {
        if (AllSlotsFilled() && tutorialController != null)
        {
            tutorialController.NotifyAllPartsPlaced();
        }
    }

    public void CheckBuild()
    {
        if (tutorialController != null)
            tutorialController.NotifyFinishClicked();

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

        if (GlobalProgressManager.Instance != null)
            GlobalProgressManager.Instance.AddBuildABugCorrect();

        yield return new WaitForSeconds(correctResultDuration);

        headWheel.RemovePart(usedHead);
        bodyWheel.RemovePart(usedBody);
        legWheel.RemovePart(usedLeg);
        wingWheel.RemovePart(usedWing);

        ClearAllSlots();

        currentPromptIndex++;

        if (currentPromptIndex >= prompts.Count)
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

            isChecking = false;
            yield break;
        }

        UpdatePrompt();
        isChecking = false;
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
}