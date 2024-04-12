using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoneScript : MonoBehaviour
{
    [Header("References")]
    public CameraMasterScript cameraMasterScript;
    [SerializeField] private CinemachineVirtualCamera myCamera;

    [Header("Modes & Settings")]

    [Header("Generic Variables")]
    private bool tripped; 
    public MovementScript currentCharacter;

    void Start()
    {
        cameraMasterScript = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
    }

    void Update()
    {
        if (tripped)
        {
            if (!gameObject.GetComponent<Collider>().bounds.Contains(currentCharacter.transform.position))
            {
                tripped = false;
                currentCharacter = null;
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!tripped) // if a trigger is newly entered
        {
            if (gameObject.GetComponent<Collider>().bounds.Contains(collider.transform.position))
            {
                MovementScript colliderMovementScript = collider.gameObject.GetComponent<MovementScript>();

                if (colliderMovementScript.active || currentCharacter.gameObject == null)
                {
                        // CHANGE CAM PRIORITY HERE
                        //SetCameraPriority();
                        Debug.Log("My name is " + colliderMovementScript.gameObject);
                        currentCharacter = colliderMovementScript;
                        tripped = true;
                        
                }
            }
        }
        else
        {
            MovementScript colliderMovementScript = collider.gameObject.GetComponent<MovementScript>();

            if (!colliderMovementScript.active)
            {
                Debug.Log("Yowza!");
                tripped = false;
                currentCharacter = null;
            }
        }
    }

    private void SetCameraPriority()
    {
        cameraMasterScript.currentCam.Priority = 0;
        myCamera.Priority = 1;
        //myCamera.Follow = gameObject.transform;
        cameraMasterScript.currentCam = myCamera;
    }
}
