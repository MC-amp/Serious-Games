using UnityEngine;

public class HideIfSessionFlagSet : MonoBehaviour
{
    [SerializeField] private string flagId;

    private void Start()
    {
        if (GlobalProgressManager.Instance != null &&
            GlobalProgressManager.Instance.HasFlag(flagId))
        {
            gameObject.SetActive(false);
        }
    }
}