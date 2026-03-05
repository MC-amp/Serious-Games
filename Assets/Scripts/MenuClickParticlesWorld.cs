using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuClickParticlesWorld : MonoBehaviour
{
    public ParticleSystem particlePrefab;

    public Camera mainCamera;
    public float depthFromCamera = 5f;

    public bool ignoreClicksOverUI = false;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (particlePrefab == null)
        {
            Debug.LogError("ClickParticles: where da particle?");
            enabled = false;
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (ignoreClicksOverUI &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 sp = new Vector3(screenPos.x, screenPos.y, depthFromCamera);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(sp);

        ParticleSystem ps = Instantiate(particlePrefab, worldPos, Quaternion.identity);

        ps.Play();

        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }
}
