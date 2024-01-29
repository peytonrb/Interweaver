using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleDoggyDoorScript : MonoBehaviour
{
    public int rotationState;
    private bool enterdoor;
    private MoleDoggyDoorManager mddmanager;
    private GameObject mole;
    private MovementScript moleMovementScript;
    private int phase;
    private float finalposition;

    // Start is called before the first frame update
    void Start()
    {
        mddmanager = GetComponentInParent<MoleDoggyDoorManager>();
        mole = GameObject.FindGameObjectWithTag("Familiar");
        moleMovementScript = mole.GetComponent<MovementScript>();

        enterdoor = false;
        phase = 0;

        if (transform.localScale.x < transform.localScale.z) {
            BoxCollider[] bc = GetComponents<BoxCollider>();
            for (int i = 0; i < bc.Length; i++) {
                if (bc[i].center.x > 0) {
                    //X Negative Direction
                    rotationState = 2;
                    Debug.Log(rotationState);
                }
                else if (bc[i].center.x < 0) {
                    //X Positive Direction
                    rotationState = 1;
                    Debug.Log(rotationState);
                }
            }
        }

        else if (transform.localScale.z < transform.localScale.x) {
            BoxCollider[] bc = GetComponents<BoxCollider>();
            for (int i = 0; i < bc.Length; i++) {
                if (bc[i].center.z > 0) {
                    //Z Negative Direction
                    rotationState = 4;
                    Debug.Log(rotationState);
                }
                else if (bc[i].center.z < 0) {
                    //Z Positive Direction
                    rotationState = 3;
                    Debug.Log(rotationState);
                }
                
            }
        }
    }

    void Update() {
        if (enterdoor) {
            EnterDoggyDoor();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Familiar")) {
            moleMovementScript.ToggleCanMove(false);
            moleMovementScript.ToggleCanLook(false);
            enterdoor = true;
        }
    }

    void EnterDoggyDoor() {
        switch (phase) {
            case 0:
                //Disables the colliders so the mole can go through the door.
                BoxCollider[] bc = GetComponents<BoxCollider>();
                for (int i = 0; i < bc.Length; i++) {
                    bc[i].enabled = false;
                }

                //Puts mole directly in front of the door before entering.
                switch (rotationState) {
                    case 1:
                        //X Positive
                        mole.transform.position = new Vector3(mole.transform.position.x, mole.transform.position.y, transform.position.z);
                        mole.transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
                        finalposition = mole.transform.position.x + 2f;
                        phase += 1;
                    break;
                    case 2:
                        //X Negative
                        mole.transform.position = new Vector3(mole.transform.position.x, mole.transform.position.y, transform.position.z);
                        mole.transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
                        finalposition = mole.transform.position.x - 2f;
                        phase += 1;
                    break;
                    case 3:
                        //Z Positive
                        mole.transform.position = new Vector3(transform.position.x, mole.transform.position.y, mole.transform.position.z);
                        mole.transform.rotation = Quaternion.Euler(Vector3.zero);
                        finalposition = mole.transform.position.z + 2f;
                        phase += 1;
                    break;
                    case 4:
                        //Z Negative
                        mole.transform.position = new Vector3(transform.position.x, mole.transform.position.y, mole.transform.position.z);
                        mole.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                        finalposition = mole.transform.position.z - 2f;
                        phase += 1;
                    break;
                }
            break;
            case 1:
                mddmanager.MoveMoleInDirection(rotationState, finalposition, GetComponent<MoleDoggyDoorScript>());
            break;
        }
            
    }
    public void ResetThisDoor() {
        BoxCollider[] bc = GetComponents<BoxCollider>();
        for (int i = 0; i < bc.Length; i++) {
            bc[i].enabled = true;
        }
        enterdoor = false;
        phase = 0;
    }
        
}
