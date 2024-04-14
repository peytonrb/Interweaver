using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoneScript : MonoBehaviour
{
    [Header("References")]
    public CameraMasterScript cameraMasterScript;
    [SerializeField] private CinemachineVirtualCamera myCamera;
    private Bounds bounds;

    [Header("Modes & Settings")]

    [Header("Generic Variables")]
    private bool tripped; 
    public List<GameObject> currentCharacters = new List<GameObject>();

    void Start()
    {
        cameraMasterScript = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
        bounds = gameObject.GetComponent<Collider>().bounds;
    }

    void Update()
    {
        if (tripped)
        {
            foreach (GameObject character in currentCharacters)
            {
                if (!bounds.Contains(character.transform.position))
                {
                    tripped = false;
                    currentCharacters.Remove(character);
                    break; // quite frankly I don't really fully understand why break is needed here, but it snuffs an outta bounds error so yay?
                }
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!tripped) // if a trigger is newly entered
        {
            if (bounds.Contains(collider.transform.position))
            {
                MovementScript colliderMovementScript = collider.gameObject.GetComponent<MovementScript>();

                if (colliderMovementScript.active && !currentCharacters.Contains(collider.gameObject))
                {
                        // CHANGE CAM PRIORITY HERE
                        //SetCameraPriority();
                        Debug.Log("My name is " + colliderMovementScript.gameObject);
                        currentCharacters.Add(collider.gameObject);
                        tripped = true;
                }
            }
        }
        else
        {
            MovementScript colliderMovementScript = collider.gameObject.GetComponent<MovementScript>();

            if (!colliderMovementScript.active && currentCharacters.Contains(colliderMovementScript.gameObject))
            {
                tripped = false;
                currentCharacters.Remove(collider.gameObject);
            }
        }
    }

    private void SetCameraPriority()
    {
        cameraMasterScript.currentCam.Priority = 0;
        myCamera.Priority = 1;
        myCamera.Follow = gameObject.transform;
        cameraMasterScript.currentCam = myCamera;
    }
}
