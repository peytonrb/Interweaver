using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawnerScript : MonoBehaviour
{
    public SensorController sensor;
    public PressurePlateScript pplate;
    public bool doesPlatformDespawn;
    private ParticleSystem successVFX;
    private bool platformSpawned = false;
    private bool pplateTriggered = false;

    void Start()
    {
        successVFX = this.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        this.GetComponent<MeshRenderer>().enabled = false; // not setting inactive for particle system
        this.GetComponent<BoxCollider>().enabled = false;
        successVFX.gameObject.SetActive(false);
    }

    void Update()
    {
        if (sensor != null && !sensor.isActive && platformSpawned && doesPlatformDespawn)
        {
            DespawnPlatform();
        }
        else if (pplate != null && !pplate.standingOnPlate && platformSpawned && doesPlatformDespawn)
        {
            pplateTriggered = false;
            DespawnPlatform();
        }
    }

    public void SpawnPlatform()
    {
        if (pplate != null && !pplateTriggered || sensor != null)
        {
            pplateTriggered = true;
            successVFX.gameObject.SetActive(true);
            successVFX.Play(true);
            StartCoroutine(FadePlatformIn());

            platformSpawned = true;
        }
    }

    private void DespawnPlatform()
    {
        successVFX.Play(true);
        StartCoroutine(FadePlatformOut());
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
        platformSpawned = false;
    }

    IEnumerator FadePlatformIn()
    {
        this.GetComponent<MeshRenderer>().enabled = true;

        Color color = this.GetComponent<Renderer>().material.color;
        color.a = 0f;
        float t = 0;

        while (t < 1.5f) // based on length of particle system
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 1.5f);
            color.a = Mathf.Lerp(0f, 100f, blend);
            this.GetComponent<Renderer>().material.color = color;
            yield return null;
        }

        this.GetComponent<BoxCollider>().enabled = true;
    }

    IEnumerator FadePlatformOut()
    {
        Color color = this.GetComponent<Renderer>().material.color;
        color.a = 100f;
        float t = 0;

        while (t < 1.5f) // based on length of particle system
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 1.5f);
            color.a = Mathf.Lerp(100f, 0f, blend);
            yield return null;
        }
    }
}
