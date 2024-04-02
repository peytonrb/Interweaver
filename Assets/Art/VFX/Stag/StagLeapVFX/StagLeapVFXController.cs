using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StagLeapVFXController : MonoBehaviour
{
    public bool isActive;
    private bool isPlaying = false;

    [Header("Components")]
    public GameObject groundVFX;
    private VisualEffect leapVFX;

    void Start()
    {
        leapVFX = this.transform.GetChild(0).GetComponent<VisualEffect>();

        leapVFX.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive && !isPlaying)
        {
            isPlaying = true;
            StartVFX(new Vector3(0, 0, 0));
        }

        if (!isActive)
        {
            isPlaying = false;
        }
    }

    public void StartVFX(Vector3 pos)
    {
        leapVFX.gameObject.SetActive(true);
        leapVFX.Play();

        GameObject groundEffect = Instantiate(groundVFX, pos, Quaternion.identity);

        groundEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        groundEffect.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        groundEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        groundEffect.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
        groundEffect.transform.GetChild(4).GetComponent<VisualEffect>().Play();

        StartCoroutine(WaitToDestroy(groundEffect));
    }

    IEnumerator WaitToDestroy(GameObject groundEffect)
    {
        yield return new WaitForSeconds(1);
        Destroy(groundEffect);
    }
}
