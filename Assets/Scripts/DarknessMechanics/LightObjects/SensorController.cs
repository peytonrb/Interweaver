using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorController : MonoBehaviour
{
    public UnityEvent sensorEvent;

    [Header("FOR TESTING")]
    public bool isActive;

    public void StartEvent()
    {
        sensorEvent.Invoke();
    }
}
