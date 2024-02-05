using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteSpawnerScript : MonoBehaviour
{
    public GameObject stalactite;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(stalactite,transform.position,Quaternion.identity,gameObject.transform);
    }

    public void SpawnStalactite() {
        Instantiate(stalactite,transform.position,Quaternion.identity,gameObject.transform);
    }

}
