using UnityEngine;

public class ResetBuildABugButton : MonoBehaviour
{
    public void ResetProgress()
    {
        BugBuildGameManager.ResetBuildABugProgress();

        if (GlobalProgressManager.Instance != null)
        {
            // Only use this if your GlobalProgressManager has a reset method.
            // GlobalProgressManager.Instance.ResetBuildABugProgress();
        }
    }
}