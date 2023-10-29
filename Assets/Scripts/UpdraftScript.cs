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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentBoost = Mathf.Lerp(start, end, animationCurve.Evaluate(1/t));
        t += 1f * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            if (collider.GetComponent<MovementScript>() != null)
            {
                MovementScript movementScript = collider.GetComponent<MovementScript>();
                movementScript.ChangeGravity(1);
                if (movementScript.GetGravity() > 0)
                {
                    
                }
                StartCoroutine(boostCountdown(collider));
            }
        }
    }

        private void OnTriggerStay(Collider collider)
    {

    }

    IEnumerator boostCountdown(Collider collider)
    {
        yield return new WaitForSeconds(0.6f);
        collider.GetComponent<MovementScript>().ChangeGravity(100);
        Debug.Log("YIPPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");

    }
}