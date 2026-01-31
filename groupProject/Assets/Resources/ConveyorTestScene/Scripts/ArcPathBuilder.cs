using UnityEngine;

[ExecuteAlways]
public class ArcPathBuilder : MonoBehaviour
{
    [Header("References")]
    public Transform center;        // Center of the conveyor circle (PathCenter)
    public Transform startPoint;    // Where objects appear (PathStart)
    public Transform endPoint;      // Where objects disappear (PathEnd)
    public Transform waypointParent; // Parent that will hold WP_00..WP_XX (GeneratedPath)

    [Header("Shape")]
    public int waypointCount = 60;  // More = smoother movement
    public bool clockwise = true;   // Direction along the arc (top view)
    public float yOffset = 0f;      // Lift path slightly if needed

    [Header("Radius Control")]
    [Tooltip("Use radius from Start->Center (keeps a perfect circle).")]
    public bool useStartRadius = true;

    [Tooltip("Keep end angle but force same radius as start (prevents 'bulging').")]
    public bool snapEndToStartRadius = true;

    [Header("Rebuild")]
    public bool rebuildNow = false; // Toggle in Inspector to regenerate waypoints

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

        // Work in XZ plane
        Vector2 s2 = new Vector2(s.x - c.x, s.z - c.z);
        Vector2 e2 = new Vector2(e.x - c.x, e.z - c.z);

        float startRadius = s2.magnitude;
        if (startRadius < 0.001f) startRadius = 0.001f;

        // Use start radius (recommended) or average radius
        float radius = useStartRadius ? startRadius : (startRadius + e2.magnitude) * 0.5f;

        float a0 = Mathf.Atan2(s2.y, s2.x);
        float a1 = Mathf.Atan2(e2.y, e2.x); // end angle

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

    // Returns signed delta angle from a0 -> a1 following CW/CCW
    float DeltaAngleRadians(float a0, float a1, bool cw)
    {
        float d = a1 - a0;

        while (d > Mathf.PI) d -= 2f * Mathf.PI;
        while (d < -Mathf.PI) d += 2f * Mathf.PI;

        if (cw)
        {
            if (d > 0f) d -= 2f * Mathf.PI; // force negative
        }
        else
        {
            if (d < 0f) d += 2f * Mathf.PI; // force positive
        }

        return d;
    }
}
