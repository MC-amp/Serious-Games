using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    [Header("Objects To Toggle")]
    public List<GameObject> objectsToToggle = new List<GameObject>();

    [Header("Starting State")]
    public bool startEnabled = true;

    private bool currentState;

    private void Start()
    {
        currentState = startEnabled;
        ApplyState();
    }

    public void Toggle()
    {
        currentState = !currentState;
        ApplyState();
    }

    private void ApplyState()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(currentState);
        }
    }
}