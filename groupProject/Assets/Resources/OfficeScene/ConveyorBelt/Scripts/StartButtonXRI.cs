using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class StartButtonXRI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public TechfallConveyorController conveyor;

    [Header("Safety")]
    public bool oneShot = true;

    [Header("Testing / Control")]
    [Tooltip("Enable/disable the start button interaction. Default: enabled.")]
    public bool buttonEnabled = true;

    [Tooltip("If true, this button will auto-disable itself when the conveyor is set to auto-start without button.")]
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
        // Keeps the button state in sync while editing.
        if (syncWithConveyorAutoStart && conveyor != null)
            buttonEnabled = !conveyor.autoStartWithoutButton;

        ApplyEnabledState();
    }

    void ApplyEnabledState()
    {
        if (!_interactable) _interactable = GetComponent<XRSimpleInteractable>();
        if (_interactable) _interactable.enabled = buttonEnabled;
    }

    void OnEnable()
    {
        if (syncWithConveyorAutoStart && conveyor != null)
            buttonEnabled = !conveyor.autoStartWithoutButton;
        ApplyEnabledState();
        if (_interactable) _interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        if (_interactable) _interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!buttonEnabled) return;
        if (oneShot && _started) return;
        _started = true;

        conveyor?.StartBelt();
    }
}
