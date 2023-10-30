using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftScript : MonoBehaviour
{
    public AnimationCurve animationCurve;

    public float start = 0f;
    [SerializeField][Range (3f, 10f)] private float boost;
    private float currentBoost = 0;
    private bool boosting;
    public float end = 500f;
    public float t = 1;

    private MovementScript movementScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            if (collider.GetComponent<MovementScript>() != null)
            {
                movementScript = collider.GetComponent<MovementScript>();
                movementScript.ChangeGravity(1);
                if (movementScript.GetVelocity().y > 0)
                {
                    boosting = true;
                }
                //StartCoroutine(boostCountdown(collider));
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (movementScript.GetVelocity().y > 0)
        {
            boosting = true;
        }
        if (boosting)
        {
            Debug.Log("Hey!");
            currentBoost = Mathf.Lerp(start, end, animationCurve.Evaluate(1/t));
            t += 1f * Time.deltaTime;
            movementScript.ChangeGravity(currentBoost);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            boosting = false;
            if (collider.GetComponent<MovementScript>() != null)
            {
                movementScript = collider.GetComponent<MovementScript>();
                movementScript.ResetGravity();
            }
        }
    }

    IEnumerator boostCountdown(Collider collider)
    {
        yield return new WaitForSeconds(0.6f);
        collider.GetComponent<MovementScript>().ChangeGravity(100);
        Debug.Log("YIPPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");

    }
}