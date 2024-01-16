using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIndexScript : MonoBehaviour, ITriggerable
{
    public int cameraIndex;
    [HideInInspector] public bool triggered;
    [HideInInspector] public bool isLoop; //Reference only, set in inspector
    public bool isZaxisTrigger; //If false, then its an X axis trigger
    public bool goingOppositeDirection;
    private Transform weaverTransform; //Put only the weaver's transform
    private Transform familiarTransform; //Put only the familiar's transform
    private FamiliarScript familiarScript; 
    [HideInInspector] private bool enteredFromNorth; //Determines if the player entered from the south side or the north side of the collider

    public void Awake()
    {
        weaverTransform = FindObjectOfType<PlayerController>().GetComponent<Transform>();
        familiarTransform = FindObjectOfType<FamiliarScript>().GetComponent<Transform>();
        familiarScript = FindObjectOfType<FamiliarScript>().GetComponent<FamiliarScript>();

    }

    public void OnTrigEnter(Collider other)
    {
        if (other.gameObject.tag == "LevelTrigger") {
            LevelTriggerScript levelTriggerScript = other.GetComponent<LevelTriggerScript>();
            int section = levelTriggerScript.triggerType;
            
            LevelManagerScript.instance.TurnOnOffSection(section);
        }
    }
    public void OnTrigExit(Collider other)
    {
         if (other.gameObject.tag == "WeaverCameraTrigger" || other.gameObject.tag == "FamiliarCameraTrigger")
         {
            //WEAVER
            if (!familiarScript.myTurn)
            {
                if (!isZaxisTrigger)
                {
                    if (!goingOppositeDirection)
                    {
                        if (transform.position.x > weaverTransform.position.x)
                        {
                            //Enter from north
                            enteredFromNorth = true;
                        }
                        else
                        {
                            enteredFromNorth = false;
                        }
                    }
                    else
                    {
                        if (transform.position.x > weaverTransform.position.x)
                        {
                            //Enter from north
                            enteredFromNorth = false;
                        }
                        else
                        {
                            enteredFromNorth = true;
                        }
                    }
                }
                else
                {
                    if (!goingOppositeDirection)
                    {
                        if (transform.position.z > weaverTransform.position.z)
                        {
                            enteredFromNorth = false;
                        }
                        else
                        {
                            enteredFromNorth = true;
                        }
                    }
                    else
                    {
                        if (transform.position.z > weaverTransform.position.z)
                        {
                            enteredFromNorth = true;
                        }
                        else
                        {
                            enteredFromNorth = false;
                        }
                    }

                }
                if (isZaxisTrigger)
                {
                    if (!goingOppositeDirection)
                    {
                        if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                    }

                }
                else
                {
                    if (!goingOppositeDirection)
                    {
                        if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(cameraIndex);
                        }
                    }

                }
            }
            //FAMILIAR
            else
            {
                if (!isZaxisTrigger)
                {
                    if (!goingOppositeDirection)
                    {
                        if (transform.position.x > familiarTransform.position.x)
                        {
                            //Enter from north
                            enteredFromNorth = true;
                        }
                        else
                        {
                            enteredFromNorth = false;
                        }
                    }
                    else
                    {
                        if (transform.position.x > familiarTransform.position.x)
                        {
                            //Enter from north
                            enteredFromNorth = false;
                        }
                        else
                        {
                            enteredFromNorth = true;
                        }
                    }
                }
                else
                {
                    if (!goingOppositeDirection)
                    {
                        if (transform.position.z > familiarTransform.position.z)
                        {
                            enteredFromNorth = false;
                        }
                        else
                        {
                            enteredFromNorth = true;
                        }
                    }
                    else
                    {
                        if (transform.position.z > familiarTransform.position.z)
                        {
                            enteredFromNorth = true;
                        }
                        else
                        {
                            enteredFromNorth = false;
                        }
                    }

                }
                if (isZaxisTrigger)
                {
                    if (!goingOppositeDirection)
                    {
                        if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                    }

                }
                else
                {
                    if (!goingOppositeDirection)
                    {
                        if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
                        }
                    }

                }
            }

            //ROTATION STATE CHANGES HAVE BEEN MOVED TO CAMERMASTERSCRIPT~
         }
    }

         
}
