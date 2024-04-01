using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StagChargingVFXController : MonoBehaviour
{
    public bool startEffect;
    private bool isPlaying = false;

    [Header("Effect Components")]
    private VisualEffect bodyVFX;
    private ParticleSystem flashPS;
    private ParticleSystem glowPS;
    private ParticleSystem twinklePS;
    private ParticleSystem semicirclePS;

    void Start()
    {
        bodyVFX = this.transform.GetChild(0).GetComponent<VisualEffect>();
        flashPS = this.transform.GetChild(1).GetComponent<ParticleSystem>();
        glowPS = this.transform.GetChild(2).GetComponent<ParticleSystem>();
        twinklePS = this.transform.GetChild(3).GetComponent<ParticleSystem>();
        semicirclePS = this.transform.GetChild(4).GetComponent<ParticleSystem>();

        bodyVFX.gameObject.SetActive(false);
        flashPS.gameObject.SetActive(false);
        glowPS.gameObject.SetActive(false);
        twinklePS.gameObject.SetActive(false);
        semicirclePS.gameObject.SetActive(false);
    }

    void Update()
    {
        if (startEffect)
        {
            StartEffect();
            startEffect = false;
        }
    }

    public void StartEffect()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            bodyVFX.gameObject.SetActive(true);
            glowPS.gameObject.SetActive(true);
            bodyVFX.Play();
            glowPS.Play();

            if (!flashPS.gameObject.activeSelf) // is off until played first time
            {
                flashPS.gameObject.SetActive(true);
                twinklePS.gameObject.SetActive(true);
                semicirclePS.gameObject.SetActive(true);
            }

            glowPS.Play();
            flashPS.Play();
            twinklePS.Play();
            semicirclePS.Play();
        }
    }

    public void EndEffect()
    {
        bodyVFX.Stop();
        bodyVFX.gameObject.SetActive(false);
        glowPS.gameObject.SetActive(false);
        flashPS.Stop();
        twinklePS.Stop();
        semicirclePS.Stop();
        isPlaying = false;
    }
}
