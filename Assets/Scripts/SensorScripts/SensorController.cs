using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorController : MonoBehaviour
{
    public UnityEvent sensorEvent;
    public bool isActive;
    private GameObject particleSystemParent;

    void Start()
    {
        particleSystemParent = this.transform.GetChild(0).gameObject;
    }

    public void StartEvent()
    {
        // start all particle systems
        foreach (ParticleSystem ps in particleSystemParent.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }

        // invoke event
        sensorEvent.Invoke();
    }
}
