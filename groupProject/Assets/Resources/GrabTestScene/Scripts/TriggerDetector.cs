using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    //Tag to be compared
    [SerializeField] string tagField;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagField))
        {
            Debug.Log(tagField + " collision happened");
        }
    }

}
