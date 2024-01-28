using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FirstCamera : MonoBehaviour
{
    [SerializeField] private bool firstWeaverCamera;
    [SerializeField] private bool firstFamiliarCamera;

    void Start() {
        AddToCameraMaster();
    }

    void AddToCameraMaster() {
        if (firstWeaverCamera) {
            CameraMasterScript.instance.weaverCameras.Insert(0,gameObject);
            CameraMasterScript.instance.currentCam = GetComponent<CinemachineVirtualCamera>();
        }
        else if (firstFamiliarCamera) {
            CameraMasterScript.instance.familiarCameras.Insert(0,gameObject);
        }
    }
}
