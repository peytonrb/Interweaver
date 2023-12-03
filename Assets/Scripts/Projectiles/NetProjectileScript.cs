using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetProjectileScript : MonoBehaviour
{
    private Rigidbody rb;

    private MovementScript MS;

    private MovementScript MSF;
    //*******************************
    //These variables are set by NetCannonScript
    public float speed;
    public float lifetime = 4;
    //*******************************

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);

        StartCoroutine(TimerBeforeDestroy(lifetime));

        MSF = GameObject.FindWithTag("Familiar").GetComponent<MovementScript>();
    }

    public IEnumerator TimerBeforeDestroy(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
        yield break;
    }
    //*************************************************************************
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<MovementScript>().GoToCheckPoint();
        }

        else if (other.gameObject.CompareTag("Familiar"))
        {
            MSF.GoToCheckPoint();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //*************************************************************************************
}
