using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public SensorController sensor;
    public PressurePlateScript pplate;
    public bool doesPlatformStop;
    private bool pplateTriggered = false;

    void Start()
    {
        this.GetComponent<ObjectMoverScript>().enabled = false;
    }

    void Update()
    {
        if (sensor != null && !sensor.isActive && doesPlatformStop)
        {
            StopPlatform();
        }
        else if (pplate != null && !pplate.standingOnPlate && doesPlatformStop)
        {
            pplateTriggered = false;
            StopPlatform();
        }
    }

    public void AdvancePlatform()
    {
        if (pplate != null && !pplateTriggered || sensor != null)
        {
            pplateTriggered = true;
            this.GetComponent<ObjectMoverScript>().enabled = true;
        }
    }

    private void StopPlatform()
    {
        this.GetComponent<ObjectMoverScript>().enabled = false;
    }
}