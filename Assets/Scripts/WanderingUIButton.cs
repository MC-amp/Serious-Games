using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class WanderingUIButton : MonoBehaviour
{
    [Header("Böundary")]
    public UIBoundaryPolygon boundaryPolygon;

    [Header("Movement")]
    public float moveSpeed = 60f;
    public float directionChangeIntervalMin = 1.0f;
    public float directionChangeIntervalMax = 2.5f;

    [Header("Turning")]
    public float bounceAngleVariance = 35f;
    public float probeDistance = 20f;

    [Header("Wobble")]
    public float wobbleStrength = 15f;
    public float wobbleSpeed = 1.5f;
    public bool randomizeWobblePerBug = true;

    [Header("Facing")]
    public bool faceMovementDirection = true;
    public float rotationLerpSpeed = 10f;

    [Header("Pause Behavior")]
    public bool usePauses = true;
    [Range(0f, 1f)] public float pauseChancePerSecond = 0.08f;
    public float pauseDurationMin = 0.3f;
    public float pauseDurationMax = 1.0f;

    [Header("Dart Behavior")]
    public bool useDarts = true;
    [Range(0f, 1f)] public float dartChancePerSecond = 0.06f;
    public float dartDurationMin = 0.25f;
    public float dartDurationMax = 0.6f;
    public float dartSpeedMultiplier = 2.2f;

    [Header("Bug Avoidance")]
    public bool avoidOtherBugs = true;
    public float avoidanceRadius = 100f;
    public float avoidanceStrength = 0.9f;
    public float avoidanceCheckInterval = 0.12f;
    public float avoidanceDartDuration = 0.25f;

    [Header("Optional")]
    public bool stopIfButtonDisabled = true;
    public bool stopIfInactive = true;

    [Header("Debug")]
    public bool debugLogs = true;
    public bool drawDebugLine = true;

    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Button button;

    private Vector2 direction;
    private float directionTimer;
    private float debugTimer;

    private float pauseTimer;
    private float dartTimer;

    private WanderingUIButton[] allBugs;
    private float avoidanceCheckTimer;

    private bool IsPaused => pauseTimer > 0f;
    private bool IsDarting => dartTimer > 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent as RectTransform;
        button = GetComponent<Button>();

        if (randomizeWobblePerBug)
        {
            wobbleSpeed *= Random.Range(0.8f, 1.2f);
            wobbleStrength *= Random.Range(0.8f, 1.2f);
        }

        PickRandomDirection();
        ResetDirectionTimer();

        allBugs = FindObjectsOfType<WanderingUIButton>(true);

        if (debugLogs)
        {
            Debug.Log($"[{name}] Awake | parentRect={(parentRect != null ? parentRect.name : "NULL")} | boundary={(boundaryPolygon != null ? boundaryPolygon.name : "NULL")}");
        }
    }

    private void Update()
    {
        if (boundaryPolygon == null || parentRect == null)
            return;

        if (stopIfInactive && !gameObject.activeInHierarchy)
            return;

        if (stopIfButtonDisabled && button != null && !button.interactable)
            return;

        if (boundaryPolygon.PointCount < 3)
            return;

        UpdateBehaviorTimers();
        TryStartPauseOrDart();
        HandleBugAvoidance();

        directionTimer -= Time.unscaledDeltaTime;
        if (directionTimer <= 0f)
        {
            PickRandomDirection();
            ResetDirectionTimer();
        }

        if (!IsPaused)
            MoveBug();

        if (faceMovementDirection)
            UpdateFacing();
    }

    private void HandleBugAvoidance()
    {
        if (!avoidOtherBugs)
            return;

        avoidanceCheckTimer -= Time.unscaledDeltaTime;
        if (avoidanceCheckTimer > 0f)
            return;

        avoidanceCheckTimer = avoidanceCheckInterval;

        if (allBugs == null || allBugs.Length == 0)
            allBugs = FindObjectsOfType<WanderingUIButton>(true);

        Vector2 myPos = rectTransform.anchoredPosition;
        Vector2 flee = Vector2.zero;
        int nearbyCount = 0;

        for (int i = 0; i < allBugs.Length; i++)
        {
            var other = allBugs[i];
            if (other == null || other == this) continue;
            if (!other.gameObject.activeInHierarchy) continue;
            if (other.parentRect != parentRect) continue;

            Vector2 otherPos = other.rectTransform.anchoredPosition;
            Vector2 offset = myPos - otherPos;
            float distance = offset.magnitude;

            if (distance <= 0.001f || distance > avoidanceRadius)
                continue;

            float weight = 1f - (distance / avoidanceRadius);
            flee += offset.normalized * weight;
            nearbyCount++;
        }

        if (nearbyCount == 0 || flee.sqrMagnitude < 0.0001f)
            return;

        direction = Vector2.Lerp(direction, flee.normalized, avoidanceStrength).normalized;

        if (useDarts)
            dartTimer = Mathf.Max(dartTimer, avoidanceDartDuration);

        pauseTimer = 0f;
        ResetDirectionTimer();
    }

    private void UpdateBehaviorTimers()
    {
        if (pauseTimer > 0f) pauseTimer -= Time.unscaledDeltaTime;
        if (dartTimer > 0f) dartTimer -= Time.unscaledDeltaTime;
    }

    private void TryStartPauseOrDart()
    {
        if (IsPaused || IsDarting) return;

        float dt = Time.unscaledDeltaTime;

        if (usePauses && Random.value < pauseChancePerSecond * dt)
        {
            pauseTimer = Random.Range(pauseDurationMin, pauseDurationMax);
            return;
        }

        if (useDarts && Random.value < dartChancePerSecond * dt)
        {
            dartTimer = Random.Range(dartDurationMin, dartDurationMax);
            PickRandomDirection();
            ResetDirectionTimer();
        }
    }

    private void MoveBug()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 moveDir = GetWobbledDirection();

        float speed = moveSpeed;
        if (IsDarting)
            speed *= dartSpeedMultiplier;

        Vector2 nextPos = currentPos + moveDir * speed * Time.unscaledDeltaTime;
        Vector2 probePos = nextPos + moveDir * probeDistance;

        if (!boundaryPolygon.IsInside(nextPos, parentRect) ||
            !boundaryPolygon.IsInside(probePos, parentRect))
        {
            BounceAway();
            return;
        }

        rectTransform.anchoredPosition = nextPos;
    }

    private Vector2 GetWobbledDirection()
    {
        float angleOffset = Mathf.Sin(Time.unscaledTime * wobbleSpeed) * wobbleStrength;
        float rad = angleOffset * Mathf.Deg2Rad;

        return new Vector2(
            direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad),
            direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad)
        ).normalized;
    }

    private void BounceAway()
    {
        direction = -direction;
        pauseTimer = 0f;
        ResetDirectionTimer();
    }

    private void PickRandomDirection()
    {
        direction = Random.insideUnitCircle.normalized;
    }

    private void ResetDirectionTimer()
    {
        directionTimer = Random.Range(directionChangeIntervalMin, directionChangeIntervalMax);
    }

    private void UpdateFacing()
    {
        Vector2 dir = GetWobbledDirection();
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        rectTransform.localRotation = Quaternion.Lerp(
            rectTransform.localRotation,
            Quaternion.Euler(0f, 0f, angle),
            Time.unscaledDeltaTime * rotationLerpSpeed
        );
    }
}