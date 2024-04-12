using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CinematicController : MonoBehaviour
{
    [Header("Clip 1")]
    public GameObject explosionPS;

    public void Clip1()
    {
        foreach (ParticleSystem ps in explosionPS.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }
}
