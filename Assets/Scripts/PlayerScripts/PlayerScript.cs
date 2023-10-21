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

    [Header("Character's Camera")]
    //Character Rotation values
    //**********************************************************
    [SerializeField] private Camera mainCamera;
    public CinemachineVirtualCamera familiarVirtualCam;
    public GameObject cameraCheckpointMaster;
    private CameraMasterScript CMScript;
    private int vCamRotationState; //State 0 is default
    //**********************************************************

    private GameMasterScript GM; //This is refrencing the game master script

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    private InputAction pauseInput;
    private bool pauseButton;


    //**********************************************************
    [Header("prototype purposes")]
    public GameObject RelocateMode;
    public GameObject CombineMode;
    //**********************************************************


    //Weave Variables
    //**********************************************************
    [Header("Weave Variables")]
    public float WeaveDistance = 12f;
    public LayerMask weaveObject;
    private Vector3 playerPosition;
    public bool IsWeaving { get; private set; }
    [SerializeField] private int WeaveModeNumbers;
    [SerializeField] private Vector2 sensitivity = new Vector2(1500f, 1500f);
    [SerializeField] private Vector2 bias = new Vector2(0f, -1f);
    [SerializeField] private Vector2 currentmousePosition;
    private Vector2 warpPosition;
    private Vector2 overflow;
    private Vector2 Cursor;
    private InputAction weaveCursor;
    private InputAction interactInput;
    private InputAction WeaveModeSwitch;
    private InputAction UninteractInput;
    [SerializeField] private Vector3 raycastPosition;
    [SerializeField] private Vector3 position;
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
        CMScript = cameraCheckpointMaster.GetComponent<CameraMasterScript>();
        possessing = false;
        IsWeaving = false;
        vCamRotationState = 0;
        pauseMenu.SetActive(false);

        //Section reserved for initiating inputs 
        interactInput = inputs.FindAction("Player/Interact");
        UninteractInput = inputs.FindAction("Player/Uninteract");
        WeaveModeSwitch = inputs.FindAction("Player/WeaveModeSwitch");
        possessInput = inputs.FindAction("Player/Switch");
        pauseInput = inputs.FindAction("Player/Pause");
        weaveCursor = inputs.FindAction("Player/Weave");

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
                if (possessing == false)
                {
                    //Looks at input coming from TAB on keyboard (for now)
                    possessButton = possessInput.WasPressedThisFrame();
                    Possession();
                }
            }

            //For pausing
            pauseButton = pauseInput.WasPressedThisFrame();
            Pausing();

            // For dialogue, I don't understand the new input system 
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }

            if (familiarScript.depossessing)
            {
                Debug.Log(CMScript.cameraOnPriority);
                CMScript.vcams[CMScript.cameraOnPriority].Priority = 1;
                familiarVirtualCam.Priority = 0;
                familiarScript.myTurn = false;
                possessing = false;
                familiarScript.depossessing = false;
                movementScript.active = true;
            }

            Weaving();
            DetectGamepad();
            if (Input.GetKeyDown(KeyCode.Space)) //this is purely for testing the checkpoint function if it's working properly
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //this is for testing
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
        if (Time.timeScale != 0)
        {
            if (possessing == false) {
            }
            
        }
    }

    private void weaveController()
    {
        Cursor = weaveCursor.ReadValue<Vector2>();
        if (Cursor.magnitude <= 0.1f)
        {
            return;
        }
        currentmousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        warpPosition = currentmousePosition + bias + overflow + sensitivity * Time.deltaTime * Cursor;
        warpPosition = new Vector2(Mathf.Clamp(warpPosition.x, 0, Screen.width), Mathf.Clamp(warpPosition.y, 0, Screen.height));
        overflow = new Vector2(warpPosition.x % 1, warpPosition.y % 1);
        Mouse.current.WarpCursorPosition(warpPosition);
    }

    void DetectGamepad()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            weaveController();// this works I just need to find a way to make it so that when the controller is detected it switches to this, it will for now be commented out for the time being
        }
        else
        {
            return;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "CameraTrigger") {
            CameraIndexScript cameraIndexScript = other.GetComponent<CameraIndexScript>();
            vCamRotationState = cameraIndexScript.cameraIndex;

            CMScript.SwitchCameras(vCamRotationState);

            //ROTATION STATE CHANGES HAVE BEEN MOVED TO CAMERMASTERSCRIPT~
            
        }
    }

    public void Interact()
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

    private void Weaving() //this method will shoot out a raycast that will see if there are objects with the weaeObject layermask and the IInteractable interface
    {

        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z); //this is the raycast origin 
        Vector3 rayDirection = transform.forward;
        Ray rayPlayer = new Ray(playerPosition, rayDirection); //the actual  raycast from the player, this can be used for the line render
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;  //grabs the raycast  hit information from both the player's raycast and the mouse raycast
        Debug.DrawRay(rayPlayer.origin, rayPlayer.direction * WeaveDistance, Color.red);//a debug line for the player's raycast
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject) || Physics.Raycast(rayPlayer, out hitInfo, WeaveDistance, weaveObject))// the value 100 is for the raycast distance for the mouse raycast and uses the OR function for the raycast for  the player
        {
            weaveableScript = hitInfo.collider.GetComponent<Weaveable>(); //local refrence to itself so that it can access itself from a different object 
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
            float DistanceBetween = Vector3.Distance(weaveableScript.transform.position, transform.position);
            if (interactable != null)
            {
                if (DistanceBetween > WeaveDistance)
                {
                    IsWeaving = false;
                    interactInput.Enable();
                    RelocateMode.SetActive(false);// remember to delete this
                    CombineMode.SetActive(false);// remember to delete this
                }
               
                if (interactInput.WasPressedThisFrame()) //this is the interact button that is taking from the player inputs
                {
                    interactable.Interact();
                    IsWeaving = true;
                    UninteractInput.Enable();//Enables the input                   
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // start weaving animations 
                    WeaveModeNumbers = 1;
                    interactable.Relocate();
                    RelocateMode.SetActive(true);// remember to delete this
                }


                if (UninteractInput.WasPressedThisFrame() && weaveableScript.Woven == true)
                {
                    interactable.Uninteract();
                    IsWeaving = false;
                    interactInput.Enable();//renables the inputs                   
                    UninteractInput.Disable();//disables the uninteract inputs
                    WeaveModeSwitch.Disable(); //disables the weavemodeswitch inputs
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // end weaving animations
                    RelocateMode.SetActive(false);// remember to delete this
                    CombineMode.SetActive(false);// remember to delete this
                }               


                switch (WeaveModeNumbers)
                {
                    case 1:
                        if (WeaveModeSwitch.WasPressedThisFrame() && weaveableScript.Woven == true)
                        {
                            interactable.WeaveMode();
                            WeaveModeNumbers += 1;
                            RelocateMode.SetActive(false);// remember to delete this
                            CombineMode.SetActive(true);// remember to delete this
                        }
                        break;

                    case 2:
                        if (WeaveModeSwitch.WasPressedThisFrame() && weaveableScript.Woven == true)
                        {
                            interactable.Relocate();
                            WeaveModeNumbers -= 1;
                            CombineMode.SetActive(false);// remember to delete this
                            RelocateMode.SetActive(true);// remember to delete this
                        }
                        break;
                }
            }
        }

        if (IsWeaving == true) //if the player is weaving an object they will look at the object
        {
            transform.LookAt(new Vector3(hitInfo.collider.transform.position.x, transform.position.y, hitInfo.collider.transform.position.z));
            WeaveModeSwitch.Enable();
            interactInput.Disable(); //disables the inputs
        }       
    }


    private void Possession()
    {
        if (possessButton)
        {
            //Switches to Familiar
            for(int i = 0; i < CMScript.vcams.Length; i++) {
                CMScript.vcams[i].Priority = 0;
            }
            familiarVirtualCam.Priority = 1;
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
}
