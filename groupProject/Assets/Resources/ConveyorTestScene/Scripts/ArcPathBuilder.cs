using UnityEngine;

[ExecuteAlways]
public class ArcPathBuilder : MonoBehaviour
{
    [Header("References")]
    public Transform center;
    public Transform startPoint;
    public Transform endPoint;
    public Transform waypointParent;

    [Header("Shape")]
    public int waypointCount = 60;
    public bool clockwise = true;
    public float yOffset = 0f;

    [Header("Radius Control")]
    [Tooltip("Use radius from Start->Center. This keeps arc perfectly circular.")]
    public bool useStartRadius = true;

    [Tooltip("If enabled, End angle is respected, but its radius is snapped to Start radius.")]
    public bool snapEndToStartRadius = true;

    [Header("Rebuild")]
    public bool rebuildNow = false;

    void Update()
    {
        if (!rebuildNow) return;
        rebuildNow = false;
        Rebuild();
    }

    [ContextMenu("Rebuild Path Now")]
    public void Rebuild()
    {
        if (!center || !startPoint || !endPoint || !waypointParent)
        {
            UnityEngine.Debug.LogWarning("[ArcPathBuilder] Missing references.");
            return;
        }

        // Clear old waypoints
        for (int i = waypointParent.childCount - 1; i >= 0; i--)
        {
            if (UnityEngine.Application.isPlaying)
                Destroy(waypointParent.GetChild(i).gameObject);
            else
                DestroyImmediate(waypointParent.GetChild(i).gameObject);
        }

        Vector3 c = center.position;
        Vector3 s = startPoint.position;
        Vector3 e = endPoint.position;

        // XZ plane vectors
        Vector2 s2 = new Vector2(s.x - c.x, s.z - c.z);
        Vector2 e2 = new Vector2(e.x - c.x, e.z - c.z);

        float startRadius = s2.magnitude;
        if (startRadius < 0.001f) startRadius = 0.001f;

        float radius = useStartRadius ? startRadius : (startRadius + e2.magnitude) * 0.5f;

        float a0 = Mathf.Atan2(s2.y, s2.x);

        // End angle: keep its direction, but optionally snap its radius to start radius
        float a1;
        if (snapEndToStartRadius)
        {
            // keep angle from end, ignore its radius mismatch
            a1 = Mathf.Atan2(e2.y, e2.x);
        }
        else
        {
            a1 = Mathf.Atan2(e2.y, e2.x);
        }

        float delta = DeltaAngleRadians(a0, a1, clockwise);

        int count = Mathf.Max(2, waypointCount);
        float y = s.y + yOffset;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            float ang = a0 + delta * t;

            Vector3 p = new Vector3(
                c.x + Mathf.Cos(ang) * radius,
                y,
                c.z + Mathf.Sin(ang) * radius
            );

            GameObject wp = new GameObject($"WP_{i:00}");
            wp.transform.SetParent(waypointParent, false);
            wp.transform.position = p;
        }
    }

    float DeltaAngleRadians(float a0, float a1, bool cw)
    {
        float d = a1 - a0;

        while (d > Mathf.PI) d -= 2f * Mathf.PI;
        while (d < -Mathf.PI) d += 2f * Mathf.PI;

        if (cw)
        {
            if (d > 0f) d -= 2f * Mathf.PI;
        }
        else
        {
            if (d < 0f) d += 2f * Mathf.PI;
        }

        return d;
    }
}
