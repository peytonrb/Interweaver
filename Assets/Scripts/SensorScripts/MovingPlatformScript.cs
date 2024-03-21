using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public SensorController sensor;
    public PressurePlateScript pplate;
    public bool doesPlatformStop;
    private bool pplateTriggered = false;
    public bool triggerStopsObject = false;
    private bool isStopped = false;
    private CarriageOpener carriageScript;

    void Start()
    {
        if (!triggerStopsObject)
        {
            this.GetComponent<ObjectMoverScript>().enabled = false;
        }

        carriageScript = this.GetComponent<CarriageOpener>();
    }

    void Update()
    {
        if (!triggerStopsObject)
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
        else
        {
            if (sensor != null && sensor.isActive && doesPlatformStop)
            {
                StopPlatform();
            }
            else if (pplate != null && pplate.standingOnPlate && doesPlatformStop)
            {
                pplateTriggered = false;
                StopPlatform();
            }
            else if (pplate != null && !pplate.standingOnPlate && isStopped)
            {
                AdvancePlatform();
            }
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
        isStopped = true;

        if (carriageScript != null)
        {
            carriageScript.OpenCarriage();
        }
    }
}