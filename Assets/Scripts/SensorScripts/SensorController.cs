using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorController : MonoBehaviour
{
    public UnityEvent sensorEvent;
    [HideInInspector] public bool isActive;

    public void StartEvent()
    {
        sensorEvent.Invoke();
    }
}
