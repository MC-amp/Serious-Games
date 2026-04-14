using UnityEngine;

public class IdentifyBugState : MonoBehaviour
{
    [Header("Unique ID för this specific bug button")]
    [SerializeField] private string bugId;

    public string BugId => bugId;

    private void Start()
    {
        if (GlobalProgressManager.Instance != null &&
            GlobalProgressManager.Instance.IsIdentifyBugSolved(bugId))
        {
            gameObject.SetActive(false);
        }
    }

    public void MarkSolved()
    {
        if (GlobalProgressManager.Instance != null)
        {
            GlobalProgressManager.Instance.MarkIdentifyBugSolved(bugId);
        }
    }
}