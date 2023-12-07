using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HawkLoFScript : MonoBehaviour
{
    public float timeToDisable = 3;

    public GameObject hawkToEnable;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Familiar"))
        {
            hawkToEnable.SetActive(true);
            StartCoroutine(DisableTheHawk());

        }
    }


    public IEnumerator DisableTheHawk()
    {

        yield return new WaitForSeconds(timeToDisable);

        hawkToEnable.SetActive(false);
        yield break;
    }
}
