using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleVFXController : MonoBehaviour
{
    public bool activateVFX;
    public float blowUpTime = 1.5f;
    private ParticleSystem firstCirclePS;
    private ParticleSystem explosionPS;

    void Start()
    {
        firstCirclePS = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        explosionPS = this.transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (activateVFX)
        {
            firstCirclePS.gameObject.SetActive(true);
            StartCoroutine(WaitForVFX());
        }
    }

    IEnumerator WaitForVFX()
    {
        yield return new WaitForSeconds(blowUpTime + 0.5f);
        explosionPS.gameObject.SetActive(true);
        activateVFX = false;
        StartCoroutine(TurnOffVFX());
    }

    IEnumerator TurnOffVFX()
    {
        yield return new WaitForSeconds(2f);
        firstCirclePS.gameObject.SetActive(false);
        explosionPS.gameObject.SetActive(false);
    }
}
