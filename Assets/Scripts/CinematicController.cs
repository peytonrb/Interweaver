using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CinematicController : MonoBehaviour
{
    [Header("Clip 1")]
    public VisualEffect deathVFX;
    public GameObject explosionPS;

    public void Clip1()
    {
        deathVFX.gameObject.SetActive(true);
        deathVFX.Play();
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(6.5f);
        deathVFX.gameObject.SetActive(false);
        explosionPS.SetActive(true);

        foreach (ParticleSystem ps in explosionPS.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }
}
