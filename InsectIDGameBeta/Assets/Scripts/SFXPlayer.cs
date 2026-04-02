using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clickClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    public void PlayClick()
    {
        if (source != null && clickClip != null)
            source.PlayOneShot(clickClip);
    }

    public void PlayCorrect()
    {
        if (source != null && correctClip != null)
            source.PlayOneShot(correctClip);
    }

    public void PlayWrong()
    {
        if (source != null && wrongClip != null)
            source.PlayOneShot(wrongClip);
    }
}