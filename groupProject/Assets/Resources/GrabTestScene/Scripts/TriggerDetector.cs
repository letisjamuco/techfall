using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Level0"))
        {
            Debug.Log("Level0 collision happened with object " + other.gameObject.name);
        }
        else if (other.gameObject.CompareTag("Level1"))
        {
            Debug.Log("Level1 collision happened with object " + other.gameObject.name);
        }
        else if (other.gameObject.CompareTag("Level2"))
        {
            Debug.Log("Level2 collision happened with object " + other.gameObject.name);
        }
    }

}
