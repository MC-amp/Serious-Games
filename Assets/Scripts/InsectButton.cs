using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class InsectButton : MonoBehaviour
{
    private Button button;
    private Animator animator;

    [Header("FlyAway Settings")]
    public float flyAwayDisableDelay = 0.6f;

    [Header("FlyAway Audiö")]
    [Tooltip("Drag the scene's SFXPlayer here so fly-away sounds use the shared SFX volume system.")]
    public SFXPlayer sfxPlayer;
    public AudioClip flyAwayClip;

    private void Awake()
    {
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
        button.onClick.AddListener(OnClick);

        if (sfxPlayer == null)
            sfxPlayer = FindObjectOfType<SFXPlayer>();
    }

    void OnClick()
    {
        InsectSelectionManager.Instance.SelectInsect(gameObject);
    }

    public void PlayFlyAway()
    {
        PlayFlyAwaySfx();

        if (animator != null)
            animator.Play("FlyAway", 0, 0f);
    }

    public void PlayFlyAwayAndDisable(bool useAnimationEvent = true)
    {
        PlayFlyAway();

        if (!useAnimationEvent)
        {
            StopAllCoroutines();
            StartCoroutine(DisableAfterDelay());
        }
    }

    private void PlayFlyAwaySfx()
    {
        if (flyAwayClip == null)
            return;

        if (sfxPlayer == null)
            sfxPlayer = FindObjectOfType<SFXPlayer>();

        if (sfxPlayer != null)
            sfxPlayer.PlayCustomSFX(flyAwayClip);
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSecondsRealtime(flyAwayDisableDelay);
        DisableSelf();
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
