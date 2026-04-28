using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BugSlot : MonoBehaviour, IPointerClickHandler
{
    public PartType partType;

    private Image image;

    public BugPartOption EquippedPart { get; private set; }

    void Awake()
    {
        image = GetComponent<Image>();

        if (image.sprite == null)
            image.enabled = false;
    }

    public void SetPart(BugPartOption part)
    {
        EquippedPart = part;

        if (part == null)
        {
            ClearSlot();
            return;
        }

        image.sprite = part.buildSprite;
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
        ClearSlot();
    }
}