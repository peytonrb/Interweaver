using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMasterScript : MonoBehaviour
{
    public GameObject[] checkpoints; //Put all checkpoints here.
    public CinemachineVirtualCamera[] vcams;
    private int vcamListLength;
    public int cameraOnPriority;
    

    public void SwitchCameras(int rotationstate) {
        vcamListLength = vcams.Length;
        CameraIndexScript cis = checkpoints[rotationstate].GetComponent<CameraIndexScript>();

        for (int i = 0; i < vcamListLength; i++) {
            if (rotationstate == i) {
                Debug.Log(rotationstate);
                if (cis.triggered == false) {
                    vcams[i].Priority = 0;
                    if (i < vcamListLength - 1) {
                        vcams[i+1].Priority = 1;
                        cameraOnPriority = i+1;
                    }
                    else {
                        vcams[0].Priority = 1;
                        cameraOnPriority = 0;
                    }
                    
                    cis.triggered = true;
                    
                    if (i == 0) {
                        ResetIsLoop();
                    }
                    else if (i == vcamListLength - 1) {
                        ResetCameras();
                    }
                } 
                else {
                    vcams[i].Priority = 1;
                    if (i < vcamListLength - 1) {
                        vcams[i+1].Priority = 0;
                    }
                    else {
                        vcams[0].Priority = 0;
                    }
                    
                    cis.triggered = false;
                    cameraOnPriority = i;

                    if (i == 0) {
                        SetTriggerForIsLoop();
                    }
                    else if (i == vcamListLength - 1) {
                        SetTriggersForCameras();
                    }
                }
            }
        }
        
    }

    public void ResetCameras() {
        for (int i = 0; i < checkpoints.Length; i++) {
            CameraIndexScript cis = checkpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == false) {
                cis.triggered = false;
            }
            
        }
    }

    public void SetTriggersForCameras() {
        for (int i = 0; i < checkpoints.Length; i++) {
            CameraIndexScript cis = checkpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == false) {
                cis.triggered = true;
            }
        }
    }

    public void SetTriggerForIsLoop() {
        for (int i = 0; i < checkpoints.Length; i++) {
            CameraIndexScript cis = checkpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == true) {
                cis.triggered = true;
            }
        }
    }
    public void ResetIsLoop() {
        for (int i = 0; i < checkpoints.Length; i++) {
            CameraIndexScript cis = checkpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == true) {
                cis.triggered = false;
            }
        }
    }
}
