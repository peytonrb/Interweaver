using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawnerScript : MonoBehaviour
{
    private ParticleSystem successVFX;
    private bool hasTriggered = false;

    void Start()
    {
        if (!this.GetComponent<LightCrystalScript>().isFocusingCrystal)
            successVFX = this.gameObject.transform.GetChild(0).transform.GetChild(1).GetComponent<ParticleSystem>();
        else
            successVFX = this.gameObject.transform.GetChild(0).transform.GetChild(3).GetComponent<ParticleSystem>();

        Color color = this.transform.GetChild(0).GetComponent<Renderer>().material.color;
        color.a = 0f;
        this.gameObject.SetActive(false);
        successVFX.gameObject.SetActive(false);
    }

    public void SpawnCrystal()
    {
        if (!hasTriggered)
        {
            this.gameObject.SetActive(true);
            successVFX.gameObject.SetActive(true);
            successVFX.Play(true);
            StartCoroutine(FadeIn());
            hasTriggered = true;
        }
    }

    IEnumerator FadeIn()
    {
        this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

        Color color = this.transform.GetChild(0).GetComponent<Renderer>().material.color;
        color.a = 0f;
        float t = 0;

        while (t < 1.5f) // based on length of particle system
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 1.5f);
            color.a = Mathf.Lerp(0f, 100f, blend);
            this.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
            yield return null;
        }

        this.GetComponent<BoxCollider>().enabled = true;
    }
}
