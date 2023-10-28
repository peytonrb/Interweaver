using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetProjectileScript : MonoBehaviour
{
    private Rigidbody rb;

    //*******************************
    //These variables are set by NetCannonScript
    public float speed;
    public bool shootingonZ;
    public float lifetime = 4;
    //*******************************

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (shootingonZ == true) {
            rb.AddForce(new Vector3(0,0,speed), ForceMode.Impulse);
        }
        else {
            rb.AddForce(new Vector3(speed,0,0), ForceMode.Impulse);
        }

        StartCoroutine(TimerBeforeDestroy(lifetime));
    }

    public IEnumerator TimerBeforeDestroy(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);


        Destroy(gameObject);
        yield break;
    }

}
