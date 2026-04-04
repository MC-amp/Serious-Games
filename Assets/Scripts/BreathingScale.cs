using UnityEngine;

public class BreathingScale : MonoBehaviour
{
    [Header("Breathing Settings")]
    public float speed = 2f;
    public float scaleAmount = 0.05f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float scale = 1f + (t - 0.5f) * 2f * scaleAmount;

        transform.localScale = baseScale * scale;
    }
}