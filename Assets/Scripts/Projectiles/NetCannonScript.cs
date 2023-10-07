using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCannonScript : MonoBehaviour
{
    public float fireRate; //Amount of time in seconds to wait between firing
    private bool fired = false;
    public GameObject projectile;
    

    // Update is called once per frame
    void Update()
    {
        if (fired == false) {
            Instantiate(projectile, transform.position, projectile.transform.rotation);
            StartCoroutine(Wait());
            fired = true;
        }
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(fireRate);
        fired = false;
    }
}
