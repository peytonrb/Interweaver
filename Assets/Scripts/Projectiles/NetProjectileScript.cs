using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetProjectileScript : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float projectileSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(new Vector3(projectileSpeed,0,0), ForceMode.Impulse);
    }

}
