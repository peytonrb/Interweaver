using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{

    public Transform lookAtObject;

    void Update()
    {
        if (lookAtObject == null)
            transform.LookAt(Camera.main.transform);
        else
        {
            transform.LookAt(new Vector3(transform.position.x, lookAtObject.position.y, transform.position.z));
        }
    }
}
