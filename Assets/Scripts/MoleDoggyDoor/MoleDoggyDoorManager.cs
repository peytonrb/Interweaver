using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleDoggyDoorManager : MonoBehaviour
{
    private MoleDoggyDoorScript[] mdds;
    private MoleDoggyDoorScript entrancedoor;
    private GameObject mole;
    private MovementScript moleMovementScript;
    public float enterExitSpeed;
    private int phase;
    
    //public bool enteringDoor;

    // Start is called before the first frame update
    void Start()
    {
        mdds = GetComponentsInChildren<MoleDoggyDoorScript>();
        mole = GameObject.FindGameObjectWithTag("Familiar");
        phase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 1) {
            MoleExitDoor(entrancedoor);
        }
    }

    public void MoveMoleInDirection(int direction, float finalposition, MoleDoggyDoorScript thismoledoorscript) {
        switch (direction) {
            case 1:
                //X Positive
                float newposition = Mathf.MoveTowards(mole.transform.position.x, finalposition, enterExitSpeed * Time.deltaTime);
                mole.transform.position = new Vector3(newposition,transform.position.y,transform.position.z);
                if (mole.transform.position.x >= finalposition) {
                    MoleGoToNewDoor(thismoledoorscript);
                }
            break;
            case 2:
                //X Negative
                newposition = Mathf.MoveTowards(mole.transform.position.x, finalposition, enterExitSpeed * Time.deltaTime);
                mole.transform.position = new Vector3(newposition, transform.position.y, transform.position.z);
                if (mole.transform.position.x <= finalposition) {
                    MoleGoToNewDoor(thismoledoorscript);
                }
            break;
            case 3:
                //Z Positive
                newposition = Mathf.MoveTowards(mole.transform.position.z, finalposition, enterExitSpeed * Time.deltaTime);
                mole.transform.position = new Vector3(transform.position.x, transform.position.y, newposition);
                if (mole.transform.position.z >= finalposition) {
                    MoleGoToNewDoor(thismoledoorscript);
                }
            break;
            case 4:
                //Z Negative
                newposition = Mathf.MoveTowards(mole.transform.position.z, finalposition, enterExitSpeed * Time.deltaTime);
                mole.transform.position = new Vector3(newposition, transform.position.y, newposition);
                if (mole.transform.position.z <= finalposition) {
                    MoleGoToNewDoor(thismoledoorscript);
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
                    switch (mdds[i].rotationState) {
                        case 1:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x-2,mdds[i].gameObject.transform.position.y,mdds[i].gameObject.transform.position.z);
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;
                        case 2:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x+2,mdds[i].gameObject.transform.position.y,mdds[i].gameObject.transform.position.z);
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;

                        case 3:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x,mdds[i].gameObject.transform.position.y,mdds[i].gameObject.transform.position.z-2);
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;

                        case 4:
                            mole.transform.position = new Vector3(mdds[i].gameObject.transform.position.x,mdds[i].gameObject.transform.position.y,mdds[i].gameObject.transform.position.z+2);
                            phase = 1;
                            thismoledoorscript.ResetThisDoor();
                        break;
                    }
                                
                }
            }
    }

    void MoleExitDoor(MoleDoggyDoorScript thismoledoorscript) {

    }
        
}
