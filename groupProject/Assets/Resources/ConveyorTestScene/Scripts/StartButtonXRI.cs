using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class StartButtonXRI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public TechfallConveyorController conveyor;

    [Header("Safety")]
    public bool oneShot = true;

    bool _started;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    void OnEnable()
    {
        _interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (oneShot && _started) return;
        _started = true;

        conveyor?.StartBelt();
    }
}
