using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetectionScript : MonoBehaviour
{
    public bool isTimedMushroom;
    private LightCrystalScript crystalScript;
    private bool wasCrystalOn = false;

     [Header("Audio")]
    [SerializeField] private AudioClip  sensorHitClip;
    //[SerializeField] private AudioClip  crystalOnClip;

    void Start()
    {
        if (!isTimedMushroom)
        {
            if (gameObject.transform.parent.TryGetComponent<LightCrystalScript>(out crystalScript))
            {

            }
            else
            {
                crystalScript = this.gameObject.transform.parent.transform.parent.GetComponent<LightCrystalScript>();
            }
            

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
              //  AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, crystalOnClip);
                // .. and that light source is on and therefore powered
                if (LightSourceScript.Instance.lightsArray[collision.GetComponent<LightCrystalScript>().arrayIndex].isOn)
                {
                    
                    crystalScript.isActive = true; // turn this light on
                }
               
            }

            // if collision is with a sensor
            
            if (collision.GetComponent<SensorController>() != null)
            {
                AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, sensorHitClip);
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
            LightCrystalScript lightScript = collider.GetComponent<LightCrystalScript>(); // efficiency xoxo

            // again ensuring this is a powered object
            if (collider.gameObject.tag == "Weaveable" && lightScript != null)
            {
                // if for some reason the light that was powering the crystal is turned off
                if (!wasCrystalOn && !crystalScript.isFocusingCrystal &&
                    !LightSourceScript.Instance.lightsArray[lightScript.arrayIndex].isOn)
                {
                    crystalScript.isActive = false;
                }

                float distance = Vector3.Distance(this.gameObject.transform.position, collider.gameObject.transform.position);
                //Debug.Log(collider.name + "I got this far" + distance);

                if (LightSourceScript.Instance.lightsArray[lightScript.arrayIndex].isOn
                    && distance < 4.5f)
                {
                    crystalScript.isActive = true;
                }
            }

            SensorController sensorScript = collider.GetComponent<SensorController>();
            // if collision is with a sensor
            if (sensorScript != null)
            {
                if (LightSourceScript.Instance.lightsArray[transform.parent.transform.parent.GetComponent<LightCrystalScript>().arrayIndex].isOn &&
                    !sensorScript.isActive)
                {
                    sensorScript.isActive = true;

                    if (sensorScript.sensorEvent != null)
                        sensorScript.StartEvent();
                }
            }
        }
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
            SensorController sensorScript = collision.GetComponent<SensorController>();
            if (sensorScript != null)
            {
                if (sensorScript.isActive)
                {
                    sensorScript.isActive = false;
                    sensorScript.RemoveVFX();
                }
            }
        }
    }
}
