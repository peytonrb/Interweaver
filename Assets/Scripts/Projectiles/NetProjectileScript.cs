using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class NetProjectileScript : MonoBehaviour
{
    private Rigidbody rb;

    private MovementScript MS;

    private MovementScript MSF;

    //*******************************
    //These variables are set by NetCannonScript
    public float speed;
    public float lifetime = 4;
    private float decayingLifeTime;
    //*******************************

    public GameObject impactVFX;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);

        //StartCoroutine(TimerBeforeDestroy(lifetime));

        MSF = GameObject.FindWithTag("Familiar").GetComponent<MovementScript>();

        decayingLifeTime = lifetime;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            decayingLifeTime -= Time.deltaTime;
            if (decayingLifeTime <= 0)
            {
                gameObject.SetActive(false);
                decayingLifeTime = lifetime;
            }
        }
    }

    public IEnumerator TimerBeforeDestroy(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        gameObject.SetActive(false);
        yield break;
    }
    //*************************************************************************
    private void OnTriggerEnter(Collider other)
    {
        GameObject clone = Instantiate(impactVFX, transform.position, Random.rotation);

        if (other.gameObject.CompareTag("Player"))
        {

           if (other.GetComponent<PlayerControllerNew>().isDead)
           {
                return;
           }
            other.GetComponent<MovementScript>().GoToCheckPoint();
            Destroy(clone.gameObject, 1f);
            gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Familiar"))
        {
            MSF.GoToCheckPoint();
            Destroy(clone.gameObject, 1f);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(clone.gameObject, 1f);
            gameObject.SetActive(false);
        }
    }
    //*************************************************************************************
}
