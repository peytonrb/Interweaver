using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; //this is for testing 

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Variables")]
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    private InputAction possessInput; //Used for possession of familiar, or switching between weaver and familiar
    private bool possessButton; //State that checks if the possess button is being pressed
    private bool possessing; //Determines if the weaver is using possessing at the moment

    [Header("character's camera")]
    //Character Rotation values
    //**********************************************************
    public GameObject cam; //Camera object reference
    [SerializeField] private Camera mainCamera;
    public CinemachineVirtualCamera virtualCam; //Virtual Camera reference
    public CinemachineVirtualCamera[] virtualCameraList; //Virtual Camera references for chaning camera priorities
    private int vCamRotationState; //State 0 is default
    private bool cameraTransition;
    //**********************************************************

    private GameMasterScript GM; //This is refrencing the game master script

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    private InputAction pauseInput;
    private bool pauseButton;

    //Weave Variables
    //**********************************************************
    [Header("Weave Variables")]
    public float WeaveDistance = 12f;
    public LayerMask weaveObject;
    private Vector3 playerPosition;
    private bool IsWeaving;
    [SerializeField] private int WeaveModeNumbers;    
     private InputAction interactInput;
     private InputAction WeaveModeSwitch;
     private InputAction UninteractInput;
    //**********************************************************

    //Familiar
    //**********************************************************
    [Header("Familiar Reference")]
    public GameObject familiar;
    private FamiliarScript familiarScript;
    //**********************************************************

    //weaveable refrence
    //**********************************************************
    private Weaveable weaveableScript;

    [Header("Animation Calls")]
    public WeaverAnimationHandler weaverAnimationHandler;

    [Header("Lost Souls")]
    public int numLostSouls;
    public GameObject lostSoulUI;
    public TextMeshProUGUI lostSoulText;
    private readonly HashSet<GameObject> alreadyCollidedWith = new HashSet<GameObject>();
    public Animator animator;

    void Awake()
    {
        //references to character components
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        characterController.enabled = false;       
    }

    void Start()
    {
        familiarScript = familiar.GetComponent<FamiliarScript>();
        possessing = false;
        IsWeaving = false;
        numLostSouls = 0;
        vCamRotationState = 0;
        familiarScript.isPaused = false;
        cameraTransition = false;
        pauseMenu.SetActive(false);

        //Section reserved for initiating inputs 
        interactInput = inputs.FindAction("Player/Interact");
        UninteractInput = inputs.FindAction("Player/Uninteract");
        WeaveModeSwitch = inputs.FindAction("Player/WeaveModeSwitch");
        possessInput = inputs.FindAction("Player/Switch");
        pauseInput = inputs.FindAction("Player/Pause");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        Debug.Log("Active Current Position: " + transform.position);

        UninteractInput.Disable();
        WeaveModeSwitch.Disable();
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
        if (Time.timeScale != 0)
        {
            //Move character only if they are on the ground
            if (characterController.isGrounded)
            {
                if (!possessing && !cameraTransition)
                {
                    //Looks at input coming from TAB on keyboard (for now)
                    possessButton = possessInput.WasPressedThisFrame();
                    Possession();
                }
            }

            //For pausing
            pauseButton = pauseInput.WasPressedThisFrame();
            Pausing();

            if (familiarScript.depossessing)
            {
                virtualCam.m_Follow = gameObject.transform;
                familiarScript.myTurn = false;
                possessing = false;
                familiarScript.depossessing = false;
                movementScript.active = true;
            }

            Weaving();           

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
        if (!isPaused && !possessing && !cameraTransition && Time.timeScale != 0)
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
        if (hit.gameObject.tag == "Lost Soul" && !alreadyCollidedWith.Contains(hit.gameObject))
        {
            alreadyCollidedWith.Add(hit.gameObject);
            animator.SetBool("isOpen", true);
            numLostSouls++;
            lostSoulText.text = "" + numLostSouls;
            Destroy(hit.gameObject);
            StartCoroutine(lostSoulOnScreen());
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

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "CameraTrigger") { 
            switch (vCamRotationState) {
                case 0:
                    cameraTransition = true;
                    //Disable character controller
                    characterController.enabled = false;
                    //Move camera to new position (change cameras to the one at the new position)
                    virtualCameraList[0].Priority = 0;
                    virtualCameraList[1].Priority = 1;
                    StartCoroutine(CameraTransitionDelay());
                    vCamRotationState = 1;
                break;
                case 1:
                    cameraTransition = true;
                    characterController.enabled = false;
                    virtualCameraList[0].Priority = 1;
                    virtualCameraList[1].Priority = 0;
                    StartCoroutine(CameraTransitionDelay());
                    vCamRotationState = 0;
                break;

            }
            
        }
    }

    private void Weaving() //this method will shoot out a raycast that will see if there are objects with the weaeObject layermask and the IInteractable interface
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject))// the value 100 is for the raycast distance
        {
            weaveableScript = hitInfo.collider.GetComponent<Weaveable>();
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
            if (interactable != null)
            {
               
                if (interactInput.WasPressedThisFrame()) //this is the interact button that is taking from the player inputs
                {
                    interactable.Interact();
                    IsWeaving = true;
                    UninteractInput.Enable();//Enables the input                   
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // start weaving animations 
                    WeaveModeNumbers = 1;
                    interactable.Relocate();
                }


                if (UninteractInput.WasPressedThisFrame() && weaveableScript.Woven == true)
                {
                    interactable.Uninteract();
                    IsWeaving = false;
                    interactInput.Enable();//renables the inputs                   
                    UninteractInput.Disable();//disables the uninteract inputs
                    WeaveModeSwitch.Disable(); //disables the weavemodeswitch inputs
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // end weaving animations
                }               


                switch (WeaveModeNumbers)
                {
                    case 1:
                        if (WeaveModeSwitch.WasPressedThisFrame() && weaveableScript.Woven == true)
                        {
                            interactable.WeaveMode();
                            WeaveModeNumbers += 1;
                        }
                        break;

                    case 2:
                        if (WeaveModeSwitch.WasPressedThisFrame() && weaveableScript.Woven == true)
                        {
                            interactable.Relocate();
                            WeaveModeNumbers -= 1;
                        }
                        break;
                }
            }
        }

        if (IsWeaving == true) //if the player is weaving an object they will look at the object
        {
            this.transform.LookAt(new Vector3(hitInfo.collider.transform.position.x, 0, hitInfo.collider.transform.position.z));
            WeaveModeSwitch.Enable();
            interactInput.Disable(); //disables the inputs
        }       
    }


    private void Possession()
    {
        if (possessButton)
        {
            //Switches to Familiar
            virtualCam.m_Follow = familiar.transform;
            possessing = true;
            Debug.Log("Possessing");
            movementScript.active = false;
            StartCoroutine(familiarScript.ForcedDelay());
            
        }

    }

    private void Pausing()
    {
        if (pauseButton)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    // keeps lost soul UI on screen for a little bit then hides
    IEnumerator lostSoulOnScreen()
    {
        yield return new WaitForSeconds(5);
        animator.SetBool("isOpen", false);
    }


    // I'm sorry I had to take the easy approach D:
    IEnumerator CameraTransitionDelay() {
        yield return new WaitForSeconds(1);
        cameraTransition = false;
        characterController.enabled = true;
        StopCoroutine(CameraTransitionDelay());
    }
}
