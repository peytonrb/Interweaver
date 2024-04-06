using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WyvernLookAt : MonoBehaviour
{
    public bool cameraIsActive = false;
    private float dampTime = 3f;
    private GameObject player;
    [HideInInspector] public GameObject wyvern;
    private GameObject wyvernCamera;
    public CinemachineVirtualCamera activeCamera;

    void Start()
    {
        player = this.transform.parent.gameObject;
        wyvernCamera = this.transform.GetChild(0).gameObject;
        StartCoroutine(WaitFrame());
    }

    void LateUpdate()
    {
        if (cameraIsActive) // ensures it only runs while player needs it 
        {
            if (wyvern == null)
            {
                wyvern = GameObject.Find("WyvernBoss");
            }

            // if stored activeCamera is different than actual active camera
            if (player.gameObject.tag == "Player")
            {
                if (activeCamera == null ||
                activeCamera != CameraMasterScript.instance.weaverCameras[CameraMasterScript.instance.weaverCameraOnPriority].GetComponent<CinemachineVirtualCamera>())
                {
                    activeCamera = CameraMasterScript.instance.weaverCameras[CameraMasterScript.instance.weaverCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
                }
            }
            else if (player.gameObject.tag == "Familiar")
            {
                if (activeCamera == null ||
                activeCamera != CameraMasterScript.instance.familiarCameras[CameraMasterScript.instance.familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>())
                {
                    activeCamera = CameraMasterScript.instance.familiarCameras[CameraMasterScript.instance.familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
                }
            }


            if (wyvern != null) // catches if wyvern doesnt exist in the scene
            {
                Vector3 wyvernPos = wyvern.transform.position;
                Vector3 playerPos = player.transform.position;

                wyvernPos.y = 0f;
                playerPos.y = 0f;

                Vector3 direction = (wyvernPos - playerPos).normalized;
                direction.y = 0f;

                Vector3 camPos = player.transform.position + (direction * 20f);
                camPos.y = 30f;

                Vector3 zero = Vector3.zero;
                //transform.position = Vector3.SmoothDamp(transform.position, camPos, ref zero, dampTime);
                Vector3 hitpoint = new Vector3((wyvernPos.x + playerPos.x) / 2f,
                                                (wyvernPos.y + playerPos.y + 34f) / 2f,
                                                (wyvernPos.z + playerPos.z) / 2f);
                transform.GetChild(0).LookAt(hitpoint);
            }
        }
    }

    IEnumerator WaitFrame()
    {
        yield return null;

        if (player.gameObject.tag == "Player")
            activeCamera = CameraMasterScript.instance.weaverCameras[CameraMasterScript.instance.weaverCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
        else if (player.gameObject.tag == "Familiar")
            activeCamera = CameraMasterScript.instance.familiarCameras[CameraMasterScript.instance.familiarCameraOnPriority].GetComponent<CinemachineVirtualCamera>();
    }
}
