using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HeightDetector : MonoBehaviour
{
    public float height { get; private set; }
    public bool IsSettled => isSettled;

    private XRGrabInteractable grabbableItem;
    private Rigidbody rb;

    private bool isGrabbed = false;
    private bool isSettled = false;

    private float stillTimer = 0f;

    // velocity threshold to detect movement
    private const float velocityThreshold = 0.05f;
    // must be still this long
    private const float settleTimeRequired = 0.5f; 

    private void Awake()
    {
        grabbableItem = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabbableItem.selectEntered.AddListener(_ =>
        {
            isGrabbed = true;
            isSettled = false;
            stillTimer = 0f;
        });

        grabbableItem.selectExited.AddListener(_ =>
        {
            isGrabbed = false;
            stillTimer = 0f; // reset timer when released
        });
    }

    private void Update()
    {
        if (isGrabbed)
        {
            isSettled = false;
            stillTimer = 0f;
            return;
        }

        if (rb.linearVelocity.magnitude < velocityThreshold)
        {
            // add time that object is still
            stillTimer += Time.deltaTime;
            // if still time is larger than threshold
            if (stillTimer >= settleTimeRequired)
            {
                isSettled = true;
                height = GetComponentInChildren<Renderer>().bounds.max.y;
            }
        }
        else
        {
            // reset if it moves again
            stillTimer = 0f;
            isSettled = false;
        }
    }

}
