using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCannonScript : MonoBehaviour
{
    public float fireRate; //Amount of time in seconds to wait between firing
    public float projectileSpeed; //Set to negative if shooting in the negative direction of an axis
    public bool ShootonZ;
    private bool fired = false;
    public GameObject projectile;
    private NetProjectileScript netProjectileScript;

    void Start() {
        netProjectileScript = projectile.GetComponent<NetProjectileScript>();
        netProjectileScript.speed = projectileSpeed;
        netProjectileScript.shootingonZ = ShootonZ;
        
    }

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
