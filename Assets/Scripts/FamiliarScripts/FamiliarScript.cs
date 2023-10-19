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

    public int playerIndex; //Set to 0 for the weaver and 1 for the familiar

    [Header("Movement Variables")]
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    public GameObject inputManager;
    private InputManagerScript inputManagerScript;
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    //private InputAction possessInput; //Input for depossessing
    public bool depossessing;
    public bool myTurn; //Responsible for determining if the familiar can move
    private bool leapOfFaith; //Determines if owl familiar is in a leap of faith
    private InputAction familiarMovementAbilityInput;//Input for movement ability
    private bool familiarMovementAbility;//Only used for reading if familiar is using movemeny ability


    [Header("character's camera")]
    //Character Rotation values
    //**********************************************************
    public GameObject cam; //Camera object reference
    public CinemachineVirtualCamera virtualCam; //Virtual Camera reference
    private Vector3 originalVirtualCamRotation; // Original rotation values for the virtual camera
    private Vector3 originalVirtualCamTransposeOffset; //Virtual Camera original transpose offset values
    //**********************************************************
    private GameMasterScript GM; //This is refrencing the game master script

    [Header ("Floating Island")]
    public GameObject[] floatingIsland;
    private int crystalIndexRef;


    [Header("Weave Variables")]
    public float WeaveDistance = 12f;

    [SerializeField]
    private InputAction interactInput;

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
        inputManagerScript = inputManager.GetComponent<InputManagerScript>();
        myTurn = false;
        islandisfalling = false;
        depossessing = false;
        leapOfFaith = false;

        //Section reserved for initiating inputs 
        interactInput = inputs.FindAction("Player/Interact");
        familiarMovementAbilityInput = inputs.FindAction("Player/Familiar Movement Ability");
        //possessInput = inputs.FindAction("Player/Switch");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>(); 
        originalVirtualCamTransposeOffset = virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        originalVirtualCamRotation = virtualCam.transform.eulerAngles;
        //transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        Debug.Log("Active Current Position: " + transform.position);
    }

    void OnEnable() {
        inputs.Enable();
        
    }

    void OnDisable() {
        inputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (myTurn) {
            if (Time.timeScale != 0) { 
                familiarMovementAbility = familiarMovementAbilityInput.IsPressed();
                
                if (interactInput.WasPressedThisFrame()) //this is the interact button that is taking from the player inputs
                {
                    Debug.Log("interact button was pressed"); //a general debug to see if the input was pressed
                }

                if (Input.GetKeyDown(KeyCode.Space)) //this is purely for testing the checkpoint function if it's working properly
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //this is for testing
                
                }
            }
        }        
    }

    private void OnControllerColliderHit(ControllerColliderHit other) {
        //Familiar collides with Crystal (Can add input later if needed)
        if (other.gameObject.tag == "Crystal") {
            CrystalScript crystalScript = other.gameObject.GetComponent<CrystalScript>();
            crystalIndexRef = crystalScript.crystalIndex;
            FloatingIslandScript FIScript = floatingIsland[crystalIndexRef].GetComponent<FloatingIslandScript>();
            //Floating island starts falling
            FIScript.StartFalling();
            FIScript.isislandfalling = true;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Breakable")) // if familiar collides with breakable object while using movement ability
        {
            if (familiarMovementAbility)
            {
                Destroy(other.gameObject); // TEMPORARY, in future, do something like this on the object's end
            }
            else
            {
                EndLeapOfFaith();
                characterController.enabled = false;
                transform.position = GM.LastCheckPointPos;
                characterController.enabled = true;
            }
        }
    }

    private void StartLeapOfFaith()
    {
        leapOfFaith = true;
        virtualCam.transform.eulerAngles = new Vector3(90f, virtualCam.transform.eulerAngles.y, virtualCam.transform.eulerAngles.z);
        virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, originalVirtualCamTransposeOffset.y, 0);
    }

    private void EndLeapOfFaith()
    {
        leapOfFaith = false;
        virtualCam.transform.eulerAngles = originalVirtualCamRotation;
        virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = originalVirtualCamTransposeOffset;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Leap of Faith Trigger"))
        {
            StartLeapOfFaith();
        }

        else if (collision.gameObject.CompareTag("Kill Area"))
        {
            EndLeapOfFaith();
            characterController.enabled = false;
            transform.position = GM.LastCheckPointPos;
            characterController.enabled = true;
        }

        else if (collision.gameObject.CompareTag("Hazard")) {
            Destroy(collision.gameObject);
            characterController.enabled = false;
            transform.position = GM.LastCheckPointPos;
            characterController.enabled = true;
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
}
