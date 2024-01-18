using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Linq;

public class CameraMasterScript : MonoBehaviour
{
    //Singleton
    public static CameraMasterScript instance;

    private CinemachineVirtualCamera currentCam;

    //Weaver Camera + Checkpoints
    [HideInInspector] public GameObject[] weaverCheckpoints;
    [HideInInspector] public GameObject[] weaverCameras;
    private int weaverCamerasTriggeredSinceLastCheckpoint;
    public int lastWeaverCameraTriggered; //This is the last camera that is triggered upon entering the next checkpoint.
    //When the player dies, this camera index will be activated when the player is teleported back to the previous checkpoint.
    
    //Familiar Camera + Checkpoints
    [HideInInspector] public GameObject[] familiarCheckpoints;
    [HideInInspector] public GameObject[] familiarCameras;
    private int familiarCamerasTriggeredSinceLastCheckpoint;
    [HideInInspector] public int lastFamiliarCameraTriggered;

    private int weaverVcamListLength;
    private int familiarVcamListLength;
    public int weaverCameraOnPriority;
    public int familiarCameraOnPriority;

    public CinemachineVirtualCamera leapOfFaithCamera;

    //Screenshake
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        weaverCameras = GameObject.FindGameObjectsWithTag("WeaverCamera");
        familiarCameras = GameObject.FindGameObjectsWithTag("FamiliarCamera");

        weaverCheckpoints = GameObject.FindGameObjectsWithTag("WeaverCameraTrigger");
        familiarCheckpoints = GameObject.FindGameObjectsWithTag("FamiliarCameraTrigger");
        
        //Sets Camera Priorities
        for (int i = 0; i < weaverCameras.Length; i++) {
            CinemachineVirtualCamera weavervcam = weaverCameras[i].GetComponent<CinemachineVirtualCamera>();
            if (i > 0) {
                weavervcam.Priority = 0;
            }
            else {
                weavervcam.Priority = 1;
            }
        }
        for (int i = 0; i < familiarCameras.Length; i++) {
            CinemachineVirtualCamera familiarvcam = familiarCameras[i].GetComponent<CinemachineVirtualCamera>();
            if (i > 0) {
                familiarvcam.Priority = 0;
            }
            else {
                familiarvcam.Priority = 1;
            }
        }

        //Sets index # for checkpoints
        for (int i = 0; i < weaverCheckpoints.Length; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            cis.cameraIndex = i;
        }
        for (int i = 0; i < familiarCheckpoints.Length; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            cis.cameraIndex = i;
        }

