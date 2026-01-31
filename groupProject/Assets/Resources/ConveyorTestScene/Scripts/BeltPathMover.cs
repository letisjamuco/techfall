using UnityEngine;

public class BeltPathMover : MonoBehaviour
{
    // Set true by grab system (so belt stops controlling this object)
    public bool wasGrabbed = false;

    // Used when we pause the belt (stop motion without destroying)
    public bool paused = false;

    Transform[] _wps;
    float _travelTime;
    float _elapsed;
    TechfallConveyorController _controller;

    // Called by TechfallConveyorController right after spawn
    public void Init(Transform[] waypoints, float travelTimeSeconds, TechfallConveyorController controller)
    {
        _wps = waypoints;
        _travelTime = Mathf.Max(0.1f, travelTimeSeconds);
        _controller = controller;
        _elapsed = 0f;

        transform.position = _wps[0].position;
        transform.rotation = _wps[0].rotation;
    }

    void Update()
    {
        if (_wps == null || _wps.Length < 2) return;
        if (wasGrabbed || paused) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / _travelTime);

        // Move along waypoint segments
        float scaled = t * (_wps.Length - 1);
        int seg = Mathf.Min(_wps.Length - 2, Mathf.FloorToInt(scaled));
        float localT = scaled - seg;

        Vector3 a = _wps[seg].position;
        Vector3 b = _wps[seg + 1].position;

        transform.position = Vector3.Lerp(a, b, localT);

        // Face forward along the path (optional, helps realism)
        Vector3 dir = (b - a);
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    // Call this from the grab script when the user grabs the object
    public void MarkGrabbed()
    {
        wasGrabbed = true;
        _controller?.RegisterGrabbed(gameObject);
    }
}
