using UnityEngine;

public class TogglePanelButton : MonoBehaviour
{
    public GameObject panel;

    public void TogglePanel()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }
}