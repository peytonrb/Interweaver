using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
    public Transform teleportExit;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = teleportExit.position;
        }
    }
}
