using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Panel Movement")]
    public RectTransform panel;

    [Range(-5000f, 5000f)]
    public float openX = 0f;

    [Range(0f, 5000f)]
    public float slideDistance = 1500f;

    public float slideSpeed = 8f;

    [Header("Buttons")]
    public Button openSettingsButton;
    public Button backButton;
    public Button outsideCloseButton;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider uiSlider;

    [Header("Percent Text")]
    public TextMeshProUGUI musicPercentText;
    public TextMeshProUGUI uiPercentText;

    [Header("Mute Buttons")]
    public Button musicMuteButton;
    public Button uiMuteButton;

    public Image musicMuteImage;
    public Image uiMuteImage;

    public Sprite mutedSprite;
    public Sprite unmutedSprite;

    [Header("Reminder Toggle")]
    public Toggle reminderToggle;

    private bool isOpen = false;
    private float lastMusicVolume = 1f;
    private float lastUIVolume = 1f;

    void Start()
    {
        if (panel != null)
            panel.anchoredPosition = new Vector2(openX + slideDistance, panel.anchoredPosition.y);

        isOpen = false;

        if (openSettingsButton != null)
            openSettingsButton.onClick.AddListener(OpenSettings);

        if (backButton != null)
            backButton.onClick.AddListener(CloseSettings);

        if (outsideCloseButton != null)
        {
            outsideCloseButton.onClick.AddListener(CloseSettings);
            outsideCloseButton.gameObject.SetActive(false);
        }

        if (musicMuteButton != null)
            musicMuteButton.onClick.AddListener(ToggleMusicMute);

        if (uiMuteButton != null)
            uiMuteButton.onClick.AddListener(ToggleUIMute);

        // Default reminders ON only if no setting exists yet
        if (!PlayerPrefs.HasKey("CompendiumRemindersEnabled"))
        {
            PlayerPrefs.SetInt("CompendiumRemindersEnabled", 1);
            PlayerPrefs.Save();
        }

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedUI = PlayerPrefs.GetFloat("UIVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (uiSlider != null)
        {
            uiSlider.value = savedUI;
            uiSlider.onValueChanged.AddListener(SetUISounds);
        }

        lastMusicVolume = savedMusic > 0 ? savedMusic : 1f;
        lastUIVolume = savedUI > 0 ? savedUI : 1f;

        if (reminderToggle != null)
        {
            reminderToggle.onValueChanged.RemoveAllListeners();

            bool remindersOn =
                PlayerPrefs.GetInt("CompendiumRemindersEnabled", 1) == 1;

            // Important: set the visual toggle WITHOUT triggering the listener
            reminderToggle.SetIsOnWithoutNotify(remindersOn);

            reminderToggle.onValueChanged.AddListener(SetReminderToggle);
        }

        ApplyAudioToScene();
        UpdateMuteIcons();
        UpdatePercentText();

        Debug.Log("Reminder setting loaded as: " + PlayerPrefs.GetInt("CompendiumRemindersEnabled", 1));
    }

    void Update()
    {
        if (panel == null) return;

        float targetX = isOpen ? openX : openX + slideDistance;

        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            new Vector2(targetX, panel.anchoredPosition.y),
            Time.unscaledDeltaTime * slideSpeed
        );
    }

    public void OpenSettings()
    {
        isOpen = true;

        if (outsideCloseButton != null)
            outsideCloseButton.gameObject.SetActive(true);

        if (openSettingsButton != null)
            openSettingsButton.interactable = false;
    }

    public void CloseSettings()
    {
        isOpen = false;

        if (outsideCloseButton != null)
            outsideCloseButton.gameObject.SetActive(false);

        if (openSettingsButton != null)
            openSettingsButton.interactable = true;
    }

    void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        if (value > 0)
            lastMusicVolume = value;

        ApplyAudioToScene();
        UpdateMuteIcons();
        UpdatePercentText();
    }

    void SetUISounds(float value)
    {
        PlayerPrefs.SetFloat("UIVolume", value);
        PlayerPrefs.SetFloat("BookVolume", value);
        PlayerPrefs.Save();

        if (value > 0)
            lastUIVolume = value;

        ApplyAudioToScene();
        UpdateMuteIcons();
        UpdatePercentText();
    }

    void SetReminderToggle(bool enabled)
    {
        PlayerPrefs.SetInt("CompendiumRemindersEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Compendium reminders saved as: " + enabled);
    }

    void ToggleMusicMute()
    {
        if (musicSlider == null) return;

        if (musicSlider.value > 0)
        {
            lastMusicVolume = musicSlider.value;
            musicSlider.value = 0;
        }
        else
        {
            musicSlider.value = lastMusicVolume;
        }
    }

    void ToggleUIMute()
    {
        if (uiSlider == null) return;

        if (uiSlider.value > 0)
        {
            lastUIVolume = uiSlider.value;
            uiSlider.value = 0;
        }
        else
        {
            uiSlider.value = lastUIVolume;
        }
    }

    void ApplyAudioToScene()
    {
        MusicVolumeApplier.ApplyToAllMusicSources();

        BookButton[] books = FindObjectsByType<BookButton>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (BookButton book in books)
        {
            book.RefreshBookVolume();
        }
    }

    void UpdateMuteIcons()
    {
        if (musicMuteImage != null && musicSlider != null)
            musicMuteImage.sprite = musicSlider.value <= 0 ? mutedSprite : unmutedSprite;

        if (uiMuteImage != null && uiSlider != null)
            uiMuteImage.sprite = uiSlider.value <= 0 ? mutedSprite : unmutedSprite;
    }

    void UpdatePercentText()
    {
        if (musicPercentText != null && musicSlider != null)
            musicPercentText.text = Mathf.RoundToInt(musicSlider.value * 100f) + "%";

        if (uiPercentText != null && uiSlider != null)
            uiPercentText.text = Mathf.RoundToInt(uiSlider.value * 100f) + "%";
    }
}