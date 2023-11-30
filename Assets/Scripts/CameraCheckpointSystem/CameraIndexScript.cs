using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIndexScript : MonoBehaviour, ITriggerable
{
    public int cameraIndex;
    public bool triggered;
    public bool isLoop; //Reference only, set in inspector
    public bool isZaxisTrigger; //If false, then its an X axis trigger
    public bool goingOppositeDirection;
    public Transform weaverTransform; //Put only the weaver's transform
    public Transform familiarTransform; //Put only the familiar's transform
    private FamiliarScript familiarScript; 
    [Header("Cutscene")]
    [CannotBeNullObjectField] public GameObject[] cutsceneManager; //Set to 0 if there are no cutscenes
    [HideInInspector]
    public bool enteredFromNorth; //Determines if the player entered from the south side or the north side of the collider

    public void Awake()
    {
        weaverTransform = FindObjectOfType<PlayerController>().GetComponent<Transform>();
        familiarTransform = FindObjectOfType<FamiliarScript>().GetComponent<Transform>();
        familiarScript = FindObjectOfType<FamiliarScript>().GetComponent<FamiliarScript>();

    }

    public void OnTrigEnter(Collider other)
    {
        if (other.gameObject.tag == "CutsceneTrigger") {
            //Only the trigger that is a child of a certain cutscene manager will activate a cutscene.
            foreach (GameObject cm in cutsceneManager) {
                CutsceneManagerScript cms = cm.GetComponent<CutsceneManagerScript>();
                cms.StartCutscene();
            }     
        }
        if (other.gameObject.tag == "LevelTrigger") {
            LevelTriggerScript levelTriggerScript = other.GetComponent<LevelTriggerScript>();
            int section = levelTriggerScript.triggerType;
            
            LevelManagerScript.instance.TurnOnOffSection(section);
        }
    }
    public void OnTrigExit(Collider other)
    {
         if (other.gameObject.tag == "CameraTrigger")
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
