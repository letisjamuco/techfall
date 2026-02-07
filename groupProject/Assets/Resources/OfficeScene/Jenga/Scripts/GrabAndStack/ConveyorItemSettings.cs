using UnityEngine;

// Per-item vertical offset so objects sit correctly on the conveyor belt, independent of world scale or pivot differences.
[DisallowMultipleComponent]
public class ConveyorItemSettings : MonoBehaviour
{
    [Tooltip("Vertical offset applied on top of the conveyor path.")]
    public float yOffset = 0f;

    [Tooltip("If true, compute yOffset automatically from local mesh bounds on spawn.")]
    public bool autoFromBounds = true;

    // Computes a stable vertical offset based on local mesh bounds. This avoids issues caused by non-uniform scaling or world-space bounds.
    public void AutoCompute()
    {
        if (!autoFromBounds) return;

        // Try MeshFilter first (most static models)
        if (TryComputeFromMeshFilter(out float computed))
        {
            yOffset = Mathf.Max(yOffset, computed);
            return;
        }

        // Fallback: Skinned meshes
        if (TryComputeFromSkinnedRenderer(out computed))
        {
            yOffset = Mathf.Max(yOffset, computed);
            return;
        }

        // Last fallback: Renderer (less precise, but safe)
        var r = GetComponentInChildren<Renderer>();
        if (!r) return;

        // Convert world-space bounds height to local Y
        float worldHalfHeight = r.bounds.extents.y;
        float scaleY = transform.lossyScale.y;

        if (scaleY > 0.0001f)
            yOffset = Mathf.Max(yOffset, worldHalfHeight / scaleY);
    }

    bool TryComputeFromMeshFilter(out float halfHeight)
    {
        halfHeight = 0f;

        var mf = GetComponentInChildren<MeshFilter>();
        if (!mf || !mf.sharedMesh) return false;

        // Local-space bounds (independent of scale)
        Bounds b = mf.sharedMesh.bounds;
        halfHeight = b.extents.y;

        return true;
    }

    bool TryComputeFromSkinnedRenderer(out float halfHeight)
    {
        halfHeight = 0f;

        var skinned = GetComponentInChildren<SkinnedMeshRenderer>();
        if (!skinned || !skinned.sharedMesh) return false;

        Bounds b = skinned.sharedMesh.bounds;
        halfHeight = b.extents.y;

        return true;
    }
}