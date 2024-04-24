using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class DarknessMechanicScript : MonoBehaviour
{
    [Header("Timer")]
    [Range(0, 14)] private float countDown;
    public float deathTime = 5f;
    public bool isSafe;
    public bool canKillPlayer = true;
    private bool hasInvoked;

    private float lastCount;
    float t = 0;
    [SerializeField] private GameObject weaverIcon;

    [SerializeField] private AnimationCurve shakeCurve;

    [SerializeField] private AnimationCurve vignetteCurve;
    private bool isLightOn = true;

    UnityEngine.Rendering.Universal.Vignette vignette;

    public bool disableVignette;

    void Start()
    {
        if (canKillPlayer)
            isSafe = false;
        else
            isSafe = true;
        weaverIcon.SetActive(false);
        hasInvoked = false;
        countDown = 0f;
        StartCoroutine(DarknessTimer());

        if (SceneManager.GetActiveScene().name == "Cavern") {
            UnityEngine.Rendering.VolumeProfile volumeProfile = GameObject.Find("CavernPostProcessing").GetComponent<UnityEngine.Rendering.Volume>()?.profile;
            if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
            if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
            if (disableVignette)
            {
                vignette.intensity.Override(0f);
            }
        }
    }

    void Update()
    {
        if (!disableVignette)
        {
            vignette.intensity.Override(vignetteCurve.Evaluate(countDown / 6));
        }
        

        if (isSafe)
        {
            if (countDown > 0)
                countDown -= Time.deltaTime * 2.2f;

        }
    }

    IEnumerator DarknessTimer()
    {
        countDown = 0;
        while ((countDown < deathTime) && (!isSafe))
        {
            countDown += Time.deltaTime;

            if (countDown > 1)
            {
                //Debug.Log(countDown);
                float shakeIntensity = shakeCurve.Evaluate(countDown / 6);

                CameraMasterScript.instance.ShakeCurrentCamera(shakeIntensity, .2f, 0.1f);

                weaverIcon.SetActive(true);
            }

            yield return null;
        }

        if (countDown >= deathTime)
        {
            GetComponent<MovementScript>().GoToCheckPoint();
        }
    }

    public void PlayerIsSafe()
    {
        isSafe = true;
        if ((isSafe) && (!hasInvoked))
        {
            StopCoroutine(DarknessTimer());
            hasInvoked = true;
            Debug.Log("player is now safe");
        }

    }

    public void PlayerIsNotSafe()
    {
        if (canKillPlayer)
        {
            isSafe = false;
            if ((!isSafe) && (hasInvoked))
            {
                StartCoroutine(DarknessTimer());
                hasInvoked = false;
                Debug.Log("player is now not safe");
            }
        }
        else
        {
            isSafe = true;
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
            // HAS to be a point light lantern
            else if (!isLightOn)
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
