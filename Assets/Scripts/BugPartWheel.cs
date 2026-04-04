using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BugPartOption
{
    public Sprite sprite;
    public BugType bugType;
}

[System.Serializable]
public class BugPartData
{
    public PartType partType;
    public List<BugPartOption> parts;
}

public class BugPartWheel : MonoBehaviour
{
    [Header("Wheel UI")]
    public Image centerDisplay;
    public Image leftPreview;
    public Image rightPreview;

    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Bug Slot")]
    public BugSlot targetSlot;

    [Header("Part Data")]
    public BugPartData partData;

    [Header("Tutorial")]
    public BugTutorialController tutorialController;

    private int currentIndex = 0;

    // Tracks whether the head tutorial already completed the first placement cycle
    private bool headTutorialPlacedOnce = false;

    void Start()
    {
        if (leftButton != null)
            leftButton.onClick.AddListener(ScrollLeft);

        if (rightButton != null)
            rightButton.onClick.AddListener(ScrollRight);

        ClampIndex();
        UpdateDisplay();
    }

    public void ScrollRight()
    {
        if (partData == null || partData.parts == null || partData.parts.Count == 0) return;

        currentIndex = (currentIndex + 1) % partData.parts.Count;
        UpdateDisplay();

        if (tutorialController != null && partData.partType == PartType.Head)
            tutorialController.NotifyHeadRightArrowClicked();
    }

    public void ScrollLeft()
    {
        if (partData == null || partData.parts == null || partData.parts.Count == 0) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = partData.parts.Count - 1;

        UpdateDisplay();
    }

    public void ApplyToSlot()
    {
        if (targetSlot == null || partData == null || partData.parts == null || partData.parts.Count == 0) return;

        targetSlot.SetPart(partData.parts[currentIndex]);

        if (tutorialController != null && partData.partType == PartType.Head)
        {
            // First click on head display: point to head slot
            if (!headTutorialPlacedOnce)
            {
                tutorialController.NotifyHeadCenterClicked();
                headTutorialPlacedOnce = true;
            }
            // Second click on head display after removal: hide until all 4 are placed
            else
            {
                tutorialController.NotifyWaitingForAllParts();
            }
        }
    }

    public void RemovePart(BugPartOption usedPart)
    {
        if (partData == null || partData.parts == null || usedPart == null) return;

        partData.parts.Remove(usedPart);

        ClampIndex();
        UpdateDisplay();
    }

    private void ClampIndex()
    {
        if (partData == null || partData.parts == null || partData.parts.Count == 0)
        {
            currentIndex = 0;
            return;
        }

        if (currentIndex >= partData.parts.Count)
            currentIndex = 0;

        if (currentIndex < 0)
            currentIndex = partData.parts.Count - 1;
    }

    public void UpdateDisplay()
    {
        if (centerDisplay == null || leftPreview == null || rightPreview == null) return;

        if (partData == null || partData.parts == null || partData.parts.Count == 0)
        {
            centerDisplay.sprite = null;
            leftPreview.sprite = null;
            rightPreview.sprite = null;

            centerDisplay.enabled = false;
            leftPreview.enabled = false;
            rightPreview.enabled = false;
            return;
        }

        centerDisplay.enabled = true;
        leftPreview.enabled = true;
        rightPreview.enabled = true;

        int leftIndex = (currentIndex - 1 + partData.parts.Count) % partData.parts.Count;
        int rightIndex = (currentIndex + 1) % partData.parts.Count;

        centerDisplay.sprite = partData.parts[currentIndex].sprite;
        leftPreview.sprite = partData.parts[leftIndex].sprite;
        rightPreview.sprite = partData.parts[rightIndex].sprite;
    }
}