using UnityEngine;
using System.Collections.Generic;

public class StackHeightCalculator : MonoBehaviour
{
    public static StackHeightCalculator instance;

    //set of stacked objects
    private HashSet<HeightDetector> objectsInStack = new HashSet<HeightDetector>();

    //max height of stack
    public float maxHeight { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("JengaPiece")) return;

        // find height detector
        HeightDetector detector = other.GetComponentInParent<HeightDetector>();

        // add to stack only if object is settled
        if (detector != null && detector.IsSettled)
        {
            objectsInStack.Add(detector);
        }
        else if (detector != null)
        {
            // remove if it starts moving or is grabbed
            objectsInStack.Remove(detector);
        }
    }

    // remove if it exits trigger
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("JengaPiece")) return;

        HeightDetector detector = other.GetComponentInParent<HeightDetector>();
        if (detector != null)
        {
            objectsInStack.Remove(detector);
        }
    }

    private void Update()
    {
        UpdateMaxHeight();
    }

    //update max height
    public void UpdateMaxHeight()
    {
        maxHeight = 0;

        foreach (HeightDetector detector in objectsInStack)
        {
            if (detector.height > maxHeight)
            {
                maxHeight = detector.height;
            }
        }
    }

}
