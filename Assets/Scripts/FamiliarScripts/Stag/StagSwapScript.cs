using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagSwapScript : MonoBehaviour
{
    [Header("References")]
    private GameObject weaver;
    private GameObject wyvern;
    private WyvernBossManager bossManager;
    [Header("Variables")]
    [SerializeField] private float timeToHold = 1f;
    [HideInInspector] public bool isHolding;
    private float timeHeld = 0f;
    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        wyvern = GameObject.FindGameObjectWithTag("Boss");
        if (wyvern != null)
        {
            if (wyvern.TryGetComponent<WyvernBossManager>(out WyvernBossManager dingus))
            {
                bossManager = dingus;
            }
        }
    }

    public IEnumerator ChargeSwap()
    {
        float startTime = Time.time;
        timeHeld = 0f;
        isHolding = true;
        
        while (isHolding && (Time.time <= startTime + timeToHold))
        {
            timeHeld += Time.deltaTime;
            yield return null;
        }

        DoSwap();
    }

    public void DoSwap()
    {
        isHolding = false;

        if (timeHeld >= timeToHold)
        {
            Vector3 prevPosition = transform.position;
            transform.position = weaver.transform.position;
            weaver.transform.position = prevPosition;
            if (bossManager != null && wyvern != null) // pissing and shitty
            {
                int prevPhase = bossManager.phases;
                if (wyvern.activeSelf && wyvern != null) {
                    bossManager.StagSwapPhaseSwap(prevPhase);
                }
            }
        }
    }
}
