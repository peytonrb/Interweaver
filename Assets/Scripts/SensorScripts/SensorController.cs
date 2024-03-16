using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class SensorController : MonoBehaviour
{
    public UnityEvent sensorEvent;
    public bool isActive;
    private GameObject particleSystemParent;

    [Header("VFX")]
    public GameObject[] wires;
    public Material activeMat;
    public Material defaultMat;
    private VisualEffect wireVFX;

    void Start()
    {
        particleSystemParent = this.transform.GetChild(0).gameObject;
        wireVFX = this.transform.GetChild(1).GetComponent<VisualEffect>();
        wireVFX.gameObject.SetActive(false);
    }

    public void StartEvent()
    {
        // start all particle systems
        foreach (ParticleSystem ps in particleSystemParent.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }

        if (wires.Length > 0)
        {
            wireVFX.gameObject.SetActive(true);

            foreach (GameObject wire in wires)
            {
                wire.GetComponent<Renderer>().material = activeMat;
            }
        }

        // invoke event
        sensorEvent.Invoke();
    }

    public void RemoveVFX()
    {
        wireVFX.gameObject.SetActive(false);

        if (wires.Length > 0)
        {
            foreach (GameObject wire in wires)
            {
                wire.GetComponent<Renderer>().material = defaultMat;
            }
        }
    }
}
