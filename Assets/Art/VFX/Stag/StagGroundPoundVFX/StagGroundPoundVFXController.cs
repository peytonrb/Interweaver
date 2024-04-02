using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagGroundPoundVFXController : MonoBehaviour
{
    [Header("Components")]
    private GameObject impactVFX;

    // falling vfx
    private ParticleSystem shardsPS;
    private ParticleSystem twinklePS;
    private ParticleSystem cometPS;
    private TrailRenderer trail;

    void Start()
    {
        impactVFX = this.transform.Find("ImpactExplosionVFX").gameObject;
        impactVFX.gameObject.SetActive(false);

        shardsPS = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        twinklePS = this.transform.GetChild(1).GetComponent<ParticleSystem>();
        cometPS = this.transform.GetChild(2).GetComponent<ParticleSystem>();
        trail = this.transform.GetChild(3).GetComponent<TrailRenderer>();

        shardsPS.gameObject.SetActive(false);
        twinklePS.gameObject.SetActive(false);
        cometPS.gameObject.SetActive(false);
        trail.gameObject.SetActive(false);
    }

    public void PlaySlammingVFX()
    {
        if (!shardsPS.gameObject.activeSelf) // will only happen once - on first active
        {
            shardsPS.gameObject.SetActive(true);
            twinklePS.gameObject.SetActive(true);
            cometPS.gameObject.SetActive(true);
        }
        trail.gameObject.SetActive(true);

        shardsPS.Play();
        twinklePS.Play();
        cometPS.Play();
    }

    public void PlayExplosionVFX()
    {
        impactVFX.gameObject.SetActive(true);

        ParticleSystem[] systemsToPlay = impactVFX.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in systemsToPlay)
        {
            ps.Play();
        }
    }

    public void DisableSlammingVFX()
    {
        trail.gameObject.SetActive(false);
        shardsPS.Stop();
        twinklePS.Stop();
        cometPS.Stop();
    }
}
