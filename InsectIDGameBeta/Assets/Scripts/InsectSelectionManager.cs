using UnityEngine;

public class InsectSelectionManager : MonoBehaviour
{
    public static InsectSelectionManager Instance;

    public GameObject currentlySelected;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectInsect(GameObject insect)
    {
        currentlySelected = insect;
    }

    public string GetSelectedTag()
    {
        if (currentlySelected == null)
            return "";

        return currentlySelected.tag;
    }
}
