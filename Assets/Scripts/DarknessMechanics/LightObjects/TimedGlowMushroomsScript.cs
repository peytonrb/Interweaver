using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedGlowMushroomsScript : MonoBehaviour
{
    public float glowTime = 2f;
    [Range(0, 10)] public float brightness;
    public float lightOnDelay;
    public float lightOffDelay;
    public int arrayIndex; // add the INDEX of the light in LIGHTARRAY in the LIGHTSOURCE OBJECT
    [HideInInspector] public bool isActive;

    [Header("Has Start Delay?")]
    public bool hasStartDelay = false;
    public float startDelayTime = 2f;

    private Light pointLight;
    private float currentBrightness;
    [TextArea][SerializeField] private string Notes = "Array Index should be the INDEX of the light in LIGHTARRAY " 
                                                    + "in the LIGHTSOURCE OBJECT (is probably called LightsManager)";

    void Start()
    {
        pointLight = this.gameObject.transform.GetChild(0).GetComponent<Light>();

        if (hasStartDelay)
        {
            StartCoroutine(WaitToStart());
        }
        else
        {
            StartCoroutine(PulseLightOn());
        }
    }

    IEnumerator WaitToStart()
    {
       var lightData = LightSourceScript.Instance.lightsArray[arrayIndex];
        lightData.isOn = false;
        LightSourceScript.Instance.lightsArray[arrayIndex] = lightData;
        isActive = false;
        pointLight.intensity = 0f;
        yield return new WaitForSeconds(startDelayTime);
        StartCoroutine(PulseLightOn());
    }

    IEnumerator PulseLightOn()
    {
        float start = Time.time;
        float end = start + glowTime;
        isActive = true;
        var lightData = LightSourceScript.Instance.lightsArray[arrayIndex];
        lightData.isOn = true;
        LightSourceScript.Instance.lightsArray[arrayIndex] = lightData;

        while (end >= Time.time)
        {
            pointLight.intensity = Mathf.Lerp(0f, brightness, (Time.time - start) / glowTime);
            yield return null;
        }

        pointLight.intensity = brightness;
        currentBrightness = pointLight.intensity;
        yield return new WaitForSeconds(lightOnDelay);
        StartCoroutine(PulseLightOff());
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

        var lightData = LightSourceScript.Instance.lightsArray[arrayIndex];
        lightData.isOn = false;
        LightSourceScript.Instance.lightsArray[arrayIndex] = lightData;
        pointLight.intensity = 0f;
        yield return new WaitForSeconds(lightOffDelay);
        StartCoroutine(PulseLightOn());
    }
}
