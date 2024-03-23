using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagSwapScript : MonoBehaviour
{
    [Header("References")]
    private FamiliarScript familiarScript;
    private GameObject weaver;
    private GameObject wyvern;
    private WyvernBossManager bossManager;
    [Header("Variables")]
    [SerializeField] private float timeToHold = 1f;
    [HideInInspector] private bool isHolding; 
    private float timeHeld = 0f;
    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        wyvern = GameObject.FindGameObjectWithTag("Boss");
        //bossManager = wyvern.GetComponent<WyvernBossManager>();
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
    }

    public void DoSwap()
    {
        isHolding = false;

        if (timeHeld >= timeToHold)
        {
            
            Vector3 prevPosition = transform.position;
            int prevPhase = bossManager.phases;
            //inputManagerScript.PossessWeaver(); // scrungly
            transform.position = weaver.transform.position;
            weaver.transform.position = prevPosition;
            /*if (wyvern.activeSelf) {
                bossManager.StagSwapPhaseSwap(prevPhase);
            }*/
        }
    }
}
