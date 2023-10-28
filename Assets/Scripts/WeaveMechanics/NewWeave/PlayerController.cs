using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    private CharacterController characterController;
    private MovementScript movementScript;
    public GameObject inputManager;
    private InputManagerScript inputManagerScript;
    private bool possessing;

    [Header("Character's Camera")]
    [SerializeField] private Camera mainCamera;
    public CinemachineVirtualCamera familiarVirtualCam;
    public GameObject cameraCheckpointMaster;
    private CameraMasterScript CMScript;
    private int vCamRotationState; //State 0 is default

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Weave Variables")]
    public float WeaveDistance = 12f;
    public LayerMask weaveObject;
    private Vector3 playerPosition;
    private bool isWeaving;
    [SerializeField] private Vector2 sensitivity = new Vector2(1500f, 1500f);
    [SerializeField] private Vector2 bias = new Vector2(0f, -1f);
    [SerializeField] private Vector2 currentMousePosition;
    private Vector2 warpPosition;
    private Vector2 overflow;
    private Vector2 cursor;
    public bool interactInput;
    private Vector3 raycastPosition;
    private WeaveableNew weaveableScript;

    [Header("References")]
    public GameObject familiar;
    private FamiliarScript familiarScript;
    private GameMasterScript GM;

    [Header("Animation")]
    public WeaverAnimationHandler weaverAnimationHandler;

    [Header("Prototype")]
    public GameObject relocateMode;
    public GameObject combineMode;

    // NEW VARIABLES
    [Header("Flags")]
    public bool inRelocateMode;
    public bool inCombineMode;
    public bool uninteract;

    [Header("Im going crazy")]
    public IInteractable interactableObject;
    private GameObject wovenObject;
    [SerializeField] private float distanceBetween;

    void Awake()
    {
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
        vCamRotationState = 0;
        pauseMenu.SetActive(false);

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        Debug.Log("Active Current Position: " + transform.position);

        // Weaving variables
        inCombineMode = false;
        inRelocateMode = false;
        interactInput = false;
    }

    void Update()
    {
        // If game is not paused, return to normal movement functions
        if (Time.timeScale != 0)
        {
            if (familiarScript.depossessing)
            {
                CMScript.SwitchToWeaverCamera();
                familiarScript.myTurn = false;
                possessing = false;
                familiarScript.depossessing = false;
                movementScript.active = true;
            }

            // WEAVE - interactInput set by InputManager
            if (interactInput)
            {
                WeaveActivated();
            }
            // switch into combining mode - relocate is default
            else if (!interactInput && inCombineMode && interactableObject != null)
            {
                interactableObject.WeaveMode();
                relocateMode.SetActive(false); // on-screen ui
                combineMode.SetActive(true); // on-screen ui
            }

            // inRelocateMode and inCombineMode both set by InputManager
            if (inRelocateMode || inCombineMode)
            {
                // player points towards woven object
                transform.LookAt(new Vector3(wovenObject.transform.position.x, transform.position.y, wovenObject.transform.position.z));

                distanceBetween = Vector3.Distance(weaveableScript.transform.position, transform.position);

                // if the player moves too far from the object while weaving
                if (distanceBetween > WeaveDistance)
                {
                    Uninteract();
                }
            }

            // snap weave - uninteract set by InputManager
            if (uninteract)
            {
                Uninteract();
            }

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

    private void WeaveActivated()
    {
        interactableObject = determineInteractability();

        // if too far apart
        if (distanceBetween < WeaveDistance)
        {
            if (inRelocateMode && interactableObject != null)
            {
                interactableObject.Interact();
                weaverAnimationHandler.ToggleWeaveAnim(isWeaving);
                interactableObject.Relocate();
                relocateMode.SetActive(true); // on-screen ui
                interactInput = false;
            }
        }
    }

    private IInteractable determineInteractability()
    {
        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z);
        Vector3 rayDirection = transform.forward;

        // the actual raycast from the player, this can be used for the line render 
        //      but it takes the raycast origin and the  direction of the raycast
        Ray rayPlayer = new Ray(playerPosition, rayDirection);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Debug.DrawRay(rayPlayer.origin, rayPlayer.direction * WeaveDistance, Color.red);

        // checks for a Weavable object within distance of Ray
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject) || Physics.Raycast(rayPlayer, out hitInfo, WeaveDistance, weaveObject))
        {
            weaveableScript = hitInfo.collider.GetComponent<WeaveableNew>();
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            wovenObject = hitInfo.collider.gameObject;
            return interactable;
        }

        return null;
    }

    private void Uninteract()
    {
        if (interactableObject != null)
        {
            interactableObject.Uninteract();
            interactableObject = null;
            weaverAnimationHandler.ToggleWeaveAnim(isWeaving);
            relocateMode.SetActive(false); // remember to delete this
            combineMode.SetActive(false); // remember to delete this
            uninteract = false;
        }
    }


    private void weaveController()
    {
        cursor = inputManagerScript.weaveCursor;
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

    void OnCollisionEnter(Collision collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();

        // this is here so that if in the case that the object does hit the player it will uninteract itself 
        if (collision.gameObject.CompareTag("Weaveable"))
        {
            interactable.Uninteract();
            inCombineMode = false;
            inRelocateMode = false;
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
                CMScript.SwitchToFamiliarCamera();
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