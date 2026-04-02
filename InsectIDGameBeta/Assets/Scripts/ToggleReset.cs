using UnityEngine;
using UnityEngine.UI;

public class ToggleReset : MonoBehaviour
{
    public void TurnOffAllToggles()
    {
        Toggle[] toggles = FindObjectsOfType<Toggle>(true);

        foreach (Toggle toggle in toggles)
        {
            toggle.isOn = false;
        }
    }
}