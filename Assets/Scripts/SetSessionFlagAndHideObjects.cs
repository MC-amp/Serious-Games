using UnityEngine;

public class SetSessionFlagAndHideObjects : MonoBehaviour
{
    [SerializeField] private string flagId;
    [SerializeField] private GameObject[] objectsToHide;

    public void Apply()
    {
        if (GlobalProgressManager.Instance != null)
        {
            GlobalProgressManager.Instance.SetFlag(flagId);
        }

        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}