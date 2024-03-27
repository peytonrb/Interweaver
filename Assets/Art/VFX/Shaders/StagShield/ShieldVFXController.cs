using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldVFXController : MonoBehaviour
{
    public bool isActive;
    public bool shouldBeOn; // needs to be varied by another script

    private GameObject stagShield;
    private GameObject outerSwirls;
    private GameObject groundEffect;
    private GameObject activateDeactivateEffect;

    void Start()
    {
        stagShield = this.transform.Find("StagShield").gameObject;
        outerSwirls = this.transform.Find("OuterShieldVFXGraph").gameObject;
        groundEffect = this.transform.Find("GroundEffectVFX").gameObject;
        activateDeactivateEffect = this.transform.Find("ActivatingDeactivatingOrbVFXGraph").gameObject;

        stagShield.SetActive(false);
        outerSwirls.SetActive(false);
        groundEffect.SetActive(false);
        activateDeactivateEffect.SetActive(false);
    }

    void Update()
    {
        if (!isActive && shouldBeOn)
        {
            StartCoroutine(TriggerVFX());
        }
        else if (isActive && !shouldBeOn)
        {
            StartCoroutine(EndVFX());
        }
    }

    IEnumerator TriggerVFX()
    {
        isActive = true;
        activateDeactivateEffect.SetActive(true);
        activateDeactivateEffect.GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(1);

        stagShield.SetActive(true);
        outerSwirls.SetActive(true);
        groundEffect.SetActive(true);
    }

    IEnumerator EndVFX()
    {
        activateDeactivateEffect.SetActive(true);
        activateDeactivateEffect.GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(1.3f);

        stagShield.SetActive(false);
        outerSwirls.SetActive(false);
        groundEffect.SetActive(false);
        isActive = false;
    }
}
