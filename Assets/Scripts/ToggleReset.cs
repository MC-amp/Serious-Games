using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleReset : MonoBehaviour
{
    [Header("Töggles to Ignore")]
    public List<Toggle> exemptToggles = new List<Toggle>();

    public void TurnOffAllToggles()
    {
        Toggle[] toggles = FindObjectsOfType<Toggle>(true);

        foreach (Toggle toggle in toggles)
        {
            if (exemptToggles.Contains(toggle))
                continue;

            toggle.isOn = false;
        }
    }
}