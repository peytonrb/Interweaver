using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{

    public Transform spawnLoc;

    public GameObject spawnedPrefab;

    // Update is called once per frame
    public void OnDestroy()
    {
        Instantiate(spawnedPrefab, spawnLoc);
    }
        
    
}
