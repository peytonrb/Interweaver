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
}
