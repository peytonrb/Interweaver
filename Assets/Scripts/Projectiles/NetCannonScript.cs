using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetCannonScript : MonoBehaviour
{
    public float fireRate; //Amount of time in seconds to wait between firing
    public float projectileSpeed; //Set to negative if shooting in the negative direction of an axis
    public float firstShotDelayTime = 0;

    private bool fired = false;
    public bool isOn = false;

    //Set this in inspector
    public Transform projectileSpawnPoint;

    public GameObject projectile;
    private NetProjectileScript netProjectileScript;

    void Start() {
        netProjectileScript = projectile.GetComponent<NetProjectileScript>();
        netProjectileScript.speed = projectileSpeed;

        isOn = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Familiar"))
        {
            isOn = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Familiar"))
        {
            isOn = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isOn) {
            if (fired == false) {
                GameObject spawnedShot = Instantiate(projectile, projectileSpawnPoint.position, transform.rotation);

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

    IEnumerator StartOffset()
    {
        yield return new WaitForSeconds(firstShotDelayTime);

        isOn = true;
        yield break;
    }
}
