using UnityEngine;
using UnityEngine.UI;

public class ToggleReset : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip resetClip;

    public void TurnOffAllToggles()
    {
        UIButtonSFX.suppressSfx = true;

        Toggle[] toggles = FindObjectsOfType<Toggle>(true);

        foreach (Toggle toggle in toggles)
        {
            toggle.isOn = false;
        }

        UIButtonSFX.suppressSfx = false;

        if (audioSource != null && resetClip != null)
        {
            audioSource.PlayOneShot(resetClip, 0.3f); // softer volume
        }
    }
}