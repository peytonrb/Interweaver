using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    public Vector3 spawnPos;
    public Quaternion spawnRotation;

    void Awake()
    {
        spawnPos = transform.position;
        spawnRotation = transform.rotation;
    }
}
