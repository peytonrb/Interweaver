using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StagLeapVFXController : MonoBehaviour
{
    public bool isActive;
    private bool isPlaying = false;
    private GameObject stag;

    [Header("Components")]
    private VisualEffect leapVFX;
    private ParticleSystem shockwavePS;
    private ParticleSystem smokePS;
    private ParticleSystem flashPS;
    private ParticleSystem twinkleImpactPS;

    void Start()
    {
        leapVFX = this.transform.GetChild(0).GetComponent<VisualEffect>();
        Transform groundEffect = this.transform.GetChild(1);
        shockwavePS = groundEffect.GetChild(0).GetComponent<ParticleSystem>();
        smokePS = groundEffect.GetChild(1).GetComponent<ParticleSystem>();
        flashPS = groundEffect.GetChild(2).GetComponent<ParticleSystem>();
        twinkleImpactPS = groundEffect.GetChild(3).GetComponent<ParticleSystem>();
        //stag = this.transform.parent.gameObject;

        leapVFX.gameObject.SetActive(false);
        shockwavePS.gameObject.SetActive(false);
        smokePS.gameObject.SetActive(false);
        flashPS.gameObject.SetActive(false);
        twinkleImpactPS.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive && !isPlaying)
        {
            isPlaying = true;
            StartVFX();
        }

        if (!isActive)
        {
            isPlaying = false;
        }
    }

    public void StartVFX()
    {
        leapVFX.gameObject.SetActive(true);
        leapVFX.Play();

        // spawn ground ones on ground - get stag position
        if (!shockwavePS.gameObject.activeSelf) // only happens first time effect plays
        {
            shockwavePS.gameObject.SetActive(true);
            smokePS.gameObject.SetActive(true);
            flashPS.gameObject.SetActive(true);
            twinkleImpactPS.gameObject.SetActive(true);
        }

        shockwavePS.Play();
        smokePS.Play();
        flashPS.Play();
        twinkleImpactPS.Play();
    }
}
