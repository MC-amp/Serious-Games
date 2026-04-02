using UnityEngine;

public class BugTutorialController : MonoBehaviour
{
    public enum TutorialStep
    {
        None,
        HeadRightArrow,
        HeadCenterClick,
        HeadSlotRemove,
        WaitingForAllParts,
        FinishButton,
        Complete
    }

    [Header("Arrow UI")]
    public RectTransform tutorialArrow;

    [Header("Targets")]
    public RectTransform headRightArrowTarget;
    public RectTransform headCenterTarget;
    public RectTransform headSlotTarget;
    public RectTransform finishButtonTarget;

    [Header("Step Offsets")]
    public Vector2 headRightArrowOffset = new Vector2(125f, 100f);
    public Vector2 headCenterOffset = new Vector2(125f, 100f);
    public Vector2 headSlotOffset = new Vector2(125f, 100f);
    public Vector2 finishButtonOffset = new Vector2(125f, 100f);

    [Header("Arrow Rotations (Z only)")]
    public float headRightArrowRotation;
    public float headCenterRotation;
    public float headSlotRotation;
    public float finishButtonRotation;

    public TutorialStep currentStep = TutorialStep.HeadRightArrow;

    void Start()
    {
        UpdateArrow();
    }

    public void NotifyHeadRightArrowClicked()
    {
        if (currentStep != TutorialStep.HeadRightArrow) return;

        currentStep = TutorialStep.HeadCenterClick;
        UpdateArrow();
    }

    public void NotifyHeadCenterClicked()
    {
        if (currentStep != TutorialStep.HeadCenterClick) return;

        currentStep = TutorialStep.HeadSlotRemove;
        UpdateArrow();
    }

    public void NotifyHeadSlotRemoved()
    {
        if (currentStep != TutorialStep.HeadSlotRemove) return;

        // Go back to the head display first
        currentStep = TutorialStep.HeadCenterClick;
        UpdateArrow();
    }

    public void NotifyWaitingForAllParts()
    {
        if (currentStep != TutorialStep.HeadCenterClick) return;

        currentStep = TutorialStep.WaitingForAllParts;
        HideArrow();
    }

    public void NotifyAllPartsPlaced()
    {
        if (currentStep != TutorialStep.WaitingForAllParts &&
            currentStep != TutorialStep.None)
            return;

        currentStep = TutorialStep.FinishButton;
        UpdateArrow();
    }

    public void NotifyFinishClicked()
    {
        if (currentStep != TutorialStep.FinishButton) return;

        currentStep = TutorialStep.Complete;
        HideArrow();
    }

    void UpdateArrow()
    {
        if (tutorialArrow == null) return;

        RectTransform target = null;
        Vector2 offset = Vector2.zero;
        float rotationZ = 0f;

        switch (currentStep)
        {
            case TutorialStep.HeadRightArrow:
                target = headRightArrowTarget;
                offset = headRightArrowOffset;
                rotationZ = headRightArrowRotation;
                break;

            case TutorialStep.HeadCenterClick:
                target = headCenterTarget;
                offset = headCenterOffset;
                rotationZ = headCenterRotation;
                break;

            case TutorialStep.HeadSlotRemove:
                target = headSlotTarget;
                offset = headSlotOffset;
                rotationZ = headSlotRotation;
                break;

            case TutorialStep.FinishButton:
                target = finishButtonTarget;
                offset = finishButtonOffset;
                rotationZ = finishButtonRotation;
                break;

            case TutorialStep.WaitingForAllParts:
            case TutorialStep.None:
            case TutorialStep.Complete:
                HideArrow();
                return;
        }

        if (target == null)
        {
            HideArrow();
            return;
        }

        tutorialArrow.gameObject.SetActive(true);
        tutorialArrow.anchoredPosition = target.anchoredPosition + offset;
        tutorialArrow.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    void HideArrow()
    {
        if (tutorialArrow != null)
            tutorialArrow.gameObject.SetActive(false);
    }
}