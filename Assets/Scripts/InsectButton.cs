using UnityEngine;
using UnityEngine.UI;

public class InsectButton : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        InsectSelectionManager.Instance.SelectInsect(gameObject);
        Debug.Log("Selected: " + gameObject.name);
    }
}
