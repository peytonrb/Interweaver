using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting; //this is for testing 

public class FamiliarScript : MonoBehaviour
{

    [Header("Movement Variables")]
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    public bool depossessing; //True if the familiar is being deposessed
    public bool myTurn; //Responsible for determining if the familiar can move
    public bool leapOfFaith; //Determines if owl familiar is in a leap of faith 
    public bool familiarMovementAbility;//Only used for reading if familiar is using movemeny ability
    


    [Header("character's camera")]
    //Character Rotation values
    //**********************************************************
    public GameObject cameraMaster; //Camera manager reference
    private int vCamRotationState;
    //public CinemachineVirtualCamera virtualCam; //Virtual Camera reference
    //private Vector3 originalVirtualCamRotation; // Original rotation values for the virtual camera
    //private Vector3 originalVirtualCamTransposeOffset; //Virtual Camera original transpose offset values
    //**********************************************************
    private GameMasterScript GM; //This is refrencing the game master script

    [Header("Weave Variables")]
    public float WeaveDistance = 12f;

    [SerializeField]
    //public InputAction NPCInteraction;

    public bool islandisfalling;

    void Awake()
    {
        //references to character components
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        characterController.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        myTurn = false;
        islandisfalling = false;
        depossessing = false;
        leapOfFaith = false;
        familiarMovementAbility = false;

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>(); 
        //originalVirtualCamTransposeOffset = virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        //originalVirtualCamRotation = virtualCam.transform.eulerAngles;
        //transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        Debug.Log("Active Current Position: " + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (myTurn) {
            if (Time.timeScale != 0) { 
                
                if (Input.GetKeyDown(KeyCode.Space)) //this is purely for testing the checkpoint function if it's working properly
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //this is for testing
                
                }
            }
        }        
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Crystal"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.GetComponent<CrystalScript>().TriggerBreak();
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit other) {
        
        if (other.gameObject.CompareTag("Breakable")) // if familiar collides with breakable object while using movement ability
        {
            if (familiarMovementAbility)
            {
                movementScript.Bounce();
                
                if (other.gameObject.TryGetComponent<CrystalScript>(out CrystalScript crystal))
                {
                    crystal.TriggerBreak();
                }
            }
            else
            {
                CameraMasterScript.instance.EndLeapOfFaith();
                Death();
            }
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Leap of Faith Trigger"))
        {
            CameraMasterScript.instance.StartLeapOfFaith();
        }

        else if (collision.gameObject.CompareTag("Kill Area"))
        {
            CameraMasterScript.instance.EndLeapOfFaith();
            Death();
        }

        else if (collision.gameObject.CompareTag("Hazard"))
        {
            Death();
        }

        else if (collision.gameObject.CompareTag("Breakable")) // if familiar collides with breakable object while using movement ability
        {
            if (familiarMovementAbility && !characterController.isGrounded)
            {
                movementScript.Bounce();

                if (collision.gameObject.TryGetComponent<CrystalScript>(out CrystalScript crystal))
                {
                    crystal.TriggerBreak();
                }
            }

        }
        else if (collision.gameObject.tag == "CameraTrigger")
        {
            CameraIndexScript cameraIndexScript = collision.GetComponent<CameraIndexScript>();
            vCamRotationState = cameraIndexScript.cameraIndex;

            CameraMasterScript.instance.SwitchFamiliarCameras(vCamRotationState);

            //ROTATION STATE CHANGES HAVE BEEN MOVED TO CAM ERMASTERSCRIPT~
        }
    }

    public void Depossess() {
        if (myTurn && Time.timeScale != 0 && !islandisfalling) {
            //Move character only if they are on the ground or in leapOfFaith
            if (characterController.isGrounded || leapOfFaith) {     
                if (!leapOfFaith) {
                    Debug.Log("Depossessing");
                    depossessing = true;
                    movementScript.active = false;
                }
            }
        }   
    }

    //This coroutine is for an intentional delay that lasts exactly 1 frame, which starts on the frame the possession button is pressed.
    //This is so the the control for depossessing does not get activated on the same frame.
    public IEnumerator ForcedDelay() {
        yield return new WaitForNextFrameUnit();
        movementScript.active = true;
        myTurn = true;
        StopCoroutine(ForcedDelay());
    }

    public void Death()
    {
        characterController.enabled = false;
        transform.position = GM.FamiliarCheckPointPos;
        characterController.enabled = true;
    }

}
