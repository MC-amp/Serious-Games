using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumToggleController : MonoBehaviour
{
    [Header("Wiring")]
    public Toggle bookToggle;
    public GameObject bookRoot;
    public Animator bookAnimator;

    [Header("Animator")]
    public string isOpenParam = "IsOpen";
    public float closeDisableDelay = 0.35f;

    Coroutine closeRoutine;

    void Awake()
    {
        if (bookRoot != null) bookRoot.SetActive(false);

        if (bookToggle != null)
            bookToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDestroy()
    {
        if (bookToggle != null)
            bookToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
            Open();
        else
            Close();
    }

    public void Open()
    {
        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }

        bookRoot.SetActive(true);
        bookAnimator.SetBool(isOpenParam, true);
    }

    public void Close()
    {
        bookAnimator.SetBool(isOpenParam, false);

        if (closeRoutine != null)
            StopCoroutine(closeRoutine);

        closeRoutine = StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSecondsRealtime(closeDisableDelay);

        if (bookAnimator.GetBool(isOpenParam) == false)
            bookRoot.SetActive(false);

        closeRoutine = null;
    }
}
