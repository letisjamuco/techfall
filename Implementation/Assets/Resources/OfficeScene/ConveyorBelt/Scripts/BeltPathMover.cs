using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public sealed class BeltPathMover : MonoBehaviour
{
    [Tooltip("True after the player has grabbed the item at least once.")]
    public bool wasGrabbed;

    CinemachinePathBase _path;
    float _travelTimeSeconds;
    float _elapsedSeconds;
    TechfallConveyorController _controller;

    public void Init(CinemachinePathBase path, float travelTimeSeconds, TechfallConveyorController controller)
    {
        _path = path;
        _travelTimeSeconds = Mathf.Max(0.1f, travelTimeSeconds);
        _controller = controller;
        _elapsedSeconds = 0f;

        ApplyPoseAt(0f);
    }

    void Update()
    {
        if (!_path) return;
        if (wasGrabbed) return;

        _elapsedSeconds += Time.deltaTime;
        float t01 = Mathf.Clamp01(_elapsedSeconds / _travelTimeSeconds);

        if (t01 >= 1f)
        {
            _controller?.RegisterMiss(gameObject);
            Destroy(gameObject);
            return;
        }

        ApplyPoseAt(t01);
    }

    void ApplyPoseAt(float t01)
    {
        Vector3 p = _path.EvaluatePositionAtUnit(t01, CinemachinePathBase.PositionUnits.Normalized);
        Vector3 tangent = _path.EvaluateTangentAtUnit(t01, CinemachinePathBase.PositionUnits.Normalized);

        transform.position = p;

        if (tangent.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(tangent.normalized, Vector3.up);
    }

    public void MarkGrabbed()
    {
        if (wasGrabbed) return;
        wasGrabbed = true;
        _controller?.RegisterGrabbed(gameObject);
    }
}
