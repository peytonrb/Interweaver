using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FocusingCrystalScript : MonoBehaviour
{
    private VisualEffect beamEffect;
    private BoxCollider collider;
    private float beamLength;

    void Start()
    {
        beamEffect = this.gameObject.transform.GetChild(0).GetComponent<VisualEffect>();
        collider = this.gameObject.transform.GetChild(0).GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (beamEffect.GetBool("Hit")) // if beam is hitting something
        {
            RaycastHit hit;
            Vector3 spawnPoint = this.gameObject.transform.position + new Vector3(0f, 1.5f, 0f);

            // calculates hit position & object
            if (Physics.Raycast(spawnPoint, transform.TransformDirection(Vector3.forward), out hit, 100f))
            {
                // Debug.DrawRay(this.gameObject.transform.position + new Vector3(0f, 1.5f, 0f), transform.TransformDirection(Vector3.forward) * 100, Color.red);
                Vector3 hitPosition = hit.collider.gameObject.transform.position; // calculation is right
            }
        }
    }
}
