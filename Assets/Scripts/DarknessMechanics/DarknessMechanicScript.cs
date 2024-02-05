using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class DarknessMechanicScript : MonoBehaviour
{
    [Header("Timer")]
    [Range(0, 14)] private float countDown;
    public float deathTime = 5f;
    [HideInInspector] public bool isSafe;


    private float lastCount;
    float t = 0;

    [SerializeField] private AnimationCurve shakeCurve;

    [SerializeField] private AnimationCurve vignetteCurve;
    private bool isLightOn = true;

    UnityEngine.Rendering.Universal.Vignette vignette;

    void Start()
    {
        isSafe = false;
        countDown = 0f;
        StartCoroutine(DarknessTimer());

        UnityEngine.Rendering.VolumeProfile volumeProfile = GameObject.Find("CavernPostProcessing").GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
    }

    void Update()
    {
        vignette.intensity.Override(vignetteCurve.Evaluate(countDown / 5));

        if (isSafe)
        {
            countDown = Mathf.SmoothStep(lastCount, 0, t);
            t += 0.5f * Time.deltaTime;
        }
    }

    IEnumerator DarknessTimer()
    {
        while ((countDown < deathTime) && (!isSafe))
        {
            countDown += Time.deltaTime;

            if (countDown > 1)
            {
                Debug.Log(countDown);
                float shakeIntensity = shakeCurve.Evaluate(countDown / 5);

                CameraMasterScript.instance.ShakeCurrentCamera(shakeIntensity, .2f, 0.1f);

            }

            yield return null;
        }

        if (countDown >= deathTime)
        {
            GetComponent<MovementScript>().GoToCheckPoint();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LightObject")
        {
            isSafe = true;

            lastCount = countDown;
            t = 0;

            StopCoroutine(DarknessTimer());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "LightObject")
        {
            // is light crystal or mushroom
            if (other.transform.parent.GetComponent<LightCrystalScript>() != null)
            {
                LightCrystalScript lcScript = other.transform.parent.GetComponent<LightCrystalScript>();
                if (!LightSourceScript.Instance.lightsArray[lcScript.arrayIndex].isOn && isLightOn)
                {
                    isLightOn = false;
                    isSafe = false;
                    StartCoroutine(DarknessTimer());
                }
                else if (LightSourceScript.Instance.lightsArray[lcScript.arrayIndex].isOn && !isLightOn)
                {
                    isLightOn = true;
                    isSafe = true;

                    lastCount = countDown;
                    t = 0;

                    StopCoroutine(DarknessTimer());
                }
            }
            else if (other.transform.parent.GetComponent<TimedGlowMushroomsScript>() != null)
            {
                TimedGlowMushroomsScript tmScript = other.transform.parent.GetComponent<TimedGlowMushroomsScript>();
                if (!LightSourceScript.Instance.lightsArray[tmScript.arrayIndex].isOn && isLightOn)
                {
                    isLightOn = false;
                    isSafe = false;
                    StartCoroutine(DarknessTimer());
                }
                else if (LightSourceScript.Instance.lightsArray[tmScript.arrayIndex].isOn && !isLightOn)
                {
                    isLightOn = true;
                    isSafe = true;

                    lastCount = countDown;
                    t = 0;

                    StopCoroutine(DarknessTimer());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LightObject")
        {
            isSafe = false;
            StartCoroutine(DarknessTimer());
        }
    }
}
