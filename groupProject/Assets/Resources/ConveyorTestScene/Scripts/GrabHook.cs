using UnityEngine;

public class GrabHook : MonoBehaviour
{
	BeltPathMover _mover;

	void Awake()
	{
		_mover = GetComponent<BeltPathMover>();
		if (!_mover) _mover = GetComponentInParent<BeltPathMover>();
	}

	// XR Interaction Toolkit calls this automatically if you wire it in events
	public void OnGrabbed()
	{
		_mover?.MarkGrabbed();
	}
}
