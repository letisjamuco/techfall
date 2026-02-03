using UnityEngine;

public class BeltPathMover : MonoBehaviour
{
    public bool wasGrabbed = false;
    public bool paused = false;

    Transform[] _wps;
    float _travelTime;
    float _elapsed;
    Transform _pathCenter;
    TechfallConveyorController _controller;
    ConveyorItemSettings _settings;

    public void Init(Transform[] waypoints, float travelTimeSeconds, TechfallConveyorController controller, Transform pathCenter)
    {
        _wps = waypoints;
        _travelTime = Mathf.Max(0.1f, travelTimeSeconds);
        _controller = controller;
        _pathCenter = pathCenter;
        _elapsed = 0f;

        _settings = GetComponent<ConveyorItemSettings>();
        if (_settings) _settings.AutoCompute();

        ApplyPoseAt(0, 0f);
    }

    void Update()
    {
        if (_wps == null || _wps.Length < 2) return;
        if (wasGrabbed || paused) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / _travelTime);

        float scaled = t * (_wps.Length - 1);
        int seg = Mathf.Min(_wps.Length - 2, Mathf.FloorToInt(scaled));
        float localT = scaled - seg;

        ApplyPoseAt(seg, localT);
    }

    void ApplyPoseAt(int seg, float localT)
    {
        Vector3 a = _wps[seg].position;
        Vector3 b = _wps[seg + 1].position;

        Vector3 p = Vector3.Lerp(a, b, localT);

        // Apply per-item offsets (up + outward from center)
        float yOff = _settings ? _settings.yOffset : 0f;
        float rOff = _settings ? _settings.radialOffset : 0f;

        if (_pathCenter && rOff != 0f)
        {
            Vector3 outward = (p - _pathCenter.position);
            outward.y = 0f;
            if (outward.sqrMagnitude > 0.0001f)
                p += outward.normalized * rOff;
        }

        p += Vector3.up * yOff;
        transform.position = p;

        // Face along the path
        Vector3 dir = (b - a);
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    public void MarkGrabbed()
    {
        wasGrabbed = true;
        _controller?.RegisterGrabbed(gameObject);
    }
}
