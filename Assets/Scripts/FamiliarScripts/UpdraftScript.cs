using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class UpdraftScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] OwlDiveScript owlDiveScript;
    private MovementScript movementScript;

    [Header("Updraft Curve")]
    public AnimationCurve animationCurve;

    [Header("Variables")]
    private bool inUpdraft; // is character currently in an updraft?
    private bool upDraftEntered; // has a character entered an updraft this frame?
    private float currentBoost = 0;
    [SerializeField] private float maxBoost = 10f; // endpoint for lerp
    private float t = 1; // t for lerp


    private void Awake()
    {
        owlDiveScript = GetComponent<OwlDiveScript>();
        movementScript = GetComponent<MovementScript>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Updraft") && !upDraftEntered)
        {
            if (movementScript)
            {
                upDraftEntered = true;
                movementScript = GetComponent<MovementScript>();
                if (movementScript.GetVelocity().y < 0 && !owlDiveScript.isDiving)
                {
                    movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, 0, movementScript.GetVelocity().z));
                }
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Updraft"))
        {
            inUpdraft = true;
            if (!owlDiveScript.isDiving)
            { 
                if (movementScript.GetVelocity().y < -0.1f)
                {
                    //movementScript.ChangeVelocity(Vector3.zero); // I'm not overly happy with this, it feels way too jerky, would much prefer a lerp but it feels eh
                    float verticalVelocity = movementScript.GetVelocity().y;
                    verticalVelocity = Mathf.Lerp(verticalVelocity, 0, t / 5f); // I genuinelly cannot explain with this feels better but it does so fuck it
                    movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, verticalVelocity, movementScript.GetVelocity().z));
                }
                else
                {
                    currentBoost = Mathf.Lerp(movementScript.GetVelocity().y, maxBoost, animationCurve.Evaluate(1 / t));
                }
                t += 1f * Time.deltaTime;
                movementScript.ChangeGravity(currentBoost);
            }
            else
            {
                t = 1f;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Updraft"))
        {
            inUpdraft = false;

            if (movementScript)
            {
                StartCoroutine(EndUpdraft());
            }
        }
    }

    IEnumerator EndUpdraft()
    {
        yield return new WaitForNextFrameUnit();
        if (!inUpdraft)
        {
            float velocity = movementScript.GetVelocity().y;
            upDraftEntered = false;
            if (velocity > 0)
            {
                movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, Mathf.Abs(velocity) / (t * 10f), movementScript.GetVelocity().z));
                movementScript.ResetGravity();
            }

            t = 1f;
        }
    }
}