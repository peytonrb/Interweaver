using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftScript : MonoBehaviour
{
    public AnimationCurve animationCurve;

    public float start = 0f;
    private float currentBoost = 0;
    private bool inUpdraft;
    private bool upDraftEntered;
    public float end = 500f;
    private float t = 1; // t for lerp
    private MovementScript movementScript;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Gust") && !inUpdraft)
        {
            if (GetComponent<MovementScript>() != null)
            {
                upDraftEntered = true;    
                movementScript = GetComponent<MovementScript>();
                if (movementScript.GetVelocity().y < 0)
                {
                    movementScript.ChangeVelocity(new Vector3 (movementScript.GetVelocity().x, 0, movementScript.GetVelocity().z));
                }
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Gust"))
        {
            inUpdraft = true;
            currentBoost = Mathf.Lerp(start, end, animationCurve.Evaluate(1/t));
            t += 1f * Time.deltaTime;
            movementScript.ChangeGravity(currentBoost);
        }

    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Gust"))
        {
            if (GetComponent<MovementScript>() != null)
            {
                upDraftEntered = false;
                movementScript = GetComponent<MovementScript>();
                movementScript.ChangeVelocity(new Vector3 (movementScript.GetVelocity().x, currentBoost/(0.5f*t), movementScript.GetVelocity().z));
                movementScript.ResetGravity();
            }

            if (!upDraftEntered)
            {
                inUpdraft = false;
            }

            t = 1;
        }
    }

    IEnumerator boostCountdown(Collider collider)
    {
        yield return new WaitForSeconds(0.6f);
        collider.GetComponent<MovementScript>().ChangeGravity(100);
        Debug.Log("YIPPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");

    }
}