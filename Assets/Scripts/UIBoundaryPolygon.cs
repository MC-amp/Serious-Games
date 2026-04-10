using UnityEngine;

public class UIBoundaryPolygon : MonoBehaviour
{
    [Header("Boundary Points")]
    [Tooltip("Assign the points in clockwise or counter-clockwise order.")]
    public RectTransform[] points;

    public int PointCount => points == null ? 0 : points.Length;

    public Vector2 GetPointLocalToParent(int index, RectTransform targetParent)
    {
        if (points == null || index < 0 || index >= points.Length || points[index] == null || targetParent == null)
            return Vector2.zero;

        Vector3 world = points[index].position;
        Vector3 local = targetParent.InverseTransformPoint(world);
        return local;
    }

    public bool IsInside(Vector2 testPoint, RectTransform targetParent)
    {
        if (points == null || points.Length < 3 || targetParent == null)
            return false;

        bool inside = false;
        int j = points.Length - 1;

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null || points[j] == null)
            {
                j = i;
                continue;
            }

            Vector2 pi = GetPointLocalToParent(i, targetParent);
            Vector2 pj = GetPointLocalToParent(j, targetParent);

            bool intersect =
                ((pi.y > testPoint.y) != (pj.y > testPoint.y)) &&
                (testPoint.x < (pj.x - pi.x) * (testPoint.y - pi.y) / ((pj.y - pi.y) + 0.00001f) + pi.x);

            if (intersect)
                inside = !inside;

            j = i;
        }

        return inside;
    }

    private void OnDrawGizmos()
    {
        if (points == null || points.Length < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;

            Vector3 a = points[i].position;
            Vector3 b = points[(i + 1) % points.Length] != null
                ? points[(i + 1) % points.Length].position
                : a;

            Gizmos.DrawSphere(a, 0.2f);
            Gizmos.DrawLine(a, b);
        }
    }
}