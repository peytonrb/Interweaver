using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Sparrow.VolumetricLight;
using UnityEngine.Experimental.GlobalIllumination;

public class LightCrystalScript : MonoBehaviour
{
    public bool isActive;
    public int arrayIndex;
    public float brightness = 0.3f;
    public bool isWeaveable = true;
    [TextArea] [SerializeField] private string Notes = "Array Index should be the INDEX of the light in LIGHTARRAY "
                                                     + "in the LightsManager";
    [Header("Focusing Crystals")]
    public bool isFocusingCrystal;
    private VisualEffect beamEffect;
    private FocusingCrystalScript focusingCrystalScript;
    private VisualEffect startEffect;

    private Light crystalLight;
    private float currentBrightness;
    public bool isActiveDefault = false;
    [Header("Audio")]
    [SerializeField] private AudioClip  crystalOnClip;

    void Start()
    {
        isActiveDefault = isActive;
        if (transform.GetChild(0).TryGetComponent<Light>(out crystalLight))
        {

        }
        else
        {
            crystalLight = this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Light>();
        }
        
        Transform vfx = this.transform.Find("LightBeamBurstVFX");

        if (vfx != null)
        {
            startEffect = vfx.GetComponent<VisualEffect>();
            startEffect.Stop();
        }

        if (!isWeaveable)
        {
            this.GetComponent<WeaveableObject>().enabled = false;
        }

        if (isFocusingCrystal)
        {
            beamEffect = this.gameObject.transform.GetChild(0).transform.GetChild(1).GetComponent<VisualEffect>();
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
            TurnLightOn();
        }
    }

    void Update()
    {
        // sets value in darkness struct depending on isActive variable
        if (isActive && !LightSourceScript.Instance.lightsArray[arrayIndex].isOn)
        {
            var lightData = LightSourceScript.Instance.lightsArray[arrayIndex];
            lightData.isOn = true;
            LightSourceScript.Instance.lightsArray[arrayIndex] = lightData;
            StartCoroutine(TurnLightOn());
        }
        else if (!isActive && LightSourceScript.Instance.lightsArray[arrayIndex].isOn)
        {
            var lightData = LightSourceScript.Instance.lightsArray[arrayIndex];
            lightData.isOn = false;
            LightSourceScript.Instance.lightsArray[arrayIndex] = lightData;
            StartCoroutine(TurnLightOff());
        }
    }

    IEnumerator TurnLightOn()
    {
        // turns on beam
        if (isFocusingCrystal)
        {
            // temp vfx code for testing
            if (startEffect != null) // keep this block after testing
            {
                startEffect.Play();
                startEffect.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                StartCoroutine(BeamFlashes());
            }
            else
            {
                beamEffect.enabled = true;
                beamEffect.Play();
                focusingCrystalScript.isActive = true;
                AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, crystalOnClip);
            }
        }

        float start = Time.time;
        float end = start + 2f;

        while (end >= Time.time)
        {
            crystalLight.intensity = Mathf.Lerp(0f, brightness, (Time.time - start) / 2f);

            // matching volumetric light to sphere collider (trust)
            if (crystalLight.GetComponent<SphereCollider>() != null)
                crystalLight.range = ((3 * Vector3.Magnitude(crystalLight.GetComponent<SphereCollider>().bounds.size)) / (4f * Mathf.PI)) * 1.2f; 
            else if (isFocusingCrystal && beamEffect != null)
                crystalLight.range = beamEffect.GetComponent<CapsuleCollider>().radius * 2f;

            yield return null;
        }
        crystalLight.intensity = brightness;
        currentBrightness = crystalLight.intensity;
    }

    IEnumerator TurnLightOff()
    {
        // turns off beam
        if (isFocusingCrystal)
        {
            // temp vfx code for testing
            if (startEffect != null) // keep this block after testing
            {
                startEffect.Play();
                startEffect.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                beamEffect.Stop();
                beamEffect.enabled = false;
                focusingCrystalScript.isActive = false;
            }
            else
            {
                beamEffect.Stop();
                beamEffect.enabled = false;
                focusingCrystalScript.isActive = false;
            }
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
        startEffect.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        beamEffect.enabled = true;
        beamEffect.Play();
        focusingCrystalScript.isActive = true;
    }

    public void Test()
    {
        Debug.Log("testing");
    }
}
