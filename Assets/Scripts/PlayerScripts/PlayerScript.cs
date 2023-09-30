using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
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
    private InputAction possessInput; //Used for possession of familiar, or switching between weaver and familiar
    private Vector2 movement; //Vector2 regarding movement, which is set to track from moveInput's Vector2
    private bool possessButton; //State that checks if the possess button is being pressed
    private bool possessing; //Determines if the weaver is using possessing at the moment


    [Header("character's camera")]
    //Character Rotation values
    //**********************************************************
    private float rotationSpeed;
    private float rotationVelocity;
    private Vector3 newDirection;
    public GameObject cam; //Camera object reference
    public CinemachineVirtualCamera virtualCam; //Virtual Camera reference
    //**********************************************************

    private Vector3 direction; //A reference to the directional movement of the player in 3D space
    private Vector3 velocity; //Velocity in relation to gravity
    private float gravity; //Gravity of player
    private GameMasterScript GM; //This is refrencing the game master script
    public int numLostSouls;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public bool isPaused;
    private InputAction pauseInput;
    private bool pauseButton;

    //Weave Variables
    //**********************************************************
    [Header("Weave Variables")]
    public float WeaveDistance = 12f;
    public LayerMask weaveObject;
    private Vector3 playerPosition;
    private bool IsWeaving;
    [SerializeField] private Vector3 raycastPosition;
    [SerializeField] private InputAction interactInput;
    [SerializeField] private InputAction UninteractInput;
    [SerializeField] private float TooCloseDistance;
    //**********************************************************

    //Familiar
    //**********************************************************
    [Header("Familiar Reference")]
    public GameObject familiar;
    private FamiliarScript familiarScript;
    //**********************************************************

    [Header("Animation Calls")]
    public WeaverAnimationHandler weaverAnimationHandler;

    void Awake()
    {
        //references to character components
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
    }

    void Start()
    {
        familiarScript = familiar.GetComponent<FamiliarScript>();

        gravity = -3f;
        rotationSpeed = 0.1f;
        possessing = false;
        IsWeaving = false;
        isPaused = false;
        numLostSouls = 0;
        familiarScript.isPaused = false;
        pauseMenu.SetActive(false);

        //Section reserved for initiating inputs 
        moveInput = inputs.FindAction("Player/Move");
        interactInput = inputs.FindAction("Player/Interact");
        UninteractInput = inputs.FindAction("Player/Uninteract");
        possessInput = inputs.FindAction("Player/Switch");
        pauseInput = inputs.FindAction("Player/Pause");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        Debug.Log("Active Current Position: " + transform.position);
    }

    void OnEnable()
    {
        inputs.Enable();

    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void Update()
    {
        //If game is not paused, return to normal movement functions
        if (isPaused == false)
        {
            //Move character only if they are on the ground
            if (characterController.isGrounded)
            {
                if (possessing == false)
                {
                    //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad.
                    movement = moveInput.ReadValue<Vector2>();
                    LookAndMove();
                }
                //Looks at input coming from TAB on keyboard (for now)
                possessButton = possessInput.WasPressedThisFrame();
                Possession();

            }

            //For pausing
            pauseButton = pauseInput.WasPressedThisFrame();
            Pausing();

            weaving();

            if (Input.GetKeyDown(KeyCode.Space)) //this is purely for testing the checkpoint function if it's working properly
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //this is for testing
            }

            if (Input.GetKeyDown(KeyCode.F)) // placeholder interaction key
            {
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 10f); // second number is radius of sphere
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "NPC")
                    {
                        hitCollider.gameObject.GetComponent<DialogueTriggers>().triggerDialogue();
                    }
                }
            }
        }


        //KILL SWITCH
        //**************************************
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        //**************************************

    }

    void FixedUpdate()
    {
        if (isPaused == false)
        {
            //Character movement
            if (direction.magnitude >= 0.1f)
            {
                characterController.Move(newDirection.normalized * speed * Time.deltaTime);
            }

            //Character movement
            if (direction.magnitude >= 0.1f)
            {
                characterController.Move(newDirection.normalized * speed * Time.deltaTime);
                weaverAnimationHandler.ToggleMoveSpeedBlend(speed); // note: speed is static now, but this should work fine when variable speed is added
            }

            characterController.Move(velocity * Time.deltaTime);

            //Character gravity
            if (!characterController.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                weaverAnimationHandler.ToggleFallAnim(true);
            }
            else
            {
                weaverAnimationHandler.ToggleFallAnim(false);
                velocity.y = -2f;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Lost Soul")
        {
            numLostSouls++;
            Destroy(hit.gameObject);
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

    private void weaving() //this method will shoot out a raycast that will see if there are objects with the weaeObject layermask and the IInteractable interface
    {
        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z); //this is the raycast origin 
        Vector3 rayDirection = transform.forward;
        Ray ray = new Ray(playerPosition, rayDirection); //the actual  raycast
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * WeaveDistance, Color.red); //debug  for  when the game  starts and the line can be  seen on  scene

        if (Physics.Raycast(ray, out hitInfo, WeaveDistance, weaveObject))
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
            if (interactable != null)
            {
                if (interactInput.WasPressedThisFrame()) //this is the interact button that is taking from the player inputs
                {
                    interactable.Interact();
                    IsWeaving = true;
                    interactInput.Disable();//disables the interactInput so  that the player can't press it multiple times
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // start weaving animations 

                }
                if (UninteractInput.WasPressedThisFrame())
                {
                    interactable.Uninteract();
                    IsWeaving = false;
                    interactInput.Enable();//renables the inputs
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // end weaving animations
                }
                float distanceBetween = Vector3.Distance(hitInfo.collider.transform.position, transform.position);
                if (distanceBetween > WeaveDistance || distanceBetween < TooCloseDistance)
                {
                    IsWeaving = false;
                    interactable.Uninteract();
                    interactInput.Enable();
                }
            }
        }

        if (IsWeaving == true) //if the player is weaving an object thet will look at the object
        {
            this.transform.LookAt(new Vector3(hitInfo.collider.transform.position.x, 0, hitInfo.collider.transform.position.z));
        }

    }


    private void Possession()
    {

        if (possessButton)
        {
            if (possessing == false)
            {
                //Switches to Familiar
                virtualCam.m_Follow = familiar.transform;
                familiarScript.myTurn = true;
                possessing = true;
            }
            else
            {
                if (familiarScript.myTurn == false)
                {
                    virtualCam.m_Follow = gameObject.transform;
                    possessing = false;
                }
            }
        }

    }

    private void Pausing()
    {
        if (pauseButton)
        {
            pauseMenu.SetActive(true);
            isPaused = true;
            familiarScript.isPaused = true;
        }
    }

    public void Unpausing()
    {
        isPaused = false;
        familiarScript.isPaused = false;
    }

}
