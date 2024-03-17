using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    private GameObject impactPSParent;

    void Start()
    {
        impactPSParent = this.transform.Find("FireballImpactVFX").gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
    
    }
}
