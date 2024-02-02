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
    private Light pointLight;
    private float currentBrightness;
    [TextArea][SerializeField] private string Notes = "Array Index should be the INDEX of the light in LIGHTARRAY " 
                                                    + "in the LIGHTSOURCE OBJECT (is probably called LightsManager)";

    void Start()
    {
        pointLight = this.gameObject.transform.GetChild(0).GetComponent<Light>();
        StartCoroutine(PulseLightOn());
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

        LightSourceScript.Instance.lightsArray[arrayIndex].isOn = false;
        isActive = false;
        pointLight.intensity = 0f;
        yield return new WaitForSeconds(lightOffDelay);
        StartCoroutine(PulseLightOn());
    }
}
