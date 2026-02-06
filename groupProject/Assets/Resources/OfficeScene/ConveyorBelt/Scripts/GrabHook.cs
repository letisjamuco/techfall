using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Bridges XR Grab events -> BeltPathMover.MarkGrabbed()
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class GrabHook : MonoBehaviour
{
    BeltPathMover _mover;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;

    void Awake()
    {
        _mover = GetComponent<BeltPathMover>();
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
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
