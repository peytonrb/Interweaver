using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraIndexScript : MonoBehaviour, ITriggerable
{
    public int triggerIndex;
    [HideInInspector] public bool triggered;
    [HideInInspector] public bool isLoop; //Reference only, set in inspector
    private bool isZaxisTrigger; //If false, then its an X axis trigger
    public bool goingOppositeDirection;
    [HideInInspector] public CinemachineVirtualCamera indexCamera;
    private Transform weaverTransform; //Put only the weaver's transform
    private Transform familiarTransform; //Put only the familiar's transform
    private FamiliarScript familiarScript; 
    private BoxCollider bc;
    private bool addedToList;
    private MolePillarScript molePillarScript;
    private bool enteredFromNorth; //Determines if the player entered from the south side or the north side of the collider

    public void Awake()
    {
        weaverTransform = FindObjectOfType<PlayerControllerNew>().GetComponent<Transform>();
        familiarTransform = FindObjectOfType<FamiliarScript>().GetComponent<Transform>();
        familiarScript = FindObjectOfType<FamiliarScript>().GetComponent<FamiliarScript>();
        indexCamera = gameObject.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        molePillarScript = FindObjectOfType<MolePillarScript>().GetComponent<MolePillarScript>();
        bc = GetComponent<BoxCollider>();
        addedToList = false;

        if (bc.size.x > bc.size.z) {
            isZaxisTrigger = true;
        }
        else {
            isZaxisTrigger = false;
        }
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
         if (other.gameObject.tag == "WeaverCameraTrigger")
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
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                    }

                }
                else
                {
                    if (!goingOppositeDirection)
                    {
                        if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchWeaverCameras(triggerIndex);
                        }
                    }

                }
            }
        }

        if (other.gameObject.tag == "FamiliarCameraTrigger") {
            //FAMILIAR
            if (familiarScript.myTurn)
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
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                    }

                }
                else
                {
                    if (!goingOppositeDirection)
                    {
                        if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                        else if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                    }
                    else
                    {
                        if (triggered && !enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                        else if (!triggered && enteredFromNorth)
                        {
                            CameraMasterScript.instance.SwitchFamiliarCameras(triggerIndex);
                            molePillarScript.familiarCamera = indexCamera;
                        }
                    }

                }
            }

            //ROTATION STATE CHANGES HAVE BEEN MOVED TO CAMERMASTERSCRIPT~   
        }
    }
    
    void Update() {
        if (!addedToList) {
            AddToCameraMaster();
        }
    }
    
    public void AddToCameraMaster() {
        if (gameObject.CompareTag("WeaverCameraTrigger")) {
            //Adds element to the very end of the list and makes the trigger index match its spot in the list.
            if (!addedToList) {
                if (CameraMasterScript.instance.weaverCheckpoints.Count == triggerIndex) {
                    GameObject camerachild = gameObject.transform.GetChild(0).gameObject;
                    
                    CameraMasterScript.instance.weaverCheckpoints.Add(gameObject);
                    CameraMasterScript.instance.weaverCameras.Add(camerachild);
                    addedToList = true;
                }
            }
        }
        else if (gameObject.CompareTag("FamiliarCameraTrigger")) {
            if (!addedToList) {
                if (CameraMasterScript.instance.familiarCheckpoints.Count == triggerIndex) {
                    GameObject camerachild = gameObject.transform.GetChild(0).gameObject;

                    CameraMasterScript.instance.familiarCheckpoints.Add(gameObject);
                    CameraMasterScript.instance.familiarCameras.Add(camerachild);
                    addedToList = true;
                }
            }
        }
    }
         
}
