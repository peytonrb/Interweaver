using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalVFXController : MonoBehaviour
{
    public bool isActive;
    private bool isPlaying = false;
    public ShieldVFXController shieldVFX;
    public GameObject[] effects;

    void Update()
    {
        if (isActive && !isPlaying)
        {
            isPlaying = true;

            foreach (GameObject effect in effects)
            {
                effect.SetActive(true);
            }

            StartCoroutine(Wait());
        }

        if (!isActive && isPlaying)
        {
            foreach (GameObject effect in effects)
            {
                effect.SetActive(false);
            }

            shieldVFX.gameObject.SetActive(false);
            isPlaying = false;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        shieldVFX.gameObject.SetActive(true);
        shieldVFX.shouldBeOn = true;
        StartCoroutine(WaitAgain());
    }

    IEnumerator WaitAgain()
    {
        yield return new WaitForSeconds(2f);
        shieldVFX.shouldBeOn = false;
    }
}
