using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AltarScript : MonoBehaviour
{
    [Header("References")]
    public GameObject cutsceneManager;
    private CutsceneManagerScript cutsceneManagerScript;
    [SerializeField] private InputManagerScript inputManagerScript;
    [SerializeField] private GameObject hatch;
    private FamiliarScript familiarScript;
    private PlayerController playerController;
    
    [Header("Variables for Orb")]
    [SerializeField] private Transform orbSnapPoint;
    private float currentSpeed = 0;
    [SerializeField] private float orbSnapSpeed = 3f;
    [SerializeField] private AnimationCurve animationCurve;
    private float t = 1;
    [Header("Variables for Event")]
    [SerializeField] private Transform owlTeleportPoint;
    [SerializeField][Min(0f)] float teleportWaitTime;

    void Start()
    {
        familiarScript = GameObject.FindGameObjectWithTag("Familiar").GetComponent<FamiliarScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cutsceneManagerScript = cutsceneManager.GetComponent<CutsceneManagerScript>();

        if (!cutsceneManagerScript || !familiarScript || !playerController || !inputManagerScript)
        {
            Debug.LogWarning("Required scripts for Orb Altar not present and/or fucky, disabling myself!");
            gameObject.SetActive(false);
        }
    }
    

    void OnTriggerEnter(Collider collider)
    {
        playerController.Uninteract();
        inputManagerScript.canSwitch = false;
        if (collider.gameObject.CompareTag("Altar Orb"))
        {   
            if (collider.GetComponent<Rigidbody>())
            {
                collider.GetComponent<Rigidbody>().isKinematic = true;
            }
            collider.gameObject.layer = LayerMask.NameToLayer("Default");
            Vector3 height = new Vector3(collider.transform.position.x, orbSnapPoint.position.y, collider.transform.position.z);
            StartCoroutine(RaiseToSnapPoint(collider, height));
        }
    }

    IEnumerator RaiseToSnapPoint(Collider orb, Vector3 height)
    {
        float distance = Vector3.Distance(orb.transform.position, height);
        
        
        if (distance > 0.3f)
        {
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, height, currentSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
            currentSpeed = Mathf.Lerp(currentSpeed, orbSnapSpeed, animationCurve.Evaluate(t));
            t += 1f * Time.deltaTime;
            StartCoroutine(RaiseToSnapPoint(orb, height));
        }
        else 
        {
            StartCoroutine(PullOrbToSnapPoint(orb));
            yield return null; 
        }
    }

    IEnumerator PullOrbToSnapPoint(Collider orb)
    {   
        float distance = Vector3.Distance(orb.transform.position, orbSnapPoint.position);
        
        if (distance > 0.1f)
        {
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, orbSnapPoint.position, currentSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
            currentSpeed = Mathf.Lerp(currentSpeed, orbSnapSpeed, animationCurve.Evaluate(t));
            t += 1f * Time.deltaTime;
            StartCoroutine(PullOrbToSnapPoint(orb));
        }
        else 
        {
            orb.transform.position = orbSnapPoint.position;
            OwlTeleport();
            PlayCutscene();
            if (hatch)
            {
                Destroy(hatch);
            }
            yield return null; 
        }
    }

    private void OwlTeleport()
    {
        familiarScript.gameObject.transform.position = owlTeleportPoint.transform.position;
    }

    private void PlayCutscene()
    {
        inputManagerScript.canSwitch = true;
        cutsceneManagerScript.StartCutscene();
        inputManagerScript.PossessFamiliar();
        inputManagerScript.canSwitch = false;
    }
}
