using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCannonScript : MonoBehaviour
{
    public float fireRate; // Amount of time in seconds to wait between firing
    public float projectileSpeed; // Set to negative if shooting in the negative direction of an axis
    public float firstShotDelayTime = 0;

    private bool fired = false;
    public bool isOn = false;

    // Set this in the inspector
    public Transform projectileSpawnPoint;

    public GameObject projectilePrefab;

    public int amountToPool;
    public List<GameObject> projectilePool = new List<GameObject>();

    void Start()
    {
        isOn = false;
        CreateProjectilePool();
    }

    // Create a pool of projectiles
    void CreateProjectilePool()
    {
        for (int i = 0; i < amountToPool; i++) // Adjust the number as needed
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any other logic here if needed
    }

    public void FireNet()
    {
        if (!fired)
        {
            GameObject projectile = GetPooledProjectile();
            if (projectile != null)
            {
                projectile.transform.position = projectileSpawnPoint.position;
                projectile.transform.rotation = transform.rotation;
                projectile.SetActive(true);

                NetProjectileScript netProjectileScript = projectile.GetComponent<NetProjectileScript>();
                if (netProjectileScript != null)
                {
                    netProjectileScript.speed = projectileSpeed;
                }

                StartCoroutine(Wait());
                fired = true;
            }
        }
    }

    // Get an inactive projectile from the pool
    GameObject GetPooledProjectile()
    {
        foreach (GameObject obj in projectilePool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null; // Pool is empty
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(fireRate);
        fired = false;
    }
}
