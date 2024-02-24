using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteSpawnerScript : MonoBehaviour
{
    public GameObject stalactite;
    public bool constantFalling;
    [Tooltip("If true, stalactites have the ability to fall")] public bool canFall;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(stalactite,transform.position,Quaternion.identity,gameObject.transform);
    }

    public void SpawnStalactite() {
        Instantiate(stalactite,transform.position,Quaternion.identity,gameObject.transform);
    }

}
