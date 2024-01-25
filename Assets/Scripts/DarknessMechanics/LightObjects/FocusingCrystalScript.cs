using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FocusingCrystalScript : MonoBehaviour
{
    [Header("FOR SCRIPTS ONLY")]
    public bool isActive;
    private VisualEffect beamEffect;
    private CapsuleCollider lightCollider;

    void Start()
    {
        beamEffect = this.gameObject.GetComponent<VisualEffect>();
        lightCollider = this.gameObject.GetComponent<CapsuleCollider>();
    }

    void Update() // needs to be refactored + optimized :)
    {
        // changes collider size
        if (beamEffect.GetBool("Hit") && isActive) // if beam is hitting something
        {
            RaycastHit hit;
            Vector3 spawnPoint = this.gameObject.transform.position + new Vector3(0f, 1.5f, 0f);

            // calculates hit position & object
            if (Physics.Raycast(spawnPoint, transform.TransformDirection(Vector3.forward), out hit, 1000f))
            {
                // Debug.DrawRay(this.gameObject.transform.position + new Vector3(0f, 1.5f, 0f), transform.TransformDirection(Vector3.forward) * 100, Color.red);
                Vector3 hitPosition = hit.collider.gameObject.transform.position;
                float distance = Vector3.Distance(spawnPoint, hitPosition);
                distance /= 2;

                if (distance > 1f)
                {
                    Vector3 center = lightCollider.center;
                    center.z = distance;
                    lightCollider.center = center;
                    lightCollider.height = (distance * 2) + 6f;
                }
            }
        }
        else
        {
            Vector3 center = lightCollider.center;
            center.z = 0;
            lightCollider.center = center;
            lightCollider.height = 7f;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        // if beam hits crystal
        if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
        {
            if (!LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                collider.GetComponent<LightCrystalScript>().isActive = true;
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Weaveable" && collider.GetComponent<LightCrystalScript>() != null)
        {
            if (!collider.GetComponent<LightCrystalScript>().isActiveDefault && LightSourceScript.Instance.lightsArray[collider.GetComponent<LightCrystalScript>().arrayIndex].isOn)
            {
                collider.GetComponent<LightCrystalScript>().isActive = false;
            }
        }
    }
}
