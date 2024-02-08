using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LightCrystalScript : MonoBehaviour
{
    public bool isActive;
    public int arrayIndex;
    public float brightness = 4f;
    public bool isWeaveable = true;
    [TextArea]
    [SerializeField]
    private string Notes = "Array Index should be the INDEX of the light in LIGHTARRAY "
                         + "in the LightsManager";
    [Header("Focusing Crystals")]
    public bool isFocusingCrystal;
    private VisualEffect beamEffect;
    private FocusingCrystalScript focusingCrystalScript;
    private VisualEffect startEffect;

    private Light crystalLight;
    private float currentBrightness;
    [HideInInspector] public bool isActiveDefault;

    void Start()
    {
        isActiveDefault = isActive;
        crystalLight = this.gameObject.transform.GetChild(0).GetComponent<Light>();
        Transform vfx = this.transform.Find("LightBeamBurstVFX");
        
        if (vfx != null)
        {
            startEffect = vfx.GetComponent<VisualEffect>();
            startEffect.Stop();
        }

        if (!isWeaveable)
        {
            this.GetComponent<WeaveableNew>().enabled = false;
        }

        if (isFocusingCrystal)
        {
            beamEffect = this.gameObject.transform.GetChild(1).GetComponent<VisualEffect>();
            focusingCrystalScript = beamEffect.gameObject.GetComponent<FocusingCrystalScript>();
        }

        if (!isActive && isFocusingCrystal)
        {
            beamEffect.Stop();
            crystalLight.intensity = 0f;
        }
        else if (isActive && isFocusingCrystal)
        {
            beamEffect.Play();
            crystalLight.intensity = brightness;
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
        // turns on beam - add better vfx
        if (isFocusingCrystal)
        {
            if (startEffect != null)
            {
                startEffect.Play();
                StartCoroutine(BeamFlashes());
            }
            else
            {
                beamEffect.enabled = true;
                beamEffect.Play();
                focusingCrystalScript.isActive = true;
            }
        }

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
        // turns off beam - add better vfx
        if (isFocusingCrystal)
        {
            beamEffect.Stop();
            beamEffect.enabled = false;
            focusingCrystalScript.isActive = false;
        }

        float start = Time.time;
        float end = start + 2f;

        while (end >= Time.time)
        {
            crystalLight.intensity = Mathf.Lerp(currentBrightness, 0f, (Time.time - start) / 2f);
            yield return null;
        }

        crystalLight.intensity = 0f;
    }

    IEnumerator BeamFlashes()
    {
        float waitTime = Random.Range(0.1f, 0.5f);
        yield return new WaitForSeconds(waitTime);
        startEffect.Play();
        beamEffect.enabled = true;
        beamEffect.Play();
        focusingCrystalScript.isActive = true;
    }
}
