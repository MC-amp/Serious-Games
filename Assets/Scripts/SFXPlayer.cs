using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource source;

    public AudioClip clickClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    private float GetSavedUIVolume()
    {
        return PlayerPrefs.GetFloat("UIVolume", 1f);
    }

    public void PlayClick()
    {
        if (source != null && clickClip != null)
            source.PlayOneShot(clickClip, GetSavedUIVolume());
    }

    public void PlayCorrect()
    {
        if (source != null && correctClip != null)
            source.PlayOneShot(correctClip, GetSavedUIVolume());
    }

    public void PlayWrong()
    {
        if (source != null && wrongClip != null)
            source.PlayOneShot(wrongClip, GetSavedUIVolume());
    }

    public void PlayCustomSFX(AudioClip clip)
    {
        if (source != null && clip != null)
            source.PlayOneShot(clip, GetSavedUIVolume());
    }
}