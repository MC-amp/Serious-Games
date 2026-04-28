using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BugPartOption
{
    public string partID;
    public Sprite wheelSprite;
    public Sprite buildSprite;
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

    private int currentIndex = 0;

    void Awake()
    {
        AddPreviewClick(leftPreview, ScrollLeft);
        AddPreviewClick(rightPreview, ScrollRight);
    }

    void Start()
    {
        if (leftButton != null)
            leftButton.onClick.AddListener(ScrollLeft);

        if (rightButton != null)
            rightButton.onClick.AddListener(ScrollRight);

        ClampIndex();
        UpdateDisplay();
    }

    private void AddPreviewClick(Image image, UnityEngine.Events.UnityAction action)
    {
        if (image == null) return;

        Button button = image.GetComponent<Button>();

        if (button == null)
            button = image.gameObject.AddComponent<Button>();

        button.onClick.AddListener(action);
    }

    public void ScrollRight()
    {
        if (partData == null || partData.parts == null || partData.parts.Count == 0) return;

        currentIndex = (currentIndex + 1) % partData.parts.Count;
        UpdateDisplay();
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
    }

    public void RemovePart(BugPartOption usedPart)
    {
        if (partData == null || partData.parts == null || usedPart == null) return;

        partData.parts.Remove(usedPart);

        ClampIndex();
        UpdateDisplay();
    }

    public void RemovePartByID(string partID)
    {
        if (partData == null || partData.parts == null || string.IsNullOrEmpty(partID)) return;

        partData.parts.RemoveAll(part => part.partID == partID);

        ClampIndex();
        UpdateDisplay();
    }

    public void RemovePartsByIDs(List<string> partIDs)
    {
        if (partIDs == null) return;

        foreach (string id in partIDs)
        {
            RemovePartByID(id);
        }

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

        centerDisplay.sprite = partData.parts[currentIndex].wheelSprite;
        leftPreview.sprite = partData.parts[leftIndex].wheelSprite;
        rightPreview.sprite = partData.parts[rightIndex].wheelSprite;
    }
}