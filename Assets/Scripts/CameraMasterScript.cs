using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMasterScript : MonoBehaviour
{
    public GameObject[] checkpoints; //Put all checkpoints here.

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
