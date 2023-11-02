using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

public class PlayerScript : MonoBehaviour
{
    //*******************************************************************************************************************
    // /!\ SCRIPT IS OBSOLETE /!\
    //*******************************************************************************************************************

    [Header("Movement Variables")]
    private CharacterController characterController; 
    private MovementScript movementScript; 
    private bool possessing;

    [Header("Character's Camera")]
    [SerializeField] private Camera mainCamera;
    public CinemachineVirtualCamera familiarVirtualCam;
    private int vCamRotationState; //State 0 is default

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Weave Variables")]
    public float WeaveDistance = 12f;
    public LayerMask weaveObject;
    private Vector3 playerPosition;
    private bool isWeaving;
    [SerializeField] private int weaveModeNumbers;
    [SerializeField] private Vector2 sensitivity = new Vector2(1500f, 1500f);
    [SerializeField] private Vector2 bias = new Vector2(0f, -1f);
    [SerializeField] private Vector2 currentMousePosition;
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
    private Weaveable weaveableScript;

    [Header("References")]
    public GameObject familiar;
    private FamiliarScript familiarScript;
    private GameMasterScript GM;   

    [Header("Animation")]
    public WeaverAnimationHandler weaverAnimationHandler;

    [Header("Prototype")]
    public GameObject relocateMode;
    public GameObject combineMode;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        characterController.enabled = false;
    }

    void Start()
    {
        familiarScript = familiar.GetComponent<FamiliarScript>();
        possessing = false;
        isWeaving = false;
        vCamRotationState = 0;
        pauseMenu.SetActive(false);
        enableInteractInput = false;

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
                CameraMasterScript.instance.SwitchToWeaverCamera();
                familiarScript.myTurn = false;
                possessing = false;
                familiarScript.depossessing = false;
                movementScript.active = true;
            }

            Weaving();
            DetectGamepad();

            //this is purely for testing the checkpoint function if it's working properly
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
            }
        }

        // KILL SWITCH
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void weaveController()
    {
        cursor = InputManagerScript.instance.weaveCursor;
        if (cursor.magnitude <= 0.1f)
        {
            return;
        }

        currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        warpPosition = currentMousePosition + bias + overflow + sensitivity * Time.deltaTime * cursor;
        warpPosition = new Vector2(Mathf.Clamp(warpPosition.x, 0, Screen.width), Mathf.Clamp(warpPosition.y, 0, Screen.height));
        overflow = new Vector2(warpPosition.x % 1, warpPosition.y % 1);
        Mouse.current.WarpCursorPosition(warpPosition);
    }

    void DetectGamepad()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            weaveController();
        }
        else
        {
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CameraTrigger")
        {
            CameraIndexScript cameraIndexScript = other.GetComponent<CameraIndexScript>();
            vCamRotationState = cameraIndexScript.cameraIndex;

            CameraMasterScript.instance.SwitchCameras(vCamRotationState);

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

    // this method will shoot out a raycast that will see if there are objects with the weaveObject LayerMask 
    //      and the IInteractable interface
    public void Weaving()
    {
        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z);

        // raycast to detect Weavable objects
        Vector3 rayDirection = transform.forward;
        Ray rayPlayer = new Ray(playerPosition, rayDirection); //the actual raycast from the player, this can be used for the line render but it takes the  raycast origin and the  direction of the raycast
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Debug.DrawRay(rayPlayer.origin, rayPlayer.direction * WeaveDistance, Color.red);

        // checks for a Weavable object within distance of Ray
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject) || Physics.Raycast(rayPlayer, out hitInfo, WeaveDistance, weaveObject))
        {
            weaveableScript = hitInfo.collider.GetComponent<Weaveable>();
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            float distanceBetween = Vector3.Distance(weaveableScript.transform.position, transform.position);

            if (interactable != null)
            {
                enableInteractInput = true;

                if (distanceBetween > WeaveDistance) 
                {
                    isWeaving = false;
                    enableInteractInput = true;
                    relocateMode.SetActive(false);// remember to delete this
                    combineMode.SetActive(false);// remember to delete this
                }

                if (interactInput) //this is the interact button that is taking from the player inputs
                {
                    //Debug.Log("Interaction");
                    interactable.Interact();
                    isWeaving = true;
                    enableUninteractInput = true;               
                    weaverAnimationHandler.ToggleWeaveAnim(isWeaving); 
                    weaveModeNumbers = 1;
                    interactable.Relocate();
                    relocateMode.SetActive(true);// remember to delete this
                    interactInput = false;
                }

                if (uninteractInput && weaveableScript.isWoven == true)
                {
                    interactable.Uninteract();
                    isWeaving = false;
                    enableInteractInput = true;                
                    enableUninteractInput = false;
                    enableWeaveModeSwitch = false; 
                    weaverAnimationHandler.ToggleWeaveAnim(isWeaving); 
                    relocateMode.SetActive(false);// remember to delete this
                    combineMode.SetActive(false);// remember to delete this
                    uninteractInput = false;
                }

                switch (weaveModeNumbers)
                {
                    case 1:
                        if (weaveModeSwitch && weaveableScript.isWoven == true)
                        {
                            interactable.WeaveMode();
                            weaveModeNumbers += 1;
                            relocateMode.SetActive(false);// remember to delete this
                            combineMode.SetActive(true);// remember to delete this
                            weaveModeSwitch = false;
                        }
                        break;

                    case 2:
                        if (weaveModeSwitch && weaveableScript.isWoven == true)
                        {
                            interactable.Relocate();
                            weaveModeNumbers -= 1;
                            combineMode.SetActive(false);// remember to delete this
                            relocateMode.SetActive(true);// remember to delete this
                            weaveModeSwitch = false;
                        }
                        break;
                }
            }
        }
        else
        {
            enableInteractInput = false;
        }

        //if the player is weaving an object they will look at the object
        if (isWeaving == true) 
        {
            // this will use the look at function based off of the hitinfo (line 210)
            transform.LookAt(new Vector3(hitInfo.collider.transform.position.x, transform.position.y, hitInfo.collider.transform.position.z)); 
            enableWeaveModeSwitch = true;
            enableInteractInput = false; 
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();

        // this is here so that if in the case that the object does hit the player it will uninteract itself 
        if (collision.gameObject.CompareTag("Weaveable"))
        {
            isWeaving = false;
            interactable.Uninteract();
            enableInteractInput = true;
            relocateMode.SetActive(false);// remember to delete this
            combineMode.SetActive(false);// remember to delete this
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
                CameraMasterScript.instance.SwitchToFamiliarCamera();
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
}
