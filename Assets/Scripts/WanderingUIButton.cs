using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class WanderingUIButton : MonoBehaviour
{
    [Header("Boundary")]
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

        if (debugLogs)
        {
            Debug.Log($"[{name}] Awake | parentRect={(parentRect != null ? parentRect.name : "NULL")} | boundary={(boundaryPolygon != null ? boundaryPolygon.name : "NULL")}");
            Debug.Log($"[{name}] Start anchoredPosition={rectTransform.anchoredPosition} direction={direction}");
        }
    }

    private void Update()
    {
        if (boundaryPolygon == null)
        {
            LogOncePerSecond("No boundaryPolygon assigned.");
            return;
        }

        if (parentRect == null)
        {
            LogOncePerSecond("No parent RectTransform found.");
            return;
        }

        if (stopIfInactive && !gameObject.activeInHierarchy)
        {
            LogOncePerSecond("Blocked because object is inactive.");
            return;
        }

        if (stopIfButtonDisabled && button != null && !button.interactable)
        {
            LogOncePerSecond("Blocked because Button.interactable is false.");
            return;
        }

        if (boundaryPolygon.PointCount < 3)
        {
            LogOncePerSecond($"Blocked because polygon has only {boundaryPolygon.PointCount} points.");
            return;
        }

        UpdateBehaviorTimers();
        TryStartPauseOrDart();

        Vector2 currentPos = rectTransform.anchoredPosition;
        bool currentInside = boundaryPolygon.IsInside(currentPos, parentRect);

        if (!currentInside)
        {
            LogOncePerSecond($"Current position is OUTSIDE polygon. anchoredPosition={currentPos}");
        }

        directionTimer -= Time.unscaledDeltaTime;
        if (directionTimer <= 0f)
        {
            PickRandomDirection();
            ResetDirectionTimer();

            if (debugLogs)
                Debug.Log($"[{name}] Picked new direction {direction}");
        }

        if (!IsPaused)
            MoveBug();

        if (faceMovementDirection)
            UpdateFacing();
    }

    private void UpdateBehaviorTimers()
    {
        if (pauseTimer > 0f)
            pauseTimer -= Time.unscaledDeltaTime;

        if (dartTimer > 0f)
            dartTimer -= Time.unscaledDeltaTime;

        if (pauseTimer < 0f) pauseTimer = 0f;
        if (dartTimer < 0f) dartTimer = 0f;
    }

    private void TryStartPauseOrDart()
    {
        if (IsPaused || IsDarting)
            return;

        float dt = Time.unscaledDeltaTime;

        if (usePauses && Random.value < pauseChancePerSecond * dt)
        {
            pauseTimer = Random.Range(pauseDurationMin, pauseDurationMax);
            return;
        }

        if (useDarts && Random.value < dartChancePerSecond * dt)
        {
            dartTimer = Random.Range(dartDurationMin, dartDurationMax);

            // Darts feel better if the bug commits to a fresh heading.
            PickRandomDirection();
            ResetDirectionTimer();
        }
    }

    private void MoveBug()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 moveDir = GetWobbledDirection();

        float currentSpeed = moveSpeed;
        if (IsDarting)
            currentSpeed *= dartSpeedMultiplier;

        Vector2 nextPos = currentPos + moveDir * currentSpeed * Time.unscaledDeltaTime;
        Vector2 probePos = nextPos + moveDir * probeDistance;

        bool nextInside = boundaryPolygon.IsInside(nextPos, parentRect);
        bool probeInside = boundaryPolygon.IsInside(probePos, parentRect);

        if (drawDebugLine)
        {
            Vector3 worldA = rectTransform.TransformPoint(Vector3.zero);
            Vector3 localNext = new Vector3(nextPos.x, nextPos.y, 0f);
            Vector3 worldB = parentRect.TransformPoint(localNext);
            Debug.DrawLine(worldA, worldB, nextInside && probeInside ? Color.green : Color.red);
        }

        if (!nextInside || !probeInside)
        {
            LogOncePerSecond($"Move blocked. nextInside={nextInside}, probeInside={probeInside}, current={currentPos}, next={nextPos}, probe={probePos}");
            BounceAway();
            return;
        }

        rectTransform.anchoredPosition = nextPos;
    }

    private Vector2 GetWobbledDirection()
    {
        float angleOffset = Mathf.Sin(Time.unscaledTime * wobbleSpeed) * wobbleStrength;

        float radians = angleOffset * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        Vector2 wobbled = new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        );

        return wobbled.normalized;
    }

    private void BounceAway()
    {
        direction = -direction;

        float angle = Random.Range(-bounceAngleVariance, bounceAngleVariance) * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        Vector2 newDir = new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        );

        direction = newDir.normalized;

        if (direction.sqrMagnitude < 0.001f)
            direction = Random.insideUnitCircle.normalized;

        // Cancel pause on a bounce so bugs do not get stuck looking awkward at walls.
        pauseTimer = 0f;

        ResetDirectionTimer();
    }

    private void PickRandomDirection()
    {
        direction = Random.insideUnitCircle.normalized;

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.right;
    }

    private void ResetDirectionTimer()
    {
        directionTimer = Random.Range(directionChangeIntervalMin, directionChangeIntervalMax);
    }

    private void UpdateFacing()
    {
        Vector2 facingDir = GetWobbledDirection();

        if (facingDir.sqrMagnitude < 0.0001f)
            return;

        float targetAngle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        rectTransform.localRotation = Quaternion.Lerp(
            rectTransform.localRotation,
            targetRotation,
            Time.unscaledDeltaTime * rotationLerpSpeed
        );
    }

    private void LogOncePerSecond(string msg)
    {
        if (!debugLogs)
            return;

        if (Time.unscaledTime >= debugTimer)
        {
            debugTimer = Time.unscaledTime + 1f;
            Debug.Log($"[{name}] {msg}");
        }
    }
}