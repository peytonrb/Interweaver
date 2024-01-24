using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
     void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log(other.gameObject.name);
        }
        
     }
}
