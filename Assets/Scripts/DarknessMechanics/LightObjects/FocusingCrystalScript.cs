using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FocusingCrystalScript : MonoBehaviour
{
    [HideInInspector] public bool isActive;
    private VisualEffect beamEffect;
    private CapsuleCollider lightCollider;
    private GameObject beamHitObj;
    private Light spotLight;

    void Start()
    {
        beamEffect = this.gameObject.GetComponent<VisualEffect>();
        lightCollider = this.gameObject.GetComponent<CapsuleCollider>();
        spotLight = this.transform.GetChild(0).GetComponent<Light>();
        spotLight.range = 0f;
    }

    void Update()
    {
        if (this.gameObject.transform.parent.GetComponent<LightCrystalScript>().isActive && !isActive)
        {
            isActive = true;
        }
        else if (!this.gameObject.transform.parent.GetComponent<LightCrystalScript>().isActive && isActive)
        {
            isActive = false;
        }

        // changes collider size
        if (beamEffect.GetBool("Hit") && isActive) // if beam is hitting something
        {
            RaycastHit hit;
            Vector3 spawnPoint = this.gameObject.transform.position;

            // calculates hit position & object
            if (Physics.Raycast(spawnPoint, transform.TransformDirection(Vector3.forward), out hit, 100000f))
            {
                //Debug.DrawRay(this.gameObject.transform.position, transform.TransformDirection(Vector3.forward) * 100, 
                //              Color.red);
                Vector3 hitPosition = hit.point;
                beamHitObj = hit.collider.gameObject;
                float distance = Vector3.Distance(spawnPoint, hitPosition);
                spotLight.range = distance + 0.5f;
                distance /= 1.35f; // man idk. it works tho.

                if (distance > 1f)
                {
                    Vector3 center = lightCollider.center;
                    center.z = distance;
                    lightCollider.center = center;
                    lightCollider.height = (distance * 2) + 8f;
                }
            }
        }
        else
        {
            Vector3 center = lightCollider.center;
            center.z = 0;
            lightCollider.center = center;
            lightCollider.height = 4f;
            spotLight.range = 0f;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        // if beam hits crystal
        if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
        {
            if (!LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                // is beam actually hitting crystal
                if (beamHitObj.GetComponent<LightCrystalScript>() != null)
                {
                    collider.GetComponent<LightCrystalScript>().isActive = true;
                }
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
        {
            if (LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                collider.GetComponent<LightCrystalScript>().isActive = true;
            }
        }

        if (collider.GetComponent<LightCrystalScript>() && collider.GetComponent<LightCrystalScript>().isFocusingCrystal 
            && !collider.GetComponent<LightCrystalScript>().isActive)
        {
            collider.GetComponent<LightCrystalScript>().isActive = true;
        }

        // for sensors if beam is moved by rotating or moving the focusing crystal itself
        if (collider.GetComponent<SensorController>() != null && beamHitObj != null && 
            beamHitObj.GetComponent<SensorController>() == null && collider.GetComponent<SensorController>().isActive)
        {
            collider.GetComponent<SensorController>().isActive = false;
        }

        // if beam hits sensor
        if (collider.GetComponent<SensorController>() != null && beamHitObj != null &&
            beamHitObj.GetComponent<SensorController>() != null && !collider.GetComponent<SensorController>().isActive)
        {
            collider.GetComponent<SensorController>().isActive = true;
        }

        // if beam hits player
        if (collider.gameObject.tag == "Player" && isActive)
        {
            collider.GetComponent<DarknessMechanicScript>().isSafe = true;
        }
        else if (collider.gameObject.tag == "Player" && !isActive)
        {
            collider.GetComponent<DarknessMechanicScript>().isSafe = false;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
        {
            if (!collider.GetComponent<LightCrystalScript>().isActiveDefault 
                && LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                Debug.Log("here");
                collider.GetComponent<LightCrystalScript>().isActive = false;
            }
        }

        if (collider.GetComponent<SensorController>() != null && 
            beamHitObj != null && beamHitObj.GetComponent<SensorController>() != null 
            && collider.GetComponent<SensorController>().isActive)
        {
            collider.GetComponent<SensorController>().isActive = false;
        }
    }
}
