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

    bool _started;
    XRSimpleInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
    }

    void OnEnable()
    {
        if (_interactable) _interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        if (_interactable) _interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (oneShot && _started) return;
        _started = true;

        conveyor?.StartBelt();
    }
}
