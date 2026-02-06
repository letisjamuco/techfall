using UnityEngine;

public class WallEaterTrigger : MonoBehaviour
{
    // Reference to the main conveyor controller (set in Inspector)
    public TechfallConveyorController controller;

    void OnTriggerEnter(Collider other)
    {
        // We search on parent because colliders might be on child meshes
        var mover = other.GetComponentInParent<BeltPathMover>();
        if (!mover) return;

        // If not grabbed, it's a "miss" -> reprimand/strike
        if (!mover.wasGrabbed)
        {
            controller?.RegisterMiss(mover.gameObject);
            Destroy(mover.gameObject);
        }
    }
}
