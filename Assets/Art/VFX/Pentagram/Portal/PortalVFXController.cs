using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PortalVFXController : MonoBehaviour
{
    public bool isActive;
    private bool isPlaying = false;
    public GameObject orb;
    public GameObject[] effects;

    void Update()
    {
        if (isActive && !isPlaying)
        {
            isPlaying = true;

            foreach (GameObject effect in effects)
            {
                effect.SetActive(true);
            }

            StartCoroutine(Wait());
        }

        if (!isActive && isPlaying)
        {
            foreach (GameObject effect in effects)
            {
                effect.SetActive(false);
            }

            isPlaying = false;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(FadeOrb());
        //StartCoroutine(WaitAgain());
    }

    IEnumerator FadeOrb()
    {
        float t = 0;
        Material material = orb.GetComponent<Renderer>().material;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 1f);
            material.SetFloat("_Alpha", Mathf.Lerp(0f, 1f, blend));
            yield return null;
        }

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Material material = orb.GetComponent<Renderer>().material;
        float startOpacity = material.GetFloat("_Alpha");

        while (t < 1f)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 1f);
            material.SetFloat("_Alpha", Mathf.Lerp(startOpacity, 0f, blend));
            yield return null;
        }
    }
}
