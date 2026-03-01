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

    [Header("FlyAway Audio")]
    public AudioSource audioSource;
    public AudioClip flyAwayClip;

    private void Awake()
    {
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
        button.onClick.AddListener(OnClick);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnClick()
    {
        InsectSelectionManager.Instance.SelectInsect(gameObject);
    }

    public void PlayFlyAway()
    {
        if (flyAwayClip != null && audioSource != null)
            audioSource.PlayOneShot(flyAwayClip);

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