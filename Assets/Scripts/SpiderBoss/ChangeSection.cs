using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeSection : MonoBehaviour
{
    public UnityEvent pressEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar"))
        {
            Activation();
        }
    }
    void Activation()
    {
        pressEvent.Invoke();
    }
}
