using UnityEngine;

public class DisableChildren : MonoBehaviour
{
    [Header("Target Parent")]
    public GameObject parentObject;

    public void DisableAllChildren()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("No parent assigned");
            return;
        }

        foreach (Transform child in parentObject.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}