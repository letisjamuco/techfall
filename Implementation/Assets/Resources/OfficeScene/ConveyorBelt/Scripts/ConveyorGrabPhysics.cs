using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class ConveyorGrabPhysics : MonoBehaviour
{
    [Header("Gravity")]
    [Tooltip("Gravity state while the item is on the belt (before it is grabbed).")]
    public bool gravityWhileOnBelt = false;

    [Tooltip("Gravity state after the player releases the item.")]
    public bool gravityAfterRelease = true;

    [Header("Damping")]
    [Tooltip("Linear damping while the item is on the belt.")]
    public float dragOnBelt = 0.05f;

    [Tooltip("Linear damping while the item is held.")]
    public float dragWhileHeld = 0.0f;

    [Tooltip("Linear damping after the item is released.")]
    public float dragAfterRelease = 0.05f;

    [Tooltip("Angular damping after the item is released.")]
    public float angularDampingAfterRelease = 0.1f;

    Rigidbody _rb;
    XRGrabInteractable _grab;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        _grab.selectEntered.AddListener(OnGrab);
        _grab.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        _grab.selectEntered.RemoveListener(OnGrab);
        _grab.selectExited.RemoveListener(OnRelease);
    }

    void Start()
    {
        ApplyOnBeltState();
    }

    void ApplyOnBeltState()
    {
        _rb.useGravity = gravityWhileOnBelt;
        _rb.linearDamping = dragOnBelt;
        // Keep existing angular damping while on belt; it is usually not critical here.
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Disable gravity while held to avoid unintended falling/oscillation during hand movement.
        _rb.useGravity = false;
        _rb.linearDamping = dragWhileHeld;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        _rb.useGravity = gravityAfterRelease;
        _rb.linearDamping = dragAfterRelease;
        _rb.angularDamping = angularDampingAfterRelease;
    }
}
