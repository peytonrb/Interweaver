using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoneScript : MonoBehaviour
{
    [Header("References")]
    private CameraMasterScript cameraMasterScript;
    [SerializeField] private CinemachineVirtualCamera myWeaverCamera;
    [SerializeField] private CinemachineVirtualCamera myFamiliarCamera;
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

    void Update() // I'd like to apologize for all this nesting, it's so fucking bad omg
    {
        if (tripped)
        {
            if (cameraMasterScript.currentCam.Priority > 0)
            {
                cameraMasterScript.currentCam.Priority = 0;
            }
            
            foreach (GameObject character in currentCharacters)
            {
                if (!bounds.Contains(character.transform.position))
                {
                    tripped = false;
                    if (character.CompareTag("Player"))
                    {
                        myWeaverCamera.Priority = 0;
                    }
                    else
                    {
                        myFamiliarCamera.Priority = 0;
                    }
                    cameraMasterScript.currentCam.Priority = 1;
                    currentCharacters.Remove(character);
                    break; // quite frankly I don't really fully understand why break is needed here, but it snuffs an outta bounds error so yay?
                }

                else
                {
                    if (character.CompareTag("Player") && myWeaverCamera.Priority < 1)
                    {
                        MovementScript movementScript = character.GetComponent<MovementScript>();
                        if (movementScript.active)
                        {
                            myWeaverCamera.Priority = 1;
                        }

                    }
                    else if (character.CompareTag("Familiar") && myFamiliarCamera.Priority < 1)
                    {
                        MovementScript movementScript = character.GetComponent<MovementScript>();
                        if (movementScript.active) // this is all so ugly and I'm so tired aaaa
                        {
                            myFamiliarCamera.Priority = 1; 
                        }
                    }
                }
            }
        }
        else
        {
            // look into this later, if shit gets fucked we may have to bring it back
            /*if (currentCharacters.Count <= 0)
            {
                Debug.Log(gameObject + " SCRONK");
                myWeaverCamera.Priority = 0;
                myFamiliarCamera.Priority = 0; 
            }*/
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
                    Debug.Log(collider.gameObject);
                    SetCameraPriority(collider.gameObject);
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
                myWeaverCamera.Priority = 0;
                Debug.Log("Setting current cam to prio 1 in the on trigger stay function");
                cameraMasterScript.currentCam.Priority = 1;
                currentCharacters.Remove(collider.gameObject);
            }
        }
    }

    private void SetCameraPriority(GameObject newTargetToFollow)
    {
        cameraMasterScript.currentCam.Priority = 0;
        if (newTargetToFollow.CompareTag("Player"))
        {
            myWeaverCamera.Priority = 1;
            myWeaverCamera.Follow = newTargetToFollow.transform;
        }
        else
        {
            myFamiliarCamera.Priority = 1;
            Debug.Log("test");
            myFamiliarCamera.Follow = newTargetToFollow.transform;
        }
    }
}
