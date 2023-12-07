using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessIndication : MonoBehaviour
{
    public GameObject indicatorToEnable;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Familiar"))
        {
            indicatorToEnable.SetActive(true);
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Familiar"))
        {
            indicatorToEnable.SetActive(false);
        }
    }


}
