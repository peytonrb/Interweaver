using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    private CharacterController characterController;
    private MovementScript movementScript;
    private bool possessing;

    [Header("Character's Camera")]
    [CannotBeNullObjectField] [SerializeField] private Camera mainCamera;
    private int vCamRotationState; //State 0 is default

    [Header("Pause Menu")]
    [CannotBeNullObjectField] public GameObject pauseMenu;
    private PauseScript pauseScript;

    [Header("Weave Variables")]
    public float weaveDistance = 20f;
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
    public bool isCurrentlyWeaving; //For input manager to read if currently weaving
    private Vector3 raycastPosition;
    public WeaveableNew weaveableScript;

    //new variables
    public bool inRelocateMode;
    public bool inCombineMode;
    public bool uninteract;
    private IInteractable interactableObject;
    private GameObject wovenObject;
    [SerializeField] private float distanceBetween;
    public bool floatingIslandCrystal = false; // used by input manager

    [Header("VFX")]
    [CannotBeNullObjectField] public GameObject weaveSpawn;   //  WILL BE ASSIGNED AT RUNTIME ONCE SCRIPTS ARE FINALIZED
    public WeaveFXScript weaveVisualizer;

    [Header("References")]
    [CannotBeNullObjectField] public GameObject familiar;
    private FamiliarScript familiarScript;
    private GameMasterScript GM;

    [Header("Animation")]
    [CannotBeNullObjectField] public WeaverAnimationHandler weaverAnimationHandler;

    [Header("Prototype")]
    [CannotBeNullObjectField] public GameObject relocateMode;
    [CannotBeNullObjectField] public GameObject combineMode;

    [Header("Cutscene")]
    [CannotBeNullObjectField] public GameObject[] cutsceneManager;
    //private CutsceneManagerScript cms;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        characterController.enabled = false;
    }

    void Start()
    {
        familiarScript = familiar.GetComponent<FamiliarScript>();
        pauseScript = pauseMenu.GetComponent<PauseScript>();
        weaveVisualizer = GetComponent<WeaveFXScript>(); // THIS WILL CAUSE A NULL IF THERE IS NO WEAVEFXSCRIPT ATTACHED TO PLAYER
        weaveVisualizer.DisableWeave();
        possessing = false;
        vCamRotationState = 0;
        pauseMenu.SetActive(false);

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.WeaverCheckPointPos;
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
                CameraMasterScript.instance.SwitchToWeaverCamera();
                familiarScript.myTurn = false;
                possessing = false;
                familiarScript.depossessing = false;
                movementScript.active = true;
            }

            // switch into combining mode - relocate is default
            if (!interactInput && inCombineMode && interactableObject != null)
            {
                interactableObject.WeaveMode();
                relocateMode.SetActive(false); // on-screen ui
                combineMode.SetActive(true); // on-screen ui
            }
            // switch back into relocate mode 
            else if (!interactInput && inRelocateMode && interactableObject != null)
            {
                interactableObject.Relocate();
                relocateMode.SetActive(true); // on-screen ui
                combineMode.SetActive(false); // on-screen ui
            }

            // inRelocateMode and inCombineMode both set by InputManager
            // handles things that occur while the weave is active
            if (inRelocateMode || inCombineMode)
            {
                // player points towards woven object
                transform.LookAt(new Vector3(wovenObject.transform.position.x, transform.position.y, wovenObject.transform.position.z));

                // line renderer draws weave
                weaveVisualizer.DrawWeave(weaveSpawn.transform.position, wovenObject.transform.position);

                distanceBetween = Vector3.Distance(weaveableScript.transform.position, transform.position);

                // if the player moves too far from the object while weaving
                if (distanceBetween > weaveDistance)
                {
                    uninteract = true;
                }
            }

            // snap weave - uninteract set by InputManager
            if (uninteract)
            {
                Uninteract();
                weaveVisualizer.DisableWeave();
            }

            //DetectGamepad();

            //this is purely for testing the checkpoint function if it's working properly
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            //Debug.Log("Using a controller = " + pauseScript.usingController);
        }

        // KILL SWITCH
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void WeaveActivated()
    {
        interactableObject = determineInteractability();

        if (interactableObject != null)
        {
            distanceBetween = Vector3.Distance(weaveableScript.transform.position, transform.position);

            if (distanceBetween < weaveDistance)
            {
                Debug.Log("Object checked!");
                isCurrentlyWeaving = true;

                // vfx
                weaveVisualizer.ActivateWeave();

                interactableObject.Interact();
                inRelocateMode = true;
                weaverAnimationHandler.ToggleWeaveAnim(isWeaving);
                interactableObject.Relocate();
                relocateMode.SetActive(true); // on-screen ui
            }
        }

        // if too far apart & object is weavable
        interactInput = false;
    }

    private IInteractable determineInteractability()
    {
        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z);
        Vector3 rayDirection = transform.forward;

        // the actual raycast from the player, this can be used for the line render 
        //      but it takes the raycast origin and the direction of the raycast
        Ray rayPlayer = new Ray(playerPosition, rayDirection);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Debug.DrawRay(rayPlayer.origin, rayPlayer.direction * weaveDistance, Color.red);

        // checks for a Weavable object within distance of Ray
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject) || Physics.Raycast(rayPlayer, out hitInfo, weaveDistance, weaveObject))
        {
            Debug.Log("Raycast hit");
            
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
            relocateMode.SetActive(false); // on screen ui
            combineMode.SetActive(false); // on screen ui
            uninteract = false;
            inRelocateMode = false;
            inCombineMode = false;
            isCurrentlyWeaving = false;
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
            pauseScript.TurnOnUsingController();
            pauseScript.toggle.isOn = true;
        }
        else
        {
            pauseScript.TurnOffUsingController();
            pauseScript.toggle.isOn = false;
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CameraTrigger")
        {
            CameraIndexScript cameraIndexScript = other.GetComponent<CameraIndexScript>();
            vCamRotationState = cameraIndexScript.cameraIndex;

            CameraMasterScript.instance.SwitchWeaverCameras(vCamRotationState);

            //ROTATION STATE CHANGES HAVE BEEN MOVED TO CAM ERMASTERSCRIPT~
        }

        if (other.gameObject.tag == "CutsceneTrigger") {
            //Only the trigger that is a child of a certain cutscene manager will activate a cutscene.
            foreach (GameObject cm in cutsceneManager) {
                CutsceneManagerScript cms = cm.GetComponent<CutsceneManagerScript>();
                cms.StartCutscene();
            }     
        }

        if (other.gameObject.tag == "LevelTrigger") {
            LevelTriggerScript levelTriggerScript = other.GetComponent<LevelTriggerScript>();
            int section = levelTriggerScript.triggerType;
            
            LevelManagerScript.instance.TurnOnOffSection(section);
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
                CameraMasterScript.instance.SwitchToFamiliarCamera();
                possessing = true;
                Debug.Log("Possessing");
                movementScript.active = false;
                StartCoroutine(familiarScript.ForcedDelay());
            }
        }
    }

    public void Death()
    {
        transform.position = GM.WeaverCheckPointPos;
    }

    public void Pausing()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
}