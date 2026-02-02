using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackHeightCalculator : MonoBehaviour
{
    public static StackHeightCalculator instance;

    //list of stacked objects
    List<GameObject> objectsInStack = new List<GameObject>();
    //maxHeight of stack
    public float maxHeight { get; private set; }

    

    private void Awake()
    {
        instance = this;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("JengaPiece"))
        {
            //height calculation script for each piece
            //update score manager
            if (!objectsInStack.Contains(other.gameObject))
            {
                objectsInStack.Add(other.gameObject);
                Debug.Log("List ");
                foreach (GameObject obj in objectsInStack)
                {
                    Debug.Log(obj.gameObject.name);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("JengaPiece"))
        {
            //remove points and update score manager
            objectsInStack.Remove(other.gameObject);
            foreach (GameObject obj in objectsInStack)
            {
                Debug.Log(obj.gameObject.name);
            }
        }
    }

    public void UpdateMaxHeight()
    {
        maxHeight = 0;

        foreach (GameObject gameObject in objectsInStack)
        {
            if (gameObject.GetComponent<HeightDetector>().height >= maxHeight)
            {
                maxHeight = gameObject.GetComponent<HeightDetector>().height;
            }
        }
    }
}
