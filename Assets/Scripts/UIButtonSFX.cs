using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIButtonSFX : MonoBehaviour,
    IPointerEnterHandler,
    ISelectHandler
{
    [Header("Audio Clips")]
    public AudioClip hoverClip;
    public AudioClip clickClip;

    [Header("Volume")]
    [Range(0f, 2f)]
    public float hoverVolume = 1f;

    [Range(0f, 2f)]
    public float clickVolume = 1f;

    [Header("Audio Settings")]
    [Range(0.5f, 1.5f)]
    public float hoverPitchMin = 0.95f;

    [Range(0.5f, 1.5f)]
    public float hoverPitchMax = 1.05f;

    public float hoverCooldown = 0.05f;

    private static AudioSource uiAudioSource;

    private Selectable selectable;
    private Button button;
    private Toggle toggle;

    private float lastHoverTime;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        if (uiAudioSource == null)
        {
            GameObject audioObj = GameObject.Find("UIAudio");

            if (audioObj != null)
                uiAudioSource = audioObj.GetComponent<AudioSource>();
            else
                Debug.LogError("UIButtonSFX: UIAudio is missing");
        }

        if (button != null)
            button.onClick.AddListener(PlayClick);

        if (toggle != null)
            toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHover();
    }

    private void OnToggleChanged(bool isOn)
    {
        PlayClick();
    }

    private void PlayHover()
    {
        if (hoverClip == null || uiAudioSource == null)
            return;

        if (Time.unscaledTime - lastHoverTime < hoverCooldown)
            return;

        lastHoverTime = Time.unscaledTime;

        uiAudioSource.pitch = Random.Range(hoverPitchMin, hoverPitchMax);
        uiAudioSource.PlayOneShot(hoverClip, hoverVolume);
        uiAudioSource.pitch = 1f;
    }

    private void PlayClick()
    {
        if (clickClip == null || uiAudioSource == null)
            return;

        uiAudioSource.pitch = 1f;
        uiAudioSource.PlayOneShot(clickClip, clickVolume);
    }
}