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
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    private InputAction possessInput; //Input for depossessing
    private bool depossess; //Only used for reading if depossessing
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
    //public InputAction NPCInteraction;

    public bool islandisfalling;
    private bool movementInactive;

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
        movementInactive = false;

        //Section reserved for initiating inputs 
        familiarMovementAbilityInput = inputs.FindAction("Player/Familiar Movement Ability");
        possessInput = inputs.FindAction("Player/Switch");

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
        //NPCInteraction.Enable();
    }

    void OnDisable() {
        inputs.Disable();
        //NPCInteraction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (myTurn) {
            if (Time.timeScale != 0) {
                if (!islandisfalling) {
                    if (movementInactive) {
                        movementScript.active = true;
                    }
                    //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad.
                    depossess = possessInput.WasPressedThisFrame();
                }
                if (islandisfalling) {
                    movementScript.active = false;
                    movementInactive = true;
                }
                
                familiarMovementAbility = familiarMovementAbilityInput.IsPressed();
                
                //Move character only if they are on the ground or in leapOfFaith
                if (characterController.isGrounded || leapOfFaith) {
                    
                    if (depossess && !leapOfFaith) {
                        Debug.Log("Depossessing");
                        depossessing = true;
                        movementScript.active = false;
                    }
                }

                /*
                if (NPCInteraction.WasPressedThisFrame()) //this is the interact button that is taking from the player inputs
                {
                    PlayerScript playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
                    playerScript.Interact();
                    Debug.Log("interact button was pressed"); //a general debug to see if the input was pressed
                }
                */

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

    //This coroutine is for an intentional delay that lasts exactly 1 frame, which starts on the frame the possession button is pressed.
    //This is so the the control for depossessing does not get activated on the same frame.
    public IEnumerator ForcedDelay() {
        yield return new WaitForNextFrameUnit();
        movementScript.active = true;
        myTurn = true;
        StopCoroutine(ForcedDelay());
    }
}
