using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; //this is for testing 

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Variables")]
    public float speed; //Base walk speed for player
    private CharacterController characterController; //references the character controller component
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    private InputAction moveInput; //Is the specific input action regarding arrow keys, WASD, and left stick
    private Vector2 movement; //Vector2 regarding movement, which is set to track from moveInput's Vector2


    [Header("character's camera")]
    //Character Rotation values
    //**********************************************************
    private float rotationSpeed; 
    private float rotationVelocity;
    private Vector3 newDirection;
    public GameObject cam; //Camera object reference
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

        //Section reserved for initiating inputs 
        moveInput = inputs.FindAction("Player/Move");
        interactInput = inputs.FindAction("Player/Interact");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>(); 
        transform.position = GM.LastCheckPointPos;
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
        //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad.
        movement = moveInput.ReadValue<Vector2>();

        //Move character only if they are on the ground
        if (characterController.isGrounded) {
            LookAndMove();
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

    void FixedUpdate() {

        //Character movement
        if (direction.magnitude >= 0.1f) {
            characterController.Move(newDirection.normalized * speed * Time.deltaTime);
        }

        //Character gravity
        if (!characterController.isGrounded) {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity * Time.deltaTime);   
            
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

}
