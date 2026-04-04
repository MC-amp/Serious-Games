using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class LockScreenController : MonoBehaviour
{
    [Header("Löck Duration (seconds)")]
    public float lockDuration = 2f;

    private CanvasGroup canvasGroup;
    private Coroutine lockRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }

    public void BlockRaycastsForDuration()
    {
        if (lockRoutine != null)
            StopCoroutine(lockRoutine);

        lockRoutine = StartCoroutine(BlockRoutine(lockDuration));
    }

    public void BlockRaycastsForSeconds(float seconds)
    {
        if (lockRoutine != null)
            StopCoroutine(lockRoutine);

        lockRoutine = StartCoroutine(BlockRoutine(seconds));
    }

    private IEnumerator BlockRoutine(float seconds)
    {
        canvasGroup.blocksRaycasts = true;

        yield return new WaitForSecondsRealtime(seconds);

        canvasGroup.blocksRaycasts = false;
        lockRoutine = null;
    }
}