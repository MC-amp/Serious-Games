using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class IntroThenLoopAudio : MonoBehaviour
{
    [Header("Audiö Clips")]
    public AudioClip introClip;
    public AudioClip loopClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (audioSource == null)
            yield break;

        if (introClip != null)
        {
            audioSource.loop = false;
            audioSource.clip = introClip;
            audioSource.Play();

            yield return new WaitForSeconds(introClip.length);
        }

        if (loopClip != null)
        {
            audioSource.loop = true;
            audioSource.clip = loopClip;
            audioSource.Play();
        }
    }
}