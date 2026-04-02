using UnityEngine;
using UnityEngine.UI;

public class SpeciesBar : MonoBehaviour
{
    [SerializeField] private string speciesId;
    [SerializeField] private Image fillImage;
    [SerializeField] private float lerpSpeed = 5f;

    float targetFill;

    public string SpeciesId => speciesId;

    public void SetFill01(float value)
    {
        targetFill = Mathf.Clamp01(value);
    }

    void Update()
    {
        if (fillImage == null) return;

        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            Time.deltaTime * lerpSpeed
        );
    }
}