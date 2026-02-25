using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRSimpleInteractable))]
public class StartButtonXRI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Conveyor controller started by this button.")]
    public TechfallConveyorController conveyor;

    [Header("Behavior")]
    [Tooltip("If true, the button can only start the conveyor once.")]
    public bool oneShot = true;

    [Header("Testing")]
    [Tooltip("If false, interaction is disabled.")]
    public bool buttonEnabled = true;

    [Tooltip("If true, the button disables itself automatically when the conveyor is configured to auto-start.")]
    public bool syncWithConveyorAutoStart = true;

    bool _started;
    XRSimpleInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        ApplyEnabledState();
    }

    void OnValidate()
    {
        if (syncWithConveyorAutoStart && conveyor != null)
            buttonEnabled = !conveyor.autoStartWithoutButton;

        ApplyEnabledState();
    }

    void OnEnable()
    {
        if (syncWithConveyorAutoStart && conveyor != null)
            buttonEnabled = !conveyor.autoStartWithoutButton;

        ApplyEnabledState();

        if (_interactable)
            _interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        if (_interactable)
            _interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void ApplyEnabledState()
    {
        if (!_interactable) _interactable = GetComponent<XRSimpleInteractable>();
        if (_interactable) _interactable.enabled = buttonEnabled;
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!buttonEnabled) return;
        if (oneShot && _started) return;

        _started = true;
        conveyor?.StartBelt();
    }
}
