using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class ConveyorGrabPhysics : MonoBehaviour
{
    [Header("Gravity behavior")]
    [Tooltip("Gravity state while the item is on the belt (before grab).")]
    public bool gravityWhileOnBelt = false;

    [Tooltip("Gravity state after the player releases the item.")]
    public bool gravityAfterRelease = true;

    [Header("Damping (to avoid 'floating in water')")]
    [Tooltip("Drag while on belt (low = less 'water').")]
    public float dragOnBelt = 0.05f;

    [Tooltip("Drag while held (usually low).")]
    public float dragWhileHeld = 0.0f;

    [Tooltip("Drag after release (low, so it falls naturally).")]
    public float dragAfterRelease = 0.05f;

    [Tooltip("Angular damping after release.")]
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
        // Subscribe to grab events
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
        // Initial state: on belt
        ApplyOnBeltState();
    }

    void ApplyOnBeltState()
    {
        _rb.useGravity = gravityWhileOnBelt;
        _rb.linearDamping = dragOnBelt;
        // keep current angular damping
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // When grabbed: keep stable
        _rb.useGravity = false;           // prevent weird falling while held
        _rb.linearDamping = dragWhileHeld;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // After release: let it fall naturally
        _rb.useGravity = gravityAfterRelease;
        _rb.linearDamping = dragAfterRelease;
        _rb.angularDamping = angularDampingAfterRelease;
    }
}
