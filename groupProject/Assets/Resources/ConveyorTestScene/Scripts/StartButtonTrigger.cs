using UnityEngine;

public class StartButtonTrigger : MonoBehaviour
{
    public TechfallConveyorController conveyor;

    // Triggered when the user's hand touches the button volume
    void OnTriggerEnter(Collider other)
    {
        // Simple: any collider from XR hands can start it
        conveyor?.StartBelt();
    }
}
