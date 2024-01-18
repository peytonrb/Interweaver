using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DarknessMechanicScript : MonoBehaviour
{
    [Header("Timer")]
    [Range(0, 14)] private float countDown;
    public float deathTime = 5f;
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
        while ((countDown < deathTime) && (!isSafe))
        {
            countDown += Time.deltaTime;
            yield return null;
        }

        if (countDown >= deathTime)
        {
            Debug.Log("not funny bro");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LightObject")
        {
            Debug.Log(other.gameObject);
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
