using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BugSlot : MonoBehaviour, IPointerClickHandler
{
    public PartType partType;

    private Image image;

    public BugPartOption EquippedPart { get; private set; }

    public BugTutorialController tutorialController;

    void Awake()
    {
        image = GetComponent<Image>();

        if (image.sprite == null)
            image.enabled = false;
    }

    public void SetPart(BugPartOption part)
    {
        EquippedPart = part;
        image.sprite = part.sprite;
        image.enabled = true;
    }

    public void ClearSlot()
    {
        EquippedPart = null;
        image.sprite = null;
        image.enabled = false;
    }

    public bool HasPart()
    {
        return EquippedPart != null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool hadPart = EquippedPart != null;
        ClearSlot();

        if (hadPart && tutorialController != null && partType == PartType.Head)
        {
            tutorialController.NotifyHeadSlotRemoved();
        }
    }
}