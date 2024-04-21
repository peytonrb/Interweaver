using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StagSwapScript : MonoBehaviour
{
    [Header("References")]
    private GameObject weaver;
    private GameObject wyvern;
    private MovementScript movementScript;
    private WyvernBossManager bossManager;
    [Header("Variables")]
    [SerializeField] private float timeToHold = 1f;
    [HideInInspector] public bool isHolding;
    private float timeHeld = 0f;

    [Header("VFX")]
    private VisualEffect chargeVFX;
    private VisualEffect swapVFX;
    private ParticleSystem flashPS;
    private ParticleSystem chargingPS;

    [Header("Audio")]
    [SerializeField] private AudioClip swapChargeSound;
    [SerializeField] private AudioClip swapCompleteSound;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        weaver = GameObject.FindGameObjectWithTag("Player");
        wyvern = GameObject.FindGameObjectWithTag("Boss");
        if (wyvern != null)
        {
            if (wyvern.TryGetComponent<WyvernBossManager>(out WyvernBossManager dingus))
            {
                bossManager = dingus;
            }
        }

        chargeVFX = this.transform.Find("StagSwapVFX").GetChild(0).GetComponent<VisualEffect>();
        chargeVFX.gameObject.SetActive(false);
        swapVFX = this.transform.Find("StagSwapVFX").GetChild(1).GetComponent<VisualEffect>();
        swapVFX.gameObject.SetActive(false);
        flashPS = this.transform.Find("StagSwapVFX").GetChild(2).GetComponent<ParticleSystem>();
        chargingPS = this.transform.Find("StagSwapVFX").GetChild(3).GetComponent<ParticleSystem>();
        chargingPS.gameObject.SetActive(false);
    }

    public IEnumerator ChargeSwap()
    {

        if (!movementScript.canMove) // prevent swapping while unable to move
        {
            yield break;
        }

        float startTime = Time.time;
        timeHeld = 0f;
        isHolding = true;

        // i apologize for the slightly boof vfx code
        chargeVFX.gameObject.SetActive(true);
        chargeVFX.Play();
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, swapChargeSound, 1f);

        if (!chargingPS.gameObject.activeSelf)
            chargingPS.gameObject.SetActive(true);

        chargingPS.Play();
        bool vfxPlayed = false;

        while (isHolding && (Time.time <= startTime + timeToHold))
        {
            timeHeld += Time.deltaTime;

            if (timeHeld > timeToHold - 0.95f && !vfxPlayed)
            {
                vfxPlayed = true;
                swapVFX.gameObject.SetActive(true);
                swapVFX.Play();
                StartCoroutine(WaitForVFX());
            }

            yield return null;
        }

        flashPS.Play();
        DoSwap();
    }

    public void DoSwap()
    {
        if (!movementScript.canMove) // prevent swapping while unable to move
        {
            return;
        }

        isHolding = false;
        chargeVFX.Stop();

        if (timeHeld >= timeToHold)
        {
            Vector3 prevPosition = transform.position;
            transform.position = weaver.transform.position;
            weaver.transform.position = prevPosition;
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, swapCompleteSound, 1f);
            if (bossManager != null && wyvern != null) // pissing and shitty
            {
                int prevPhase = bossManager.phases;
                if (wyvern.activeSelf && wyvern != null)
                {
                    bossManager.StagSwapPhaseSwap(prevPhase);
                }
            }
        }
    }

    IEnumerator WaitForVFX()
    {
        yield return new WaitForSeconds(5);
        //swapVFX.gameObject.SetActive(false);
        chargeVFX.gameObject.SetActive(false);
    }
}
