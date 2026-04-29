using System.Collections.Generic;
using UnityEngine;

public class FullCompletionRewardDisplay : MonoBehaviour
{
    [Header("Which game is this reward screen for?")]
    public GlobalProgressManager.GameId currentGame = GlobalProgressManager.GameId.Identify;

    [Header("Turn ON when this is the FIRST completed game")]
    public List<GameObject> firstGameCompletedObjects = new List<GameObject>();

    [Header("Turn ON when this is the SECOND completed game")]
    public List<GameObject> secondGameCompletedObjects = new List<GameObject>();

    [Header("Turn ON whenever BOTH games are complete")]
    public List<GameObject> fullCompletionObjects = new List<GameObject>();

    [Header("Turn OFF whenever BOTH games are complete")]
    [Tooltip("Use this for old win text, buttons, panels, or anything that should disappear once the final completion reward is available.")]
    public List<GameObject> fullCompletionObjectsToTurnOff = new List<GameObject>();

    [Header("Always turn OFF when this screen refreshes")]
    public List<GameObject> objectsToTurnOff = new List<GameObject>();

    [Header("Options")]
    [Tooltip("If true, this script updates itself whenever GlobalProgressManager progress changes.")]
    public bool refreshWhenProgressChanges = true;

    private void OnEnable()
    {
        if (refreshWhenProgressChanges)
            GlobalProgressManager.OnProgressChanged += RefreshDisplay;

        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (refreshWhenProgressChanges)
            GlobalProgressManager.OnProgressChanged -= RefreshDisplay;
    }

    public void RefreshDisplay()
    {
        SetObjectsActive(firstGameCompletedObjects, false);
        SetObjectsActive(secondGameCompletedObjects, false);
        SetObjectsActive(fullCompletionObjects, false);
        SetObjectsActive(fullCompletionObjectsToTurnOff, true);
        SetObjectsActive(objectsToTurnOff, false);

        if (GlobalProgressManager.Instance == null)
        {
            Debug.LogWarning("FullCompletionRewardDisplay: No GlobalProgressManager exists in the scene.");
            return;
        }

        GlobalProgressManager progress = GlobalProgressManager.Instance;

        bool thisGameComplete = progress.IsGameComplete(currentGame);
        bool otherGameComplete = progress.IsOtherGameComplete(currentGame);
        bool bothGamesComplete = progress.IsFullyComplete;

        if (!thisGameComplete)
            return;

        if (bothGamesComplete || otherGameComplete)
        {
            SetObjectsActive(secondGameCompletedObjects, true);
            SetObjectsActive(fullCompletionObjects, true);
            SetObjectsActive(fullCompletionObjectsToTurnOff, false);
        }
        else
        {
            SetObjectsActive(firstGameCompletedObjects, true);
        }
    }

    private void SetObjectsActive(List<GameObject> objects, bool active)
    {
        if (objects == null)
            return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}
