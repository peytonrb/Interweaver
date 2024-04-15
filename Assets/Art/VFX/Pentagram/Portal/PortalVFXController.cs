using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// dear anyone who may come upon this script - i greatly apologize

public class PortalVFXController : MonoBehaviour
{
    public bool isActive;
    private bool isPlaying = false;
    public GameObject orb;
    public GameObject[] effects;
    public GameObject[] startingPS;
    public GameObject[] endingPS;
    public float waitTime;

    void Update()
    {
        if (isActive && !isPlaying)
        {
            isPlaying = true;
            foreach (GameObject ps in startingPS)
            {
                ps.gameObject.SetActive(true);
                ps.GetComponent<ParticleSystem>().Play();
                StartCoroutine(ScalePS(startingPS[3]));
                StartCoroutine(More());
            }
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

    IEnumerator ScalePS(GameObject ps)
    {
        yield return new WaitForSeconds(0.5f);
        float t = 0;
        ParticleSystem.ShapeModule shapePS = ps.GetComponent<ParticleSystem>().shape;
        float startRadius = shapePS.radius;
        while (t < 1f)
        {
            t += Time.deltaTime;
            shapePS.radius = Mathf.Lerp(startRadius, 9f, t);
            yield return null;
        }
    }

    IEnumerator More()
    {
        yield return new WaitForSeconds(0.75f);
        foreach (GameObject effect in effects)
        {
            effect.SetActive(true);
        }

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
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
        bool hasPlayed = false;

        while (t < 1f)
        {
            t += Time.deltaTime;

            if (t < 0.75f && !hasPlayed)
            {
                hasPlayed = true;
                StartCoroutine(idec());

                foreach (GameObject ps in endingPS)
                {
                    ps.SetActive(true);
                    ps.GetComponent<ParticleSystem>().Play();
                    StartCoroutine(ScalePS(endingPS[5]));
                }
            }

            float blend = Mathf.Clamp01(t / 1f);
            material.SetFloat("_Alpha", Mathf.Lerp(startOpacity, 0f, blend));
            yield return null;
        }

        StartCoroutine(WaitForDeath());
    }

    IEnumerator idec()
    {
        yield return new WaitForSeconds(0.25f);
        foreach (GameObject effect in effects)
        {
            if (effect.name != "Orb")
                effect.SetActive(false);
        }
    }

    IEnumerator WaitForDeath()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
