using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DarknessMechanicScript : MonoBehaviour
{
    [Header("timer")]

    [Range(0, 14)] public float countDown;

    private bool isSafe;

    
    void Start()
    {
        isSafe = false;
        countDown = 0f;
        StartCoroutine(DarknessTimer());
    }

    void Update()
    {
       
    }

    IEnumerator DarknessTimer()
    {
        while ((countDown < 14f) && (!isSafe))
        {
            countDown += Time.deltaTime;            
            yield return null;
        }

        if (countDown >= 14)
        {
            Debug.Log("not funny bro");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LightObject")
        {
            isSafe = true;
            countDown = 0f;
            StopCoroutine(DarknessTimer());
            Debug.Log("funny");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LightObject")
        {
            isSafe = false;
            StartCoroutine(DarknessTimer());
            Debug.Log("really not funny");
        }
    }
}
