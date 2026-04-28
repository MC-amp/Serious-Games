using UnityEngine;

public class MusicVolumeApplier : MonoBehaviour
{
    [Header("Music Source")]
    public AudioSource musicSource;

    [Header("Base Volume")]
    [Range(0f, 1f)]
    public float baseVolume = 1f;

    private void Awake()
    {
        // Auto grab AudioSource if you forget to assign it
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("MusicVolumeApplier: No AudioSource found on " + gameObject.name);
            return;
        }

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        musicSource.volume = baseVolume * savedVolume;
    }

    // This updates ALL music objects in the scene
    public static void ApplyToAllMusicSources()
    {
        MusicVolumeApplier[] allMusic =
            FindObjectsByType<MusicVolumeApplier>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (MusicVolumeApplier music in allMusic)
        {
            music.ApplyVolume();
        }
    }
}