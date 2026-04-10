using System.Collections.Generic;
using UnityEngine;

public class MainMenuStarDisplay : MonoBehaviour
{
    [System.Serializable]
    public class StarThreshold
    {
        public int requiredCorrect = 1;
        public GameObject starObject;
    }

    [Header("Identify Stars")]
    public List<StarThreshold> identifyStars = new List<StarThreshold>();

    [Header("Build-A-Bug Stars")]
    public List<StarThreshold> buildABugStars = new List<StarThreshold>();

    private void Start()
    {
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (GlobalProgressManager.Instance == null)
            return;

        int identifyCount = GlobalProgressManager.Instance.IdentifyCorrectCount;
        int buildABugCount = GlobalProgressManager.Instance.BuildABugCorrectCount;

        UpdateStarList(identifyStars, identifyCount);
        UpdateStarList(buildABugStars, buildABugCount);
    }

    private void UpdateStarList(List<StarThreshold> stars, int currentCount)
    {
        foreach (var star in stars)
        {
            if (star.starObject == null)
                continue;

            bool shouldBeOn = currentCount >= star.requiredCorrect;
            star.starObject.SetActive(shouldBeOn);
        }
    }
}