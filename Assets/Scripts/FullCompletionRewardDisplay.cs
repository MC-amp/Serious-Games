using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullCompletionRewardDisplay : MonoBehaviour
{
    [Header("Activation Settings")]
    public float activationDelay = 1f;

    [Header("Turn ON When Complete")]
    public List<GameObject> objectsToEnable = new List<GameObject>();

    [Header("Turn OFF When Complete")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    private bool hasActivated = false;

    private void OnEnable()
    {
        // Ensure correct initial state
        SetObjects(objectsToEnable, false);

        GlobalProgressManager.OnProgressChanged += CheckFullCompletion;
        CheckFullCompletion();
    }

    private void OnDisable()
    {
        GlobalProgressManager.OnProgressChanged -= CheckFullCompletion;
    }

    private void CheckFullCompletion()
    {
        if (hasActivated)
            return;

        if (GlobalProgressManager.Instance == null)
            return;

        if (GlobalProgressManager.Instance.IdentifyCorrectCount >= 9 &&
            GlobalProgressManager.Instance.BuildABugCorrectCount >= 9)
        {
            hasActivated = true;
            StartCoroutine(ActivateAfterDelay());
        }
    }

    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        SetObjects(objectsToEnable, true);
        SetObjects(objectsToDisable, false);
    }

    private void SetObjects(List<GameObject> list, bool state)
    {
        foreach (GameObject obj in list)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}