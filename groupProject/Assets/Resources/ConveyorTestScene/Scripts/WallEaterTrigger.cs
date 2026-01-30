using UnityEngine;

public class WallEaterTrigger : MonoBehaviour
{
    public TechfallConveyorController controller;

    void OnTriggerEnter(Collider other)
    {
        var mover = other.GetComponentInParent<BeltPathMover>();
        if (mover == null) return;

        // If not grabbed => miss
        if (!mover.wasGrabbed)
        {
            controller?.RegisterMiss(mover.gameObject);
            Destroy(mover.gameObject);
        }
    }
}
