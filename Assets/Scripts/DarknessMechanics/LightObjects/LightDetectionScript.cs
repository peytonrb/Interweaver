using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetectionScript : MonoBehaviour
{
    public bool isTimedMushroom;
    private LightCrystalScript crystalScript;
    private bool wasCrystalOn = false;

    void Start()
    {
        if (!isTimedMushroom)
        {
            crystalScript = this.gameObject.transform.parent.transform.parent.GetComponent<LightCrystalScript>();

            if (crystalScript.isActive)
            {
                wasCrystalOn = true; // crystal was originally powered, so it cannot be unpowered
            }

            // ensures the volumetric lighting matches the collider volume
            Light lightComponent = this.GetComponent<Light>();

            if (this.TryGetComponent<SphereCollider>(out SphereCollider collider))
            {
                float radius = collider.radius;
                lightComponent.range = radius;
            }
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (!isTimedMushroom)
        {
            // if the collision is with another light crystal
            if (collision.gameObject.tag == "Weaveable" && collision.GetComponent<LightCrystalScript>() != null)
            {
                // .. and that light source is on and therefore powered
                if (LightSourceScript.Instance.lightsArray[collision.GetComponent<LightCrystalScript>().arrayIndex].isOn)
                {
                    crystalScript.isActive = true; // turn this light on
                }
            }

            // if collision is with a sensor
            if (collision.GetComponent<SensorController>() != null)
            {
                if (LightSourceScript.Instance.lightsArray[transform.parent.transform.parent.GetComponent<LightCrystalScript>().arrayIndex].isOn &&
                    !collision.GetComponent<SensorController>().isActive)
                {
                    collision.GetComponent<SensorController>().isActive = true;

                    if (collision.GetComponent<SensorController>().sensorEvent != null)
                        collision.GetComponent<SensorController>().StartEvent();
                }
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (!isTimedMushroom)
        {
            // again ensuring this is a powered object
            if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
            {
                // if for some reason the light that was powering the crystal is turned off
                if (!wasCrystalOn && !crystalScript.isFocusingCrystal &&
                    !LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
                {
                    crystalScript.isActive = false;
                }

                float distance = Vector3.Distance(this.gameObject.transform.position, collider.gameObject.transform.position);

                if (LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn
                    && distance < 4.5f)
                {
                    crystalScript.isActive = true;
                }
            }

            // if collision is with a sensor
            if (collider.GetComponent<SensorController>() != null)
            {
                if (LightSourceScript.Instance.lightsArray[transform.parent.transform.parent.GetComponent<LightCrystalScript>().arrayIndex].isOn &&
                    !collider.GetComponent<SensorController>().isActive)
                {
                    collider.GetComponent<SensorController>().isActive = true;

                    if (collider.GetComponent<SensorController>().sensorEvent != null)
                        collider.GetComponent<SensorController>().StartEvent();
                }
            }
        }

        // if light hits player
        //if (collider.gameObject.tag == "Player")
        //{
        //    if ((isTimedMushroom && this.gameObject.transform.parent.GetComponent<TimedGlowMushroomsScript>().isActive) 
        //         || !isTimedMushroom)
        //    {
        //        collider.GetComponent<DarknessMechanicScript>().isSafe = true;
        //    }
        //    else if (isTimedMushroom && !this.gameObject.transform.parent.GetComponent<TimedGlowMushroomsScript>().isActive)
        //    {
        //        collider.GetComponent<DarknessMechanicScript>().isSafe = false;
        //    }
        //}
    }

    public void OnTriggerExit(Collider collision)
    {
        if (!isTimedMushroom)
        {
            // same thing as above but when powered crystal is leaving trigger
            if (collision.gameObject.tag == "Weaveable" && collision.GetComponent<LightCrystalScript>() != null)
            {
                // if crystal wasn't originally on and object in trigger was powering it, turn off once out of trigger
                if (!wasCrystalOn &&
                    LightSourceScript.Instance.lightsArray[collision.GetComponent<LightCrystalScript>().arrayIndex].isOn)
                {
                    crystalScript.isActive = false;
                }
            }

            // if collision is with a sensor
            if (collision.GetComponent<SensorController>() != null)
            {
                if (collision.GetComponent<SensorController>().isActive)
                {
                    collision.GetComponent<SensorController>().isActive = false;
                }
            }
        }
    }
}
