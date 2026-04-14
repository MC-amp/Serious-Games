using UnityEngine;
using TMPro;

public class FinishChecker : MonoBehaviour
{
    [Header("References")]
    public PromptManager promptManager;      // Reference to PromptManager
    public TextMeshProUGUI messageText;      // UI Text for messages

    // Call this from Finish button OnClick
    public void CheckFinish()
    {
        string bugToBuild = promptManager.GetCurrentBug();

        if (string.IsNullOrEmpty(bugToBuild))
        {
            messageText.text = "Click Start first!";
            messageText.gameObject.SetActive(true);
            return;
        }

        bugToBuild = bugToBuild.Trim(); // clean the string

        bool allCorrect = true;

        // Track which required part types are placed correctly
        System.Collections.Generic.HashSet<string> requiredParts = new System.Collections.Generic.HashSet<string>
        {
            "Head", "Body", "Leg", "Wing"
        };

        BugPartToggle[] parts = FindObjectsOfType<BugPartToggle>();

        foreach (BugPartToggle part in parts)
        {
            // Only check parts that the player placed
            if (!part.IsPlaced())
                continue;

            string expectedTag = "Identify" + bugToBuild;

            if (!string.Equals(part.gameObject.tag.Trim(), expectedTag, System.StringComparison.OrdinalIgnoreCase))
            {
                allCorrect = false;
                Debug.Log("Incorrect part placed: " + part.name + " tag: " + part.gameObject.tag + " expected: " + expectedTag);
                break;
            }

            // Mark this part type as correctly placed
            requiredParts.Remove(part.partType);
        }

        // If there are any required parts not placed, it's not correct
        if (requiredParts.Count > 0)
        {
            allCorrect = false;
            Debug.Log("Missing parts: " + string.Join(", ", requiredParts));
        }

        // Show result
        if (allCorrect)
            messageText.text = "Congratulations! You built a " + bugToBuild + "!";
        else
            messageText.text = "Not correct. Try again!";

        messageText.gameObject.SetActive(true);
    }
}