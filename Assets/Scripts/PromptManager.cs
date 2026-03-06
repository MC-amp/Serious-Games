using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public TextMeshProUGUI promptText; // Drag your UI TextMeshPro here
    private string currentBugToBuild;

    private string[] bugs = { "Fly", "Bee", "Wasp" };

    // Call this when Start button is clicked
    public void StartBuild()
    {
        Debug.Log("Start button clicked!");
        // Pick a random bug
        int index = Random.Range(0, bugs.Length);
        currentBugToBuild = bugs[index];

        // Set prompt text and position
        promptText.text = "Build A " + currentBugToBuild;
        promptText.rectTransform.anchoredPosition = new Vector2(-1093, 566);
        promptText.gameObject.SetActive(true);
    }

    public string GetCurrentBug()
    {
        return currentBugToBuild;
    }
}