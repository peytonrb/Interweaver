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
    public GameObject inputManager;
    private InputManagerScript inputManagerScript;
    //private InputAction possessInput; //Used for possession of familiar, or switching between weaver and familiar
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
    private bool IsWeaving;
    [SerializeField] private int WeaveModeNumbers;
    [SerializeField] private Vector2 sensitivity = new Vector2(1500f, 1500f);
    [SerializeField] private Vector2 bias = new Vector2(0f, -1f);
    [SerializeField] private Vector2 currentmousePosition;
    private Vector2 warpPosition;
    private Vector2 overflow;
    private Vector2 cursor;
    public bool interactInput;
    public bool enableInteractInput;
    public bool weaveModeSwitch;
    public bool enableWeaveModeSwitch;
    public bool uninteractInput;
    public bool enableUninteractInput;
    [SerializeField] private Vector3 raycastPosition;
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
        CMScript = cameraCheckpointMaster.GetComponent<CameraMasterScript>();
        inputManagerScript = inputManager.GetComponent<InputManagerScript>();
        possessing = false;
        IsWeaving = false;
        numLostSouls = 0;
        vCamRotationState = 0;
        pauseMenu.SetActive(false);
        enableInteractInput = true;

        //Section reserved for initiating inputs 
        //interactInput = inputs.FindAction("Player/Interact");
        //UninteractInput = inputs.FindAction("Player/Uninteract");
        //WeaveModeSwitch = inputs.FindAction("Player/WeaveModeSwitch");
        //possessInput = inputs.FindAction("Player/Switch");
        //pauseInput = inputs.FindAction("Player/Pause");
        //weaveCursor = inputs.FindAction("Player/Weave");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        Debug.Log("Active Current Position: " + transform.position);

        enableUninteractInput = false;
        enableWeaveModeSwitch = false;
    }

    void Update()
    {
        //If game is not paused, return to normal movement functions
        if (Time.timeScale != 0)
        {
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

    private void weaveController()
    {
        cursor = inputManagerScript.weaveCursor;
        if (cursor.magnitude <= 0.1f)
        {
            return;
        }
        currentmousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        warpPosition = currentmousePosition + bias + overflow + sensitivity * Time.deltaTime * cursor;
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

    public void Weaving() //this method will shoot out a raycast that will see if there are objects with the weaeObject layermask and the IInteractable interface
    {

        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z); //this is the raycast origin 
        Vector3 rayDirection = transform.forward;
        Ray rayPlayer = new Ray(playerPosition, rayDirection); //the actual  raycast from the player, this can be used for the line render
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // the raycast from the mouse
        RaycastHit hitInfo;  //grabs the raycast  hit information from both the player's raycast and the mouse raycast
        Debug.DrawRay(rayPlayer.origin, rayPlayer.direction * WeaveDistance, Color.red);//a debug line for the player's raycast
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject) || Physics.Raycast(rayPlayer, out hitInfo, WeaveDistance, weaveObject))// the value 100 is for the raycast distance for the mouse raycast and uses the OR function for the raycast for  the player
        {
            weaveableScript = hitInfo.collider.GetComponent<Weaveable>(); //local refrence to itself so that it can access itself from a different object 
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
            if (interactable != null)
            {
               
                if (interactInput) //this is the interact button that is taking from the player inputs
                {
                    //Debug.Log("Interaction");
                    interactable.Interact();
                    IsWeaving = true;
                    enableUninteractInput = true;//Enables the input                   
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // start weaving animations 
                    WeaveModeNumbers = 1;
                    interactable.Relocate();
                    RelocateMode.SetActive(true);// remember to delete this
                    interactInput = false;
                }


                if (uninteractInput && weaveableScript.Woven == true)
                {
                    interactable.Uninteract();
                    IsWeaving = false;
                    enableInteractInput = true;//renables the inputs                   
                    enableUninteractInput = false;//disables the uninteract inputs
                    enableWeaveModeSwitch = false; //disables the weavemodeswitch inputs
                    weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // end weaving animations
                    RelocateMode.SetActive(false);// remember to delete this
                    CombineMode.SetActive(false);// remember to delete this
                    uninteractInput = false;
                }               


                switch (WeaveModeNumbers)
                {
                    case 1:
                        if (weaveModeSwitch && weaveableScript.Woven == true)
                        {
                            interactable.WeaveMode();
                            WeaveModeNumbers += 1;
                            RelocateMode.SetActive(false);// remember to delete this
                            CombineMode.SetActive(true);// remember to delete this
                            weaveModeSwitch = false;
                        }
                        break;

                    case 2:
                        if (weaveModeSwitch && weaveableScript.Woven == true)
                        {
                            interactable.Relocate();
                            WeaveModeNumbers -= 1;
                            CombineMode.SetActive(false);// remember to delete this
                            RelocateMode.SetActive(true);// remember to delete this
                            weaveModeSwitch = false;
                        }
                        break;
                }
            }
        }

        if (IsWeaving == true) //if the player is weaving an object they will look at the object
        {
            transform.LookAt(new Vector3(hitInfo.collider.transform.position.x, transform.position.y, hitInfo.collider.transform.position.z)); // this will use the look at function based off of the hitinfo (line 252)
            weaveModeSwitch = true;
            interactInput = false; //disables the inputs
        }       
    }

    void OnCollisionEnter(Collision collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();
        if ( collision.gameObject.CompareTag("Weaveable"))
        {
            IsWeaving = false;
            interactable.Uninteract();
            interactInput = true;
            RelocateMode.SetActive(false);// remember to delete this
            CombineMode.SetActive(false);// remember to delete this
        }
    }

    public void Possession()
    {
        //Move character only if they are on the ground
        if (characterController.isGrounded && Time.timeScale != 0)
        {
            if (possessing == false)
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
    }

    public void Pausing()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    // keeps lost soul UI on screen for a little bit then hides
    IEnumerator lostSoulOnScreen()
    {
        yield return new WaitForSeconds(5);
        animator.SetBool("isOpen", false);
    }
}
