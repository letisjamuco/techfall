using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trump_Destinations : MonoBehaviour
{
    public int Pivotpoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trump")
        {
            if(Pivotpoint == 0)
            {
                this.gameObject.transform.position = new Vector3 (0.7f, 0, 8.7f);
                Pivotpoint++;
            }
        }
    }
}
