using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            transform.parent.GetComponent<Robot_transport>()._trigger = 
                !transform.parent.GetComponent<Robot_transport>()._trigger;
        }
    }
}
