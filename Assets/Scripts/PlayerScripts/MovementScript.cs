using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    private GameMasterScript GM; //This is refrencing the game master script

    [Header("Movement Variables")]
    public float speed; //Base walk speed for player
    private CharacterController characterController; //references the character controller component
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    private InputAction moveInput; //Is the specific input action regarding arrow keys, WASD, and left stick
    private Vector2 movement; //Vector2 regarding movement, which is set to track from moveInput's Vector2
    private Vector3 direction; //A reference to the directional movement of the player in 3D space
    private Vector3 velocity; //Velocity in relation to gravity
    private float gravity; //Gravity of player
    private float originalGravity; // Original gravity of the player
    public bool aerialControl; //Bool which determines if controller can move in air 

    [Header("character's camera")]
    //Character Rotation values
    private float rotationSpeed;
    private float rotationVelocity;
    private Vector3 newDirection;
    public GameObject cam; //Camera object reference
    public CinemachineVirtualCamera virtualCam; //Virtual Camera reference

    public bool active; //Determines if movement controller is active

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        gravity = -3f;
        rotationSpeed = 0.1f;
        originalGravity = gravity;

        //Section reserved for initiating inputs 
        moveInput = inputs.FindAction("Player/Move");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        //transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
    }

    void OnEnable()
    {
        inputs.Enable();

    }

    void OnDisable()
    {
        inputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Move character only if they are on the ground
        if (characterController.isGrounded || aerialControl)
        {
            if (Time.timeScale != 0 && active)
            {
                //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad
                movement = moveInput.ReadValue<Vector2>();
                LookAndMove();
            }
        }
    }

    void FixedUpdate()
    {
        //Character movement
        if (direction.magnitude >= 0.1f)
        {
            characterController.Move(newDirection.normalized * speed * Time.deltaTime);
            //weaverAnimationHandler.ToggleMoveSpeedBlend(speed); // note: speed is static now, but this should work fine when variable speed is added
        }

        characterController.Move(velocity * Time.deltaTime);

        //Character gravity
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            //weaverAnimationHandler.ToggleFallAnim(true);
        }
        else
        {
            //weaverAnimationHandler.ToggleFallAnim(false);
            velocity.y = -2f;
        }
    }

    private void LookAndMove()
    {
        direction = new Vector3(movement.x, 0, movement.y).normalized; //direction of movement

        //Character rotations
        if (direction.magnitude >= 0.1f)
        {
            float targetangle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref rotationVelocity, rotationSpeed);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            newDirection = Quaternion.Euler(0, targetangle, 0) * Vector3.forward;
        }
    }

    public void ChangeInGravity(float gravityChange)
    {
        gravity = gravityChange;
    }

    public void ResetGravity()
    {
        gravity = originalGravity;
    }
}
