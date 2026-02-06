using UnityEngine;

// Per-item offsets so big objects don't clip into the belt/wall.
public class ConveyorItemSettings : MonoBehaviour
{
    [Tooltip("Lift item above the belt surface.")]
    public float yOffset = 0.0f;

    [Tooltip("Push item outward from the circle center (helps large items).")]
    public float radialOffset = 0.0f;

    [Tooltip("If true, auto-compute offsets from renderer bounds once on spawn.")]
    public bool autoFromBounds = true;

    // Called by controller right after spawn (optional).
    public void AutoCompute()
    {
        if (!autoFromBounds) return;

        var r = GetComponentInChildren<Renderer>();
        if (!r) return;

        // Very simple heuristic:
        // yOffset = half height, radial = half of the largest horizontal size.
        var b = r.bounds;
        yOffset = Mathf.Max(yOffset, b.extents.y);

        float horiz = Mathf.Max(b.extents.x, b.extents.z);
        radialOffset = Mathf.Max(radialOffset, horiz * 0.5f);
    }
}
