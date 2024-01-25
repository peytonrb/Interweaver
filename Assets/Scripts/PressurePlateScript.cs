using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    /*
    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.CompareTag("Player")) {
            Debug.Log("Yes");
        }
       
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Yes");
        }
    }
}
