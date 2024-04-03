using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteSpawnerScript : MonoBehaviour
{
    public GameObject stalactite;    
    [Tooltip("If true, stalactites have the ability to fall")] public bool canFall;
    public int amountToPool;
    public List <GameObject> stalactitePool = new List <GameObject>();

    // Start is called before the first frame update
    void Start()
    {        
        CreateStalactitePool();
        canFall = false;
    }

    private void Update()
    {
        if (canFall) 
        {
            SpawnStalactite();
        }
    }

    void CreateStalactitePool()
    {
        for (int i = 0; i < amountToPool; i++) 
        {
            GameObject obj = Instantiate(stalactite);
            obj.SetActive(false);
            stalactitePool.Add(obj);
        }
    }

    public void SpawnStalactite() 
    {
        GameObject activeStalactite = GetPooledStalactite();
        if (activeStalactite != null)
        {
            activeStalactite.transform.position = transform.position;
            activeStalactite.transform.rotation = transform.rotation;
            activeStalactite.transform.parent = gameObject.transform;
            activeStalactite.SetActive(true);
        }
    }

    GameObject GetPooledStalactite()
    {
        foreach(GameObject obj in stalactitePool) 
        {
            if (!obj.activeInHierarchy)
            {
                return obj; 
            }
        }

        return null;
    }

}
