using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    private GameObject impactPSParent;
    private GameObject flareFX;
    private GameObject activeWyvern;

    void Start()
    {
        impactPSParent = this.transform.Find("FireballImpactVFX").gameObject;
        flareFX = this.transform.Find("FlarePS").gameObject;
    }

    void Update()
    {
        if (activeWyvern == null)
        {
            activeWyvern = GameObject.Find("WyvernBoss"); // only one should be in the scene at a time
        }
        
        flareFX.transform.rotation = Quaternion.LookRotation(flareFX.transform.position - activeWyvern.transform.position);
        Vector3 point = 1f * Vector3.Normalize(activeWyvern.transform.position - transform.position) + transform.position;
        flareFX.transform.position = point;
    }

    private void OnCollisionEnter(Collision collision)
    {
    
    }
}
