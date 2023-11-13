using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetCannonScript : MonoBehaviour
{
    public float fireRate; //Amount of time in seconds to wait between firing
    public float projectileSpeed; //Set to negative if shooting in the negative direction of an axis

    private bool fired = false;
    public bool isOn;

    public GameObject projectile;
    private NetProjectileScript netProjectileScript;

    void Start() {
        netProjectileScript = projectile.GetComponent<NetProjectileScript>();
        netProjectileScript.speed = projectileSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn) {
            if (fired == false) {
                GameObject spawnedShot = Instantiate(projectile, transform.position, transform.rotation);

                spawnedShot.GetComponent<NetProjectileScript>().speed = projectileSpeed;

                StartCoroutine(Wait());
                fired = true;
            }
        }
        
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(fireRate);
        fired = false;
    }
}
