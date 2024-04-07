using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFlamethrower : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerControllerNew playerController = other.gameObject.GetComponent<PlayerControllerNew>();
            playerController.Death();
        }
        if (other.gameObject.CompareTag("Familiar")) {
            FamiliarScript familiarScript = other.gameObject.GetComponent<FamiliarScript>();
            familiarScript.Death();
        }
    }
}
