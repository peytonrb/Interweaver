using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; //this is for testing 

public class FamiliarScript : MonoBehaviour
{

    public int playerIndex; //Set to 0 for the weaver and 1 for the familiar

    [Header("Movement Variables")]
    public float speed; //Base walk speed for player
    private CharacterController characterController; //references the character controller component
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    private InputAction moveInput; //Is the specific input action regarding arrow keys, WASD, and left stick
    private InputAction possessInput; //Input for depossessing
    private Vector2 movement; //Vector2 regarding movement, which is set to track from moveInput's Vector2
    private bool depossess; //Only used for reading if depossessing
    public bool myTurn; //Responsible for determining if the familiar can move
    public bool isPaused; //Determines if the game is paused
    private bool leapOfFaith; //Determines if owl familiar is in a leap of faith
    private InputAction familiarMovementAbilityInput;//Input for movement ability
    private bool familiarMovementAbility;//Only used for reading if familiar is using movemeny ability


    [Header("character's camera")]
    //Character Rotation values
    //**********************************************************
    private float rotationSpeed; 
    private float rotationVelocity;
    private Vector3 newDirection;
    public GameObject cam; //Camera object reference
    public CinemachineVirtualCamera virtualCam; //Virtual Camera reference
    private Vector3 originalVirtualCamRotation; // Original rotation values for the virtual camera
    private Vector3 originalVirtualCamTransposeOffset; //Virtual Camera original transpose offset values
    //**********************************************************


    private Vector3 direction; //A reference to the directional movement of the player in 3D space
    private Vector3 velocity; //Velocity in relation to gravity
    private float gravity; //Gravity of player
    private GameMasterScript GM; //This is refrencing the game master script


    [Header("Weave Variables")]
    public float WeaveDistance = 12f;

    [SerializeField]
    private InputAction interactInput;





    
    

    void Awake()
    {
        //references to character components
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
      
        gravity = -3f;
        rotationSpeed = 0.1f;
        myTurn = false;

        //Section reserved for initiating inputs 
        moveInput = inputs.FindAction("Player/Move");
        interactInput = inputs.FindAction("Player/Interact");
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
        
    }

    void OnDisable() {
        inputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (myTurn) {
            if (!isPaused) {
                //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad.
                movement = moveInput.ReadValue<Vector2>();
                depossess = possessInput.WasPressedThisFrame();
                familiarMovementAbility = familiarMovementAbilityInput.IsPressed();
                
                //Move character only if they are on the ground or in leapOfFaith
                if (characterController.isGrounded || leapOfFaith) {
                    LookAndMove();
                    if (depossess && !leapOfFaith) {
                        myTurn = false;
                    }
                }

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

    void FixedUpdate() {
        if (myTurn) {
            if (!isPaused) {
                //Character movement
                if (direction.magnitude >= 0.1f) {
                    characterController.Move(newDirection.normalized * speed * Time.deltaTime);
                }

                characterController.Move(velocity * Time.deltaTime);
                    
                //Character gravity
                if (!characterController.isGrounded) {
                    velocity.y += gravity * Time.deltaTime;
                }
                else if (!familiarMovementAbility) { // retain gravitational momementum if dashing
                    velocity.y = -2f;
                }
            }
            
        }          
    }

    private void LookAndMove() {

        direction = new Vector3(movement.x,0,movement.y).normalized; //direction of movement

        //Character rotations
        if (direction.magnitude >= 0.1f) {

            float targetangle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetangle, ref rotationVelocity, rotationSpeed);

            transform.rotation = Quaternion.Euler(0,angle,0);
            newDirection = Quaternion.Euler(0,targetangle,0) * Vector3.forward;     

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
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (hit.gameObject.CompareTag("Breakable") && familiarMovementAbility) // if familiar collides with breakable object while using movement ability
        {
            Destroy(hit.gameObject); // TEMPORARY, in future, do something like this on the object's end
        }
    }



}
