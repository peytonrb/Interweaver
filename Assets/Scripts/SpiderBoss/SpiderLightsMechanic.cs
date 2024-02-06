using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLightsMechanic : MonoBehaviour
{
    public float glowTime = 2f;
    [Range(0, 10)] public float brightness;
    public float lightOnDelay;
    public float lightOffDelay;
    public int arrayIndex; // add the INDEX of the light in LIGHTARRAY in the LIGHTSOURCE OBJECT
    [HideInInspector] public bool isActive;
    private Light pointLight;
    private float currentBrightness;
    [TextArea]
    [SerializeField]
    private string Notes = "Array Index should be the INDEX of the light in LIGHTARRAY "
                                                    + "in the LIGHTSOURCE OBJECT (is probably called LightsManager)";
    private float intervalSpiders;

    [SerializeField] private bool isBossSpider;

    [SerializeField] private int mySections = 1;

    //sounds here

    void Start()
    {
        if (isBossSpider)
        {
            StartCoroutine(RepeatEverySetIntervals());
        }
        isActive = true;

        pointLight = this.gameObject.transform.GetChild(0).GetComponent<Light>();
        pointLight.intensity = brightness;
        currentBrightness = pointLight.intensity;
    }

    IEnumerator PulseLightOn()
    {
        float start = Time.time;
        float end = start + glowTime;
        isActive = true;
        LightSourceScript.Instance.lightsArray[arrayIndex].isOn = true;

        while (end >= Time.time)
        {
            pointLight.intensity = Mathf.Lerp(0f, brightness, (Time.time - start) / glowTime);
            yield return null;
        }
        //sounds here? will have to test
        pointLight.intensity = brightness;
        currentBrightness = pointLight.intensity;
        yield return new WaitForSeconds(lightOnDelay);
        //sounds here? will have to test
        StartCoroutine(RepeatEverySetIntervals());
    }

    IEnumerator PulseLightOff()
    {
        float start = Time.time;
        float end = start + glowTime;

        while (end >= Time.time)
        {
            pointLight.intensity = Mathf.Lerp(currentBrightness, 0f, (Time.time - start) / glowTime);
            yield return null;
        }
        //sounds here? will have to test
        LightSourceScript.Instance.lightsArray[arrayIndex].isOn = false;
        isActive = false;
        pointLight.intensity = 0f;
        yield return new WaitForSeconds(lightOffDelay);
        //sounds here? will have to test
        StartCoroutine(PulseLightOn());
    }

    public void GoUpSections()
    {
        mySections++;
    }

    IEnumerator RepeatEverySetIntervals()
    {

        switch (mySections)
        {
            case 1:
                intervalSpiders = 30f;
                break;

            case 2:
                intervalSpiders = 25f;
                break;

            case 3:
                intervalSpiders = 20f;
                break;
        }
        yield return new WaitForSeconds(intervalSpiders);
        StartCoroutine(PulseLightOff());
        Debug.Log("breh " + intervalSpiders + " seconds");

    }
}
