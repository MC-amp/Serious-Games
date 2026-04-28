using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleReset : MonoBehaviour
{
    [Header("Toggles to Ignore")]
    public List<Toggle> exemptToggles = new List<Toggle>();

    public void TurnOffAllToggles()
    {
        Toggle[] toggles = FindObjectsByType<Toggle>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Toggle toggle in toggles)
        {
            if (toggle == null) continue;

            // CHANGED: skip settings/reminder toggles
            if (toggle.GetComponent<DoNotResetToggle>() != null)
                continue;

            if (exemptToggles.Contains(toggle))
                continue;

            toggle.isOn = false;
        }
    }
}