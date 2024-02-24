using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagSwapScript : MonoBehaviour
{
    [Header("References")]
    private FamiliarScript familiarScript;
    private GameObject weaver;
    [SerializeField] private InputManagerScript inputManagerScript;
    [Header("Variables")]
    [SerializeField] private float timeToHold = 1f;
    [HideInInspector] private bool isHolding; 
    private float timeHeld = 0f;
    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
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
            //inputManagerScript.PossessWeaver(); // scrungly
            transform.position = weaver.transform.position;
            weaver.transform.position = prevPosition;
        }
    }
}
