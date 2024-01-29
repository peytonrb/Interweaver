using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleDoggyDoorManager : MonoBehaviour
{
    private MoleDoggyDoorScript[] mdds;
    private MoleDoggyDoorScript entrancedoor;
    private MoleDoggyDoorScript exitdoor;
    private GameObject mole;
    private MovementScript moleMovementScript;
    public float enterExitSpeed;
    private float exitfinalposition;
    private bool foundexitdoor;
    private int phase;
    
    //public bool enteringDoor;

    // Start is called before the first frame update
    void Start()
    {
        mdds = GetComponentsInChildren<MoleDoggyDoorScript>();
        mole = GameObject.FindGameObjectWithTag("Familiar");
        moleMovementScript = mole.GetComponent<MovementScript>();
        foundexitdoor = false;
        phase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 1 || phase == 2) {
            MoleExitDoor(entrancedoor);
        }
    }

    public void MoveMoleInDirection(int direction, float finalposition, MoleDoggyDoorScript thismoledoorscript) {
        switch (phase) {
            case 0:
                //Disables exit door's collider
                if (foundexitdoor == false) {
                    for (int i = 0; i < mdds.Length; i++) {
                        if (mdds[i] != thismoledoorscript) {
                            exitdoor = mdds[i];
                            BoxCollider[] bc = exitdoor.GetComponents<BoxCollider>();
                            for (int x = 0; x < bc.Length; x++) {
                                bc[x].enabled = false;
                            }
                            foundexitdoor = true;
                        }
                    }
                }
                
                switch (direction) {
                    case 1:
                        //X Positive
                        float newposition = Mathf.MoveTowards(mole.transform.position.x, finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(newposition,mole.transform.position.y,mole.transform.position.z);
                        if (mole.transform.position.x >= finalposition) {
                            MoleGoToNewDoor(thismoledoorscript);
                        }
                    break;
                    case 2:
                        //X Negative
                        newposition = Mathf.MoveTowards(mole.transform.position.x, finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(newposition, mole.transform.position.y, mole.transform.position.z);
                        if (mole.transform.position.x <= finalposition) {
                            MoleGoToNewDoor(thismoledoorscript);
                        }
                    break;
                    case 3:
                        //Z Positive
                        newposition = Mathf.MoveTowards(mole.transform.position.z, finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(mole.transform.position.x, mole.transform.position.y, newposition);
                        if (mole.transform.position.z >= finalposition) {
                            MoleGoToNewDoor(thismoledoorscript);
                        }
                    break;
                    case 4:
                        //Z Negative
                        newposition = Mathf.MoveTowards(mole.transform.position.z, finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(mole.transform.position.x, mole.transform.position.y, newposition);
                        if (mole.transform.position.z <= finalposition) {
                            MoleGoToNewDoor(thismoledoorscript);
                        }
                    break;
                }
            break;
            //Exiting
            case 2:
                switch (direction) {
                    case 1:
                        float newposition = Mathf.MoveTowards(mole.transform.position.x,finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(newposition,mole.transform.position.y,mole.transform.position.z);
                        mole.transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
                        if (mole.transform.position.x <= finalposition) {
                            //End mole enter exit transition
                            moleMovementScript.ToggleCanMove(true);
                            moleMovementScript.ToggleCanLook(true);
                            thismoledoorscript.ResetThisDoor();
                            phase = 0;
                            foundexitdoor = false;
                        }
                    break;
                    case 2:
                        newposition = Mathf.MoveTowards(mole.transform.position.x,finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(newposition,mole.transform.position.y,mole.transform.position.z);
                        mole.transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
                        if (mole.transform.position.x >= finalposition) {
                            //End mole enter exit transition
                            moleMovementScript.ToggleCanMove(true);
                            moleMovementScript.ToggleCanLook(true);
                            thismoledoorscript.ResetThisDoor();
                            phase = 0;
                            foundexitdoor = false;
                        }
                    break;
                    case 3:
                        newposition = Mathf.MoveTowards(mole.transform.position.z, finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(mole.transform.position.x,mole.transform.position.y,newposition);
                        mole.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                        if (mole.transform.position.z <= finalposition) {
                            //End mole enter exit transition
                            moleMovementScript.ToggleCanMove(true);
                            moleMovementScript.ToggleCanLook(true);
                            thismoledoorscript.ResetThisDoor();
                            phase = 0;
                            foundexitdoor = false;
                        }
                    break;
                    case 4:
                        newposition = Mathf.MoveTowards(mole.transform.position.z, finalposition, enterExitSpeed * Time.deltaTime);
                        mole.transform.position = new Vector3(mole.transform.position.x,mole.transform.position.y,newposition);
                        mole.transform.rotation = Quaternion.Euler(Vector3.zero);
                        if (mole.transform.position.z >= finalposition) {
                            //End mole enter exit transition
                            moleMovementScript.ToggleCanMove(true);
                            moleMovementScript.ToggleCanLook(true);
                            thismoledoorscript.ResetThisDoor();
                            phase = 0;
                            foundexitdoor = false;
                        }
                    break;
                } 
            break;
        }
        
    }

    public void MoleGoToNewDoor(MoleDoggyDoorScript thismoledoorscript) {
            //Mole teleports to new door
            for (int i = 0; i < mdds.Length; i++) {
                //This is the 2nd door
                if (mdds[i] != thismoledoorscript) {
                    entrancedoor = thismoledoorscript;
                    //Trust me, dear
                    //This moves the mole to the new door.
                    switch (mdds[i].rotationState) {
                        case 1:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x,mdds[i].gameObject.transform.position.y-1.5f,mdds[i].gameObject.transform.position.z);
                            
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;
                        case 2:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x,mdds[i].gameObject.transform.position.y-1.5f,mdds[i].gameObject.transform.position.z);
                            
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;

                        case 3:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x,mdds[i].gameObject.transform.position.y-1.5f,mdds[i].gameObject.transform.position.z);
                            
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;

                        case 4:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x,mdds[i].gameObject.transform.position.y-1.5f,mdds[i].gameObject.transform.position.z);
                            
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;
                    }
                                
                }
            }
    }

    void MoleExitDoor(MoleDoggyDoorScript entrancemoledoor) {
        switch (phase) {
            case 1:
                for (int i = 0; i < mdds.Length; i++) {
                    if (mdds[i] != entrancemoledoor) {
                        exitdoor = mdds[i];

                        BoxCollider[] bc = mdds[i].gameObject.GetComponents<BoxCollider>();
                        for (int x = 0; x < bc.Length; x++) {
                            bc[x].enabled = false;
                        }

                        switch (mdds[i].rotationState) {
                            case 1:
                                exitfinalposition = mole.transform.position.x - 3f;
                                phase += 1;
                            break;
                            case 2:
                                exitfinalposition = mole.transform.position.x + 3f;
                                phase += 1;
                            break;
                            case 3:
                                exitfinalposition = mole.transform.position.z - 3f;
                                phase += 1;
                            break;
                            case 4:
                                exitfinalposition = mole.transform.position.z + 3f;
                                phase += 1;
                            break;
                        }
                    }
                }
            break;
            case 2:
                MoveMoleInDirection(exitdoor.rotationState, exitfinalposition, exitdoor);
            break;
        }
        
    }
        
}
