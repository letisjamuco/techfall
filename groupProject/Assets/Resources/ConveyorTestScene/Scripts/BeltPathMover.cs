using UnityEngine;

public class BeltPathMover : MonoBehaviour
{
	public bool wasGrabbed = false;

	Transform[] _path;
	float _travelTime;
	float _elapsed;
	TechfallConveyorController _controller;

	public void Init(Transform[] path, float travelTimeSeconds, TechfallConveyorController controller)
	{
		_path = path;
		_travelTime = Mathf.Max(0.1f, travelTimeSeconds);
		_controller = controller;

		_elapsed = 0f;
		transform.position = _path[0].position;
	}

	void Update()
	{
		if (_path == null || _path.Length < 2) return;
		if (wasGrabbed) return; // once grabbed, stop moving it

		_elapsed += Time.deltaTime;

		// normalized progress 0..1 across full path time
		float t = Mathf.Clamp01(_elapsed / _travelTime);

		// move along segments
		float scaled = t * (_path.Length - 1);
		int seg = Mathf.Min(_path.Length - 2, Mathf.FloorToInt(scaled));
		float localT = scaled - seg;

		Vector3 a = _path[seg].position;
		Vector3 b = _path[seg + 1].position;
		transform.position = Vector3.Lerp(a, b, localT);

		// Optional: face direction
		Vector3 dir = (b - a);
		if (dir.sqrMagnitude > 0.0001f)
			transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
	}

	// Call this from grab event (XR/OVR) or manually
	public void MarkGrabbed()
	{
		wasGrabbed = true;
		_controller?.RegisterGrabbed(gameObject);
	}
}
