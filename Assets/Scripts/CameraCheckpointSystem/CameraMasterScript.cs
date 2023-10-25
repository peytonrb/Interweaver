using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMasterScript : MonoBehaviour
{
    //Weaver Camera + Checkpoints
    public GameObject[] checkpoints; //Put all checkpoints here.
    public CinemachineVirtualCamera[] weaverVirtualCams; //For all cameras that are around the weaver
    
    //Familiar Camera
    public CinemachineVirtualCamera familiarCamera;
    private Vector3 originalFamiliarCamRotation;
    private Vector3 originalFamiliarCamTransposeOffset;
    
    //Floating Island Camera
    public CinemachineVirtualCamera floatingIslandCamera;
    private int vcamListLength;
    public int cameraOnPriority;

    void Start() {
        originalFamiliarCamTransposeOffset = familiarCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        originalFamiliarCamRotation = familiarCamera.transform.eulerAngles;

    }
    

    //WEAVER CAMERAS
    //**************************************************************************************
    public void SwitchCameras(int rotationstate) {
        vcamListLength = weaverVirtualCams.Length;
        CameraIndexScript cis = checkpoints[rotationstate].GetComponent<CameraIndexScript>();

        for (int i = 0; i < vcamListLength; i++) {
            if (rotationstate == i) {
                Debug.Log(rotationstate);
                if (cis.triggered == false) {
                    weaverVirtualCams[i].Priority = 0;
                    if (i < vcamListLength - 1) {
                        weaverVirtualCams[i+1].Priority = 1;
                        cameraOnPriority = i+1;
                    }
                    else {
                        weaverVirtualCams[0].Priority = 1;
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
                    weaverVirtualCams[i].Priority = 1;
                    if (i < vcamListLength - 1) {
                        weaverVirtualCams[i+1].Priority = 0;
                    }
                    else {
                        weaverVirtualCams[0].Priority = 0;
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

    public void SwitchToFamiliarCamera() {
       for(int i = 0; i < weaverVirtualCams.Length; i++) {
            weaverVirtualCams[i].Priority = 0;
        }
        familiarCamera.Priority = 1; 
    }

    public void SwitchToWeaverCamera() {
        weaverVirtualCams[cameraOnPriority].Priority = 1;
        familiarCamera.Priority = 0;
        Debug.Log(cameraOnPriority);
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
    //**************************************************************************************


    //FAMILIAR CAMERA
    //**************************************************************************************
    public void StartLeapOfFaith() {
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        familiarScript.leapOfFaith = true;
        familiarCamera.transform.eulerAngles = new Vector3(90f, familiarCamera.transform.eulerAngles.y, familiarCamera.transform.eulerAngles.z);
        familiarCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, originalFamiliarCamTransposeOffset.y, 0);
    }

    public void EndLeapOfFaith() {
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        familiarScript.leapOfFaith = false;
        familiarCamera.transform.eulerAngles = originalFamiliarCamRotation;
        familiarCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = originalFamiliarCamTransposeOffset;
    }

    //**************************************************************************************


    //FLOATING ISLAND CAMERA
    //**************************************************************************************
    public void FloatingIslandCameraSwitch() {
        GameObject floatingIsland = GameObject.FindGameObjectWithTag("FloatingIsland");
        FloatingIslandScript floatingIslandScript = floatingIsland.GetComponent<FloatingIslandScript>();

        familiarCamera.Priority = 0;
        floatingIslandCamera.Priority = 1;
        floatingIslandScript.cameraswitched = true;
        floatingIslandScript.StartFalling();
    }

    public void FloatingIslandCameraReturn() {
        GameObject floatingIsland = GameObject.FindGameObjectWithTag("FloatingIsland");
        FloatingIslandScript floatingIslandScript = floatingIsland.GetComponent<FloatingIslandScript>();

        familiarCamera.Priority = 1;
        floatingIslandCamera.Priority = 0;
        floatingIslandScript.rb.constraints = RigidbodyConstraints.FreezeAll;
        floatingIslandScript.isislandfalling = false;
    }
}
