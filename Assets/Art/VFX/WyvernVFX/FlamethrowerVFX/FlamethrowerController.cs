using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlamethrowerController : MonoBehaviour
{
    public bool isActive;
    private VisualEffect flamethrowerVFX;
    private ParticleSystem flamethrowerPS;
    private Vector3 origin;

    private void Start()
    {
        origin = this.transform.position;
        flamethrowerVFX = this.transform.GetChild(0).GetComponent<VisualEffect>();
        flamethrowerPS = this.transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // if isActive is on and the VFX or PS is turned off, turn them on
        if (isActive && !flamethrowerVFX.gameObject.activeSelf || isActive && !flamethrowerPS.gameObject.activeSelf)
        {
            flamethrowerVFX.gameObject.SetActive(true);
            flamethrowerVFX.Play();
            flamethrowerPS.gameObject.SetActive(true);
            flamethrowerPS.Play();
        }
        // if isActive is off and the VFX or PS is turned on, turn them off
        else if (!isActive && flamethrowerVFX.gameObject.activeSelf || !isActive && flamethrowerPS.gameObject.activeSelf)
        {
            flamethrowerVFX.gameObject.SetActive(false);
            flamethrowerPS.gameObject.SetActive(false);
        }

        if (isActive)
        {
            RaycastHit hit;
            origin = this.transform.position;

            if (Physics.Raycast(origin, transform.TransformDirection(Vector3.forward), out hit, 100000f))
            {
                //Debug.DrawRay(origin, transform.TransformDirection(Vector3.forward) * 100,
                //              Color.red);
                Vector3 hitPosition = hit.point;
                float distance = Vector3.Distance(origin, hitPosition);
                flamethrowerVFX.SetFloat("BeamLength", distance / 2);
            }
        }
    }
}
