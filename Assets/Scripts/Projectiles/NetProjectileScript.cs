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
            
    }

    void Update() {
        if (transform.position.magnitude > 100f) {
            Destroy(gameObject);
        }
    }

}
