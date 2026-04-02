using UnityEngine;

public class BugPartToggle : MonoBehaviour
{
    public string partType; // "Head", "Body", "Leg", "Wing"

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    private bool placed = false;

    // Tracks the currently active part for each type
    private static System.Collections.Generic.Dictionary<string, BugPartToggle> activeParts
        = new System.Collections.Generic.Dictionary<string, BugPartToggle>();

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Store the original position and scale so we can reset
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    // Call this to toggle placing or removing this part
    public void TogglePart()
    {
        if (!placed)
        {
            // If another part of this type exists, reset it
            if (activeParts.ContainsKey(partType) && activeParts[partType] != this)
            {
                activeParts[partType].ResetPart();
            }

            // Set position and scale based on part type
            Vector2 targetPosition = Vector2.zero;
            Vector3 targetScale = Vector3.one;

            if (partType == "Head")
            {
                targetPosition = new Vector2(-1063, 275);
                targetScale = new Vector3(2, 2, 1);
            }
            else if (partType == "Body")
            {
                targetPosition = new Vector2(-1063, -1);
                targetScale = new Vector3(2, 4, 1);
            }
            else if (partType == "Leg")
            {
                targetPosition = new Vector2(-1063, -68);
                targetScale = new Vector3(8, 8, 1);
            }
            else if (partType == "Wing")
            {
                targetPosition = new Vector2(-1063, 38);
                targetScale = new Vector3(8, 8, 1);
            }

            rectTransform.anchoredPosition = targetPosition;
            rectTransform.localScale = targetScale;

            activeParts[partType] = this;
            placed = true;
        }
        else
        {
            ResetPart();
        }
    }

    // Resets the part back to its original position and scale
    public void ResetPart()
    {
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;

        if (activeParts.ContainsKey(partType) && activeParts[partType] == this)
        {
            activeParts.Remove(partType);
        }

        placed = false;
    }

    // Returns whether this part is currently placed (used by FinishChecker)
    public bool IsPlaced()
    {
        return placed;
    }

    // Ensure that if the object is destroyed, it removes itself from activeParts
    void OnDestroy()
    {
        if (activeParts.ContainsKey(partType) && activeParts[partType] == this)
        {
            activeParts.Remove(partType);
        }
    }
}