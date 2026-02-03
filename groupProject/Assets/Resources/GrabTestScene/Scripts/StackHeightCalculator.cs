using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackHeightCalculator : MonoBehaviour
{
    public static StackHeightCalculator instance;

    //set of stacked objects
    HashSet<GameObject> objectsInStack = new HashSet<GameObject>();
    //maxHeight of stack
    public float maxHeight { get; private set; }

    

    private void Awake()
    {
        instance = this;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("JengaPiece"))
        {
            Rigidbody rigidBody = other.GetComponentInParent<Rigidbody>();
            if (rigidBody != null)
            {
                // add to set only if it stopped moving
                if (rigidBody.linearVelocity.magnitude < 0.05f)
                {
                    objectsInStack.Add(other.gameObject);
                }
            }
        }
    }

/*    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("JengaPiece"))
        {
            height calculation script for each piece
            update score manager
            if (!objectsInStack.Contains(other.gameObject))
            {
                objectsInStack.Add(other.gameObject);
            }
        }
    }*/

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("JengaPiece"))
        {
            //remove points and update score manager
            objectsInStack.Remove(other.gameObject);
        }
    }

    public void UpdateMaxHeight()
    {
        maxHeight = 0;

        foreach (GameObject gameObject in objectsInStack)
        {
            if (gameObject.GetComponentInParent<HeightDetector>().height >= maxHeight)
            {
                maxHeight = gameObject.GetComponentInParent<HeightDetector>().height;
            }
        }
    }
}