        //Error messages
        if (weaverCheckpoints.Length < weaverCameras.Length - 1) {
            Debug.LogError("ERROR: There isn't enough camera triggers on the weaver side. This may cause the incorrect camera to be on at certain sections of the level. Assure that there is exactly one camera more than the amount of camera triggers.");
        }
        if (familiarCheckpoints.Length < familiarCameras.Length - 1) {
            Debug.LogError("ERROR: There isn't enough camera triggers on the familiar side. This may cause the incorrect camera to be on at certain sections of the level. Assure that there is exactly one camera more than the amount of camera triggers.");
        }
        if (weaverCheckpoints.Length > weaverCameras.Length - 1) {
            Debug.LogError("ERROR: There are too many camera triggers on the weaver side. This may cause the incorrect camera to be on at certain sections of the level. Assure that there is exactly one camera more than the amount of camera triggers.");
        }
        if (familiarCheckpoints.Length > familiarCameras.Length - 1) {
            Debug.LogError("ERROR: There are too many camera triggers on the familiar side. This may cause the incorrect camera to be on at certain sections of the level. Assure that there is exactly one camera more than the amount of camera triggers.");
        }
        
        
        weaverCamerasTriggeredSinceLastCheckpoint = 0;
        familiarCamerasTriggeredSinceLastCheckpoint = 0;
        lastWeaverCameraTriggered = 0;
        lastFamiliarCameraTriggered = 0;

    }

    //WEAVER CAMERAS
    //**************************************************************************************
    public void SwitchWeaverCameras(int rotationstate) {
        weaverVcamListLength = weaverCameras.Length;
        CameraIndexScript cis = weaverCheckpoints[rotationstate].GetComponent<CameraIndexScript>();

        for (int i = 0; i < weaverVcamListLength; i++) {
            if (rotationstate == i) {
                Debug.Log(rotationstate);
                if (cis.triggered == false) {
                    CinemachineVirtualCamera previousvcam = weaverCameras[i].GetComponent<CinemachineVirtualCamera>();
                    previousvcam.Priority = 0;
                    if (i < weaverVcamListLength - 1) {
                        CinemachineVirtualCamera nextvcam = weaverCameras[i+1].GetComponent<CinemachineVirtualCamera>();
                        nextvcam.Priority = 1;
                        weaverCameraOnPriority = i+1;
                    }
                    else {
                        CinemachineVirtualCamera firstvcam = weaverCameras[0].GetComponent<CinemachineVirtualCamera>();
                        firstvcam.Priority = 1;
                        weaverCameraOnPriority = 0;
                    }
                    
                    cis.triggered = true;
                    weaverCamerasTriggeredSinceLastCheckpoint += 1;
                    
                    if (i == 0) {
                        ResetWeaverIsLoop();
                    }
                    else if (i == weaverVcamListLength - 1) {
                        ResetWeaverCameras();
                    }
                } 
                else {
                    CinemachineVirtualCamera previousvcam = weaverCameras[i].GetComponent<CinemachineVirtualCamera>();
                    previousvcam.Priority = 1;
                    if (i < weaverVcamListLength - 1) {
                        CinemachineVirtualCamera nextvcam = weaverCameras[i+1].GetComponent<CinemachineVirtualCamera>();
                        nextvcam.Priority = 0;
                    }
                    else {
                        CinemachineVirtualCamera firstvcam = weaverCameras[0].GetComponent<CinemachineVirtualCamera>();
                        firstvcam.Priority = 0;
                    }
                    
                    cis.triggered = false;
                    //weaverCamerasTriggered[rotationstate] = false;
                    weaverCameraOnPriority = i;
                    weaverCamerasTriggeredSinceLastCheckpoint -= 1;

                    if (i == 0) {
                        SetTriggerForWeaverIsLoop();
                    }
                    else if (i == weaverVcamListLength - 1) {
                        SetTriggersForWeaverCameras();
                    }
                }
            }
        }
        
    }
    
    public void WeaverCameraReturnOnDeath(int rotationstate) {
        int cameraIndexTriggersToReset = rotationstate + weaverCamerasTriggeredSinceLastCheckpoint; 
        weaverCameraOnPriority = rotationstate;
        
        for (int i = rotationstate; i < cameraIndexTriggersToReset; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            CinemachineVirtualCamera returnvcam = weaverCameras[rotationstate].GetComponent<CinemachineVirtualCamera>();
            CinemachineVirtualCamera othervcams = weaverCameras[i].GetComponent<CinemachineVirtualCamera>();
            returnvcam.Priority = 1;
            if (i > rotationstate) {
                othervcams.Priority = 0;
            }
            cis.triggered = false;
        }

        weaverCamerasTriggeredSinceLastCheckpoint = 0;
    }

    public void SwitchToFamiliarCamera() {
       for(int i = 0; i < weaverCameras.Length; i++) {
            CinemachineVirtualCamera vcams = weaverCameras[i].GetComponent<CinemachineVirtualCamera>();
            vcams.Priority = 0;
        }
        CinemachineVirtualCamera familiarvcam = familiarCameras[familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        familiarvcam.Priority = 1;
    }

    public void SwitchToWeaverCamera() {
        for(int i = 0; i < familiarCameras.Length; i++) {
            CinemachineVirtualCamera vcams = familiarCameras[i].GetComponent<CinemachineVirtualCamera>();
            vcams.Priority = 0;
        }
        CinemachineVirtualCamera vcam = weaverCameras[weaverCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 1;
        
        Debug.Log(weaverCameraOnPriority);
    }

    public void ResetWeaverCameras() {
        for (int i = 0; i < weaverCheckpoints.Length; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == false) {
                cis.triggered = false;
            }
            
        }
    }

    public void SetTriggersForWeaverCameras() {
        for (int i = 0; i < weaverCheckpoints.Length; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == false) {
                cis.triggered = true;
            }
        }
    }

    public void SetTriggerForWeaverIsLoop() {
        for (int i = 0; i < weaverCheckpoints.Length; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == true) {
                cis.triggered = true;
            }
        }
    }

    public void ResetWeaverIsLoop() {
        for (int i = 0; i < weaverCheckpoints.Length; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == true) {
                cis.triggered = false;
            }
        }
    }

    //This function tells the checkpoint what camera will be active when the player dies.
    public void ResetWeaverCamerasTriggered() {
        weaverCamerasTriggeredSinceLastCheckpoint = 0;
        lastWeaverCameraTriggered = weaverCameraOnPriority;
    }
    //**************************************************************************************


    //FAMILIAR CAMERA
    //**************************************************************************************
    public void SwitchFamiliarCameras(int rotationstate) {
        familiarVcamListLength = familiarCameras.Length;
        CameraIndexScript cis = familiarCheckpoints[rotationstate].GetComponent<CameraIndexScript>();

        for (int i = 0; i < familiarVcamListLength; i++) {
            if (rotationstate == i) {
                Debug.Log(rotationstate);
                if (cis.triggered == false) {
                    CinemachineVirtualCamera previousvcam = familiarCameras[i].GetComponent<CinemachineVirtualCamera>();
                    previousvcam.Priority = 0;
                    if (i < familiarVcamListLength - 1) {
                        CinemachineVirtualCamera nextvcam = familiarCameras[i+1].GetComponent<CinemachineVirtualCamera>();
                        nextvcam.Priority = 1;
                        familiarCameraOnPriority = i+1;
                    }
                    else {
                        CinemachineVirtualCamera firstvcam = familiarCameras[0].GetComponent<CinemachineVirtualCamera>();
                        firstvcam.Priority = 1;
                        familiarCameraOnPriority = 0;
                    }
                    
                    cis.triggered = true;
                    //familiarCamerasTriggered[rotationstate] = true;
                    familiarCamerasTriggeredSinceLastCheckpoint += 1;
                    
                    if (i == 0) {
                        ResetFamiliarIsLoop();
                    }
                    else if (i == familiarVcamListLength - 1) {
                        ResetFamiliarCameras();
                    }
                } 
                else {
                    CinemachineVirtualCamera previousvcam = familiarCameras[i].GetComponent<CinemachineVirtualCamera>();
                    previousvcam.Priority = 1;
                    if (i < familiarVcamListLength - 1) {
                        CinemachineVirtualCamera nextvcam = familiarCameras[i+1].GetComponent<CinemachineVirtualCamera>();
                        nextvcam.Priority = 0;
                    }
                    else {
                        CinemachineVirtualCamera firstvcam = familiarCameras[0].GetComponent<CinemachineVirtualCamera>();
                        firstvcam.Priority = 0;
                    }
                    
                    cis.triggered = false;
                    //familiarCamerasTriggered[rotationstate] = false;
                    familiarCameraOnPriority = i;
                    familiarCamerasTriggeredSinceLastCheckpoint -= 1;

                    if (i == 0) {
                        SetTriggerForFamiliarIsLoop();
                    }
                    else if (i == familiarVcamListLength - 1) {
                        SetTriggersForFamiliarCameras();
                    }
                }
            }
        }
        
    }

    public void FamiliarCameraReturnOnDeath(int rotationstate) {
        int cameraIndexTriggersToReset = rotationstate + familiarCamerasTriggeredSinceLastCheckpoint;
        familiarCameraOnPriority = rotationstate;
        
        for (int i = rotationstate; i < cameraIndexTriggersToReset; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            CinemachineVirtualCamera returnvcam = familiarCameras[rotationstate].GetComponent<CinemachineVirtualCamera>();
            CinemachineVirtualCamera othervcams = familiarCameras[i].GetComponent<CinemachineVirtualCamera>();
            returnvcam.Priority = 1;
            if (i > rotationstate) {
                othervcams.Priority = 0;   
            }
            cis.triggered = false;
            
        }

        familiarCamerasTriggeredSinceLastCheckpoint = 0;
    }

    public void ResetFamiliarCameras() {
        for (int i = 0; i < familiarCheckpoints.Length; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == false) {
                cis.triggered = false;
            }
            
        }
    }

    public void SetTriggersForFamiliarCameras() {
        for (int i = 0; i < familiarCheckpoints.Length; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == false) {
                cis.triggered = true;
            }
        }
    }

    public void SetTriggerForFamiliarIsLoop() {
        for (int i = 0; i < familiarCheckpoints.Length; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == true) {
                cis.triggered = true;
            }
        }
    }

    public void ResetFamiliarIsLoop() {
        for (int i = 0; i < familiarCheckpoints.Length; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            if (cis.isLoop == true) {
                cis.triggered = false;
            }
        }
    }

    //This function tells the checkpoint what camera will be active when the player dies.
    public void ResetFamiliarCamerasTriggered() {
        familiarCamerasTriggeredSinceLastCheckpoint = 0;
        lastFamiliarCameraTriggered = familiarCameraOnPriority;
    }

    public void StartLeapOfFaith() {
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        familiarScript.leapOfFaith = true;
        CinemachineVirtualCamera vcam = familiarCameras[familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 0;
        leapOfFaithCamera.Priority = 1;
    }

    public void EndLeapOfFaith() {
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        familiarScript.leapOfFaith = false;
        CinemachineVirtualCamera vcam = familiarCameras[familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 1;
        leapOfFaithCamera.Priority = 0;
    }

    //**************************************************************************************

    //FLOATING ISLAND CAMERA
    //**************************************************************************************
    public void FloatingIslandCameraSwitch(CinemachineVirtualCamera cameraToSwitchTo, FloatingIslandScript floatingIsland)
    {
        CinemachineVirtualCamera vcam = familiarCameras[familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 0;
        cameraToSwitchTo.Priority = 1;
        floatingIsland.cameraswitched = true;
        //floatingIsland.RaiseIsland();
    }

    public void FloatingIslandCameraReturn(CinemachineVirtualCamera cameraToSwitchFrom)
    {
        CinemachineVirtualCamera vcam = weaverCameras[weaverCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 1;
        cameraToSwitchFrom.Priority = 0;
    }

    public void ShakeCameraWeaver(float intensity, float freq, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
        currentCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = freq;
        startingIntensity = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
        Gamepad.current.SetMotorSpeeds(intensity, 0f);
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                // Timer Over!
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                currentCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                Gamepad.current.SetMotorSpeeds(0f, 0f);

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                    Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }
}
