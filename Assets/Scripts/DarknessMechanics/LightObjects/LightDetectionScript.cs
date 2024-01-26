using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetectionScript : MonoBehaviour
{
    private LightCrystalScript crystalScript;
    private bool wasCrystalOn;

    void Start()
    {
        crystalScript = this.gameObject.transform.parent.GetComponent<LightCrystalScript>();

        if (crystalScript.isActive)
        {
            wasCrystalOn = true; // crystal was originally powered, so it cannot be unpowered
        }
    }

    public void OnTriggerEnter(Collider collision)
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
    }

    public void OnTriggerStay(Collider collider)
    {
        // again ensuring this is a powered object
        if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
        {
            // if for some reason the light that was powering the crystal is turned off
            if (!wasCrystalOn && !crystalScript.isFocusingCrystal && !LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                crystalScript.isActive = false;
            }

            float distance = Vector3.Distance(this.gameObject.transform.position, collider.gameObject.transform.position);

            if (LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn && distance < 4.5f)
            {
                crystalScript.isActive = true;
            }
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        // same thing as above but when powered crystal is leaving trigger
        if (collision.gameObject.tag == "Weaveable" && collision.GetComponent<LightCrystalScript>() != null)
        {
            // if crystal wasn't originally on and object in trigger was powering it, turn off once out of trigger
            if (!wasCrystalOn && LightSourceScript.Instance.lightsArray[collision.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, collision.gameObject.transform.position);

                if (distance > 4.5f) // 4.5f is based on calculation for dynamic collider in FocusingCrystalScript
                {
                    crystalScript.isActive = false;
                }
            }
        }
    }
}
