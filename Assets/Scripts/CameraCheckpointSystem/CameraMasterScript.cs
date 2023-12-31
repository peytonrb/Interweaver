using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraMasterScript : MonoBehaviour
{
    //Singleton
    public static CameraMasterScript instance;

    private CinemachineVirtualCamera currentCam;

    //Weaver Camera + Checkpoints
    public GameObject[] weaverCheckpoints; //Put all weaver checkpoints here.
    public CinemachineVirtualCamera[] weaverVirtualCams; //For all cameras that are around the weaver
    //public bool[] weaverCamerasTriggered; //Determines if the cameras have been triggered for weaver side
    private int weaverCamerasTriggeredSinceLastCheckpoint;
    [HideInInspector] public int lastWeaverCameraTriggered;
    
    //Familiar Camera + Checkpoints
    public GameObject[] familiarCheckpoints; //Put all familiar checkpoints here.
    public CinemachineVirtualCamera[] familiarVirtualCams; //For all cameras that are around the familiar
    //[HideInInspector] public bool[] familiarCamerasTriggered; //Determines if the cameras have been triggered for familiar side
    private int familiarCamerasTriggeredSinceLastCheckpoint;
    [HideInInspector] public int lastFamiliarCameraTriggered;

    private Vector3 originalFamiliarCamRotation;
    private Vector3 originalFamiliarCamTransposeOffset;
    
    private int weaverVcamListLength;
    private int familiarVcamListLength;
    public int weaverCameraOnPriority;
    public int familiarCameraOnPriority;

    public CinemachineVirtualCamera leapOfFaithCamera;
    public float LoFCameraYoffset = 0;

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
        originalFamiliarCamTransposeOffset = familiarVirtualCams[0].GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        originalFamiliarCamRotation = familiarVirtualCams[0].transform.eulerAngles;

        //weaverCamerasTriggered = new bool[weaverCheckpoints.Length];
        //familiarCamerasTriggered = new bool[familiarCheckpoints.Length];
        weaverCamerasTriggeredSinceLastCheckpoint = 0;
        familiarCamerasTriggeredSinceLastCheckpoint = 0;
        lastWeaverCameraTriggered = 0;
        lastFamiliarCameraTriggered = 0;

    }

    //WEAVER CAMERAS
    //**************************************************************************************
    public void SwitchWeaverCameras(int rotationstate) {
        weaverVcamListLength = weaverVirtualCams.Length;
        CameraIndexScript cis = weaverCheckpoints[rotationstate].GetComponent<CameraIndexScript>();

        for (int i = 0; i < weaverVcamListLength; i++) {
            if (rotationstate == i) {
                Debug.Log(rotationstate);
                if (cis.triggered == false) {
                    weaverVirtualCams[i].Priority = 0;
                    if (i < weaverVcamListLength - 1) {
                        weaverVirtualCams[i+1].Priority = 1;
                        weaverCameraOnPriority = i+1;
                    }
                    else {
                        weaverVirtualCams[0].Priority = 1;
                        weaverCameraOnPriority = 0;
                    }
                    
                    cis.triggered = true;
                    //weaverCamerasTriggered[rotationstate] = true;
                    weaverCamerasTriggeredSinceLastCheckpoint += 1;
                    
                    if (i == 0) {
                        ResetWeaverIsLoop();
                    }
                    else if (i == weaverVcamListLength - 1) {
                        ResetWeaverCameras();
                    }
                } 
                else {
                    weaverVirtualCams[i].Priority = 1;
                    if (i < weaverVcamListLength - 1) {
                        weaverVirtualCams[i+1].Priority = 0;
                    }
                    else {
                        weaverVirtualCams[0].Priority = 0;
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
        
        for (int i = rotationstate; i < cameraIndexTriggersToReset; i++) {
            CameraIndexScript cis = weaverCheckpoints[i].GetComponent<CameraIndexScript>();
            weaverVirtualCams[rotationstate].Priority = 1;
            if (i > rotationstate) {
                weaverVirtualCams[i].Priority = 0;
                
            }
            cis.triggered = false;
        }
    }

    public void SwitchToFamiliarCamera() {
       for(int i = 0; i < weaverVirtualCams.Length; i++) {
            weaverVirtualCams[i].Priority = 0;
        }
        familiarVirtualCams[familiarCameraOnPriority].Priority = 1; 
    }

    public void SwitchToWeaverCamera() {
        for(int i = 0; i < familiarVirtualCams.Length; i++) {
            familiarVirtualCams[i].Priority = 0;
        }
        weaverVirtualCams[weaverCameraOnPriority].Priority = 1;
        
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

    public void ResetWeaverCamerasTriggered() {
        weaverCamerasTriggeredSinceLastCheckpoint = 0;
        lastWeaverCameraTriggered = weaverCameraOnPriority;
    }
    //**************************************************************************************


    //FAMILIAR CAMERA
    //**************************************************************************************
    public void SwitchFamiliarCameras(int rotationstate) {
        familiarVcamListLength = familiarVirtualCams.Length;
        CameraIndexScript cis = familiarCheckpoints[rotationstate].GetComponent<CameraIndexScript>();

        for (int i = 0; i < familiarVcamListLength; i++) {
            if (rotationstate == i) {
                Debug.Log(rotationstate);
                if (cis.triggered == false) {
                    familiarVirtualCams[i].Priority = 0;
                    if (i < familiarVcamListLength - 1) {
                        familiarVirtualCams[i+1].Priority = 1;
                        familiarCameraOnPriority = i+1;
                    }
                    else {
                        familiarVirtualCams[0].Priority = 1;
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
                    familiarVirtualCams[i].Priority = 1;
                    if (i < familiarVcamListLength - 1) {
                        familiarVirtualCams[i+1].Priority = 0;
                    }
                    else {
                        familiarVirtualCams[0].Priority = 0;
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
        
        for (int i = rotationstate; i < cameraIndexTriggersToReset; i++) {
            CameraIndexScript cis = familiarCheckpoints[i].GetComponent<CameraIndexScript>();
            familiarVirtualCams[rotationstate].Priority = 1;
            if (i > rotationstate) {
                familiarVirtualCams[i].Priority = 0;   
            }
            cis.triggered = false;
            
        }
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

    public void ResetFamiliarCamerasTriggered() {
        familiarCamerasTriggeredSinceLastCheckpoint = 0;
        lastFamiliarCameraTriggered = familiarCameraOnPriority;
    }

    public void StartLeapOfFaith() {
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        familiarScript.leapOfFaith = true;
        familiarVirtualCams[familiarCameraOnPriority].Priority = 0;
        leapOfFaithCamera.Priority = 1;
        //familiarVirtualCams[familiarCameraOnPriority].transform.eulerAngles = new Vector3(90f, familiarVirtualCams[familiarCameraOnPriority].transform.eulerAngles.y, familiarVirtualCams[familiarCameraOnPriority].transform.eulerAngles.z);
        //familiarVirtualCams[familiarCameraOnPriority].GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, originalFamiliarCamTransposeOffset.y + LoFCameraYoffset, 0);
    }

    public void EndLeapOfFaith() {
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        familiarScript.leapOfFaith = false;
        familiarVirtualCams[familiarCameraOnPriority].Priority = 1;
        leapOfFaithCamera.Priority = 0;
        //familiarVirtualCams[familiarCameraOnPriority].transform.eulerAngles = originalFamiliarCamRotation;
        //familiarVirtualCams[familiarCameraOnPriority].GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = originalFamiliarCamTransposeOffset;
    }

    //**************************************************************************************

    //FLOATING ISLAND CAMERA
    //**************************************************************************************
    public void FloatingIslandCameraSwitch(CinemachineVirtualCamera cameraToSwitchTo, FloatingIslandScript floatingIsland)
    {

        familiarVirtualCams[familiarCameraOnPriority].Priority = 0;
        cameraToSwitchTo.Priority = 1;
        floatingIsland.cameraswitched = true;
        //floatingIsland.RaiseIsland();
    }

    public void FloatingIslandCameraReturn(CinemachineVirtualCamera cameraToSwitchFrom)
    {

        weaverVirtualCams[weaverCameraOnPriority].Priority = 1;
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
