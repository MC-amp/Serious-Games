using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TMPGlowBreath : MonoBehaviour
{
    [Header("Breathing")]
    public float speed = 2f;

    [Tooltip("Glow power range")]
    public float minGlow = 0f;
    public float maxGlow = 1f;

    [Tooltip("Face Brightness")]
    public bool alsoPulseFaceColor = false;

    [Range(0f, 1f)]
    public float facePulseAmount = 0.15f;

    private TMP_Text text;
    private Material runtimeMat;
    private Color originalFaceColor;

    private static readonly int GlowPowerId = Shader.PropertyToID("_GlowPower");
    private static readonly int FaceColorId = Shader.PropertyToID("_FaceColor");

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        originalFaceColor = text.color;

        runtimeMat = text.fontMaterial;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f; // 0..1
        float glow = Mathf.Lerp(minGlow, maxGlow, t);

        if (runtimeMat != null && runtimeMat.HasProperty(GlowPowerId))
        {
            runtimeMat.SetFloat(GlowPowerId, glow);
            text.UpdateMeshPadding();
        }

        if (alsoPulseFaceColor)
        {
            float pulse = 1f + (t - 0.5f) * 2f * facePulseAmount; // approx 1 +/- amount
            text.color = new Color(
                originalFaceColor.r * pulse,
                originalFaceColor.g * pulse,
                originalFaceColor.b * pulse,
                originalFaceColor.a
            );
        }
    }
}