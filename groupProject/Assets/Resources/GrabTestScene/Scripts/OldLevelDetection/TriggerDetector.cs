using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    //Tag to be compared
    [SerializeField] string tagField;

    private void OnTriggerEnter(Collider other)
    {
        //get score only if level of object is the correct one
        if (other.gameObject.CompareTag(tagField))
        {
            Debug.Log(tagField + " object inserted!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if object falls out of level, then lose points anyway
        Debug.Log("Object fell!");
    }

}
