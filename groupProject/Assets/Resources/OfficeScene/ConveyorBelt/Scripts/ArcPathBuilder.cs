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
    [Min(2)] public int waypointCount = 60;
    public bool clockwise = true;
    public float yOffset = 0f;

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
            if (UnityEngine.Application.isPlaying) Destroy(waypointParent.GetChild(i).gameObject);
            else DestroyImmediate(waypointParent.GetChild(i).gameObject);
        }

        Vector3 c = center.position;
        Vector3 s = startPoint.position;
        Vector3 e = endPoint.position;

        // Work in XZ
        Vector2 s2 = new Vector2(s.x - c.x, s.z - c.z);
        Vector2 e2 = new Vector2(e.x - c.x, e.z - c.z);

        float radius = Mathf.Max(0.001f, s2.magnitude); // always use start radius (stable circle)
        float a0 = Mathf.Atan2(s2.y, s2.x);
        float a1 = Mathf.Atan2(e2.y, e2.x);

        float delta = DeltaAngleRadians(a0, a1, clockwise);
        float y = s.y + yOffset;

        for (int i = 0; i < waypointCount; i++)
        {
            float t = (float)i / (waypointCount - 1);
            float ang = a0 + delta * t;

            Vector3 p = new Vector3(
                c.x + Mathf.Cos(ang) * radius,
                y,
                c.z + Mathf.Sin(ang) * radius
            );

            var wp = new GameObject($"WP_{i:00}");
            wp.transform.SetParent(waypointParent, false);
            wp.transform.position = p;
        }
    }

    float DeltaAngleRadians(float a0, float a1, bool cw)
    {
        float d = a1 - a0;
        while (d > Mathf.PI) d -= 2f * Mathf.PI;
        while (d < -Mathf.PI) d += 2f * Mathf.PI;

        if (cw) { if (d > 0f) d -= 2f * Mathf.PI; }
        else { if (d < 0f) d += 2f * Mathf.PI; }

        return d;
    }
}
