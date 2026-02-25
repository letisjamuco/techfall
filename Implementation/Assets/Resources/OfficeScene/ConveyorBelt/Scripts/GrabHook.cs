using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRGrabInteractable))]
public class GrabHook : MonoBehaviour
{
    BeltPathMover _mover;
    XRGrabInteractable _grab;

    void Awake()
    {
        _mover = GetComponent<BeltPathMover>();
        _grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (_grab) _grab.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        if (_grab) _grab.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (!_mover) _mover = GetComponent<BeltPathMover>();
        _mover?.MarkGrabbed();
    }
}
