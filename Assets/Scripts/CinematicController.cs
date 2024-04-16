using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CinematicController : MonoBehaviour
{
    [Header("Clip 1")]
    public GameObject explosionPS;

    [Header("Clip 2")]
    public GameObject transformPS;
    public ParticleSystem flashPS;
    public GameObject rival;
    public GameObject portalVFX;
    public GameObject weaver;

    [Header("Clip 3")]
    public Animator animator;

    [Header("Clip 4")]
    public StagChargingVFXController stagChargingVFXScript;
    public StagLeapVFXController stagLeapVFXScript;
    public StagGroundPoundVFXController stagGroundPoundVFXScript;
    public SensorController sensor;

    public void Clip1()
    {
        foreach (ParticleSystem ps in explosionPS.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }

    public void PlayTransformEffect()
    {
        transformPS.SetActive(true);
    }

    public void StopTransformEffect()
    {
        transformPS.SetActive(false);
    }

    public void PlayFlash()
    {
        flashPS.gameObject.SetActive(true);
        rival.SetActive(false);
        flashPS.Play();
    }

    public void portal()
    {
        portalVFX.GetComponent<PortalVFXController>().isActive = true;
    }

    public void DeactivateWeaver()
    {
        weaver.SetActive(false);
    }

    public void AnimateLookUp()
    {
        animator.SetBool("LookUp", true);
    }

    public void StagCharge()
    {
        stagChargingVFXScript.StartEffect();
    }

    public void StagLeap()
    {
        stagLeapVFXScript.StartVFX(stagLeapVFXScript.transform.position);
    }

    public void StagSlam()
    {
        stagGroundPoundVFXScript.PlaySlammingVFX();
    }

    public void StagExplosion()
    {
        stagGroundPoundVFXScript.DisableSlammingVFX();
        stagGroundPoundVFXScript.PlayExplosionVFX();
    }

    public void ActivateSensor()
    {
        sensor.isActive = true;
    }
}
