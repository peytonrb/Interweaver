using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCrystalScript : MonoBehaviour
{
    public bool isActive;
    public int arrayIndex;
    public float brightness = 4f;
    public bool isWeaveable = true;
    [TextArea] [SerializeField] private string Notes = "Array Index should be the INDEX of the light in LIGHTARRAY "
                                                    + "in the LIGHTSOURCE OBJECT (is probably called LightsManager)";
    private Light crystalLight;
    private float currentBrightness;

    void Start()
    {
        crystalLight = this.gameObject.transform.GetChild(0).GetComponent<Light>();

        if (!isWeaveable)
        {
            this.GetComponent<WeaveableNew>().enabled = false;
        }
    }

    void Update()
    {
        // sets value in darkness struct depending on isActive variable
        if (isActive && !LightSourceScript.Instance.lightsArray[arrayIndex].isOn)
        {
            LightSourceScript.Instance.lightsArray[arrayIndex].isOn = true;
            StartCoroutine(TurnLightOn());
        }
        else if (!isActive && LightSourceScript.Instance.lightsArray[arrayIndex].isOn)
        {
            LightSourceScript.Instance.lightsArray[arrayIndex].isOn = false;
            StartCoroutine(TurnLightOff());
        }
    }

    IEnumerator TurnLightOn()
    {
        float start = Time.time;
        float end = start + 2f;

        while (end >= Time.time)
        {
            crystalLight.intensity = Mathf.Lerp(0f, brightness, (Time.time - start) / 2f);
            yield return null;
        }

        crystalLight.intensity = brightness;
        currentBrightness = crystalLight.intensity;
    }

    IEnumerator TurnLightOff()
    {
        float start = Time.time;
        float end = start + 2f;

        while (end >= Time.time)
        {
            crystalLight.intensity = Mathf.Lerp(currentBrightness, 0f, (Time.time - start) / 2f);
            yield return null;
        }

        crystalLight.intensity = 0f;
    }
}
