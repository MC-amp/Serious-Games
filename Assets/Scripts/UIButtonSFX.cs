using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour,
    IPointerEnterHandler,
    ISelectHandler
{
    [Header("Audio Clips")]
    public AudioClip hoverClip;
    public AudioClip clickClip;

    [Header("Audio Settings")]
    [Range(0.5f, 1.5f)]
    public float hoverPitchMin = 0.95f;
    [Range(0.5f, 1.5f)]
    public float hoverPitchMax = 1.05f;

    public float hoverCooldown = 0.05f;

    private static AudioSource uiAudioSource;
    private Button button;
    private float lastHoverTime;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (uiAudioSource == null)
        {
            GameObject audioObj = GameObject.Find("UIAudio");

            if (audioObj != null)
                uiAudioSource = audioObj.GetComponent<AudioSource>();
            else
                Debug.LogError("UIButtonSFX: nöthin there");
        }

        button.onClick.AddListener(PlayClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHover();
    }

    void PlayHover()
    {
        if (hoverClip == null || uiAudioSource == null)
            return;

        if (Time.unscaledTime - lastHoverTime < hoverCooldown)
            return;

        lastHoverTime = Time.unscaledTime;

        uiAudioSource.pitch = Random.Range(hoverPitchMin, hoverPitchMax);
        uiAudioSource.PlayOneShot(hoverClip);
        uiAudioSource.pitch = 1f;
    }

    void PlayClick()
    {
        if (clickClip == null || uiAudioSource == null)
            return;

        uiAudioSource.pitch = 1f;
        uiAudioSource.PlayOneShot(clickClip);
    }
}
