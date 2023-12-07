using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HawkLoFScript : MonoBehaviour
{
    public float timeToDisable = 3;

    public GameObject hawkToEnable;

    public AudioClip cacawClip;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Familiar"))
        {
            hawkToEnable.SetActive(true);
            hawkToEnable.GetComponentInChildren<HawkFollowScript>().Start();
            StartCoroutine(DisableTheHawk());

        }
    }


    public IEnumerator DisableTheHawk()
    {
        yield return new WaitForSeconds(1);

        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, cacawClip);

        yield return new WaitForSeconds(2);

        hawkToEnable.SetActive(false);
        yield break;
    }
}
