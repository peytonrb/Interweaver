using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public bool canSwitch = true; // is the weaver allowed to switch 
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
    [SerializeField]
    private GameObject TargetingArrow;
    private Vector3 worldPosition;

    //new variables
    public bool inRelocateMode;
    public bool inCombineMode;
    public bool uninteract = false;
    private IInteractable interactableObject;
    private GameObject wovenObject;
    [SerializeField] private float distanceBetween;
    public bool floatingIslandCrystal = false; // used by input manager
    [HideInInspector] public bool inCutscene;

    [Header("VFX")]
    [CannotBeNullObjectField] public GameObject weaveSpawn;   //  WILL BE ASSIGNED AT RUNTIME ONCE SCRIPTS ARE FINALIZED
    public WeaveFXScript weaveVisualizer;

    [Header("References")]
    [CannotBeNullObjectField] public GameObject familiar;
    [CannotBeNullObjectField] public RespawnController respawnController;
    private FamiliarScript familiarScript;
    private GameMasterScript GM;

    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;

    [Header("Prototype")]
    [CannotBeNullObjectField] public GameObject relocateMode;
    [CannotBeNullObjectField] public GameObject combineMode;

    [Header("Audio")]
    [SerializeField] private AudioClip startWeaveClip;
    [SerializeField] private AudioClip weavingLoopClip;
    [SerializeField] private AudioClip weavingIntroClip;
    [SerializeField] private AudioClip weavingOutroClip;
    [SerializeField] private AudioClip possessionClip;
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
        weaveVisualizer = GetComponent<WeaveFXScript>();
        weaveVisualizer.DisableWeave();
        possessing = false;
        vCamRotationState = 0;
        pauseMenu.SetActive(false);
        TargetingArrow.SetActive(false);

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.WeaverCheckPointPos;
        characterController.enabled = true;

        // Weaving variables
        inCombineMode = false;
        inRelocateMode = false;
        interactInput = false;

        inCutscene = movementScript.inCutscene;
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
            if (uninteract && isCurrentlyWeaving)
            {
                Uninteract();
                
                weaveVisualizer.DisableWeave();
            }

            inCutscene = movementScript.inCutscene;

        }
    }

    public IEnumerator EndWeaveAudio()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.weaveLoopingChannel, weavingOutroClip);

        yield return new WaitForSeconds(.732f);

        AudioManager.instance.StopSound(AudioManagerChannels.weaveLoopingChannel);
        yield break;
    }


    public void WeaveActivated()
    {
        interactableObject = determineInteractability();

        if (interactableObject != null)
        {
            distanceBetween = Vector3.Distance(weaveableScript.transform.position, transform.position);

            if (distanceBetween < weaveDistance)
            {
                StartCoroutine(WeaveAesthetics());
                isCurrentlyWeaving = true;               
                interactableObject.Interact();
                inRelocateMode = true;
                //characterAnimationHandler.ToggleWeaveAnim(isWeaving);
                interactableObject.Relocate();
                relocateMode.SetActive(true); // on-screen ui
            }
        }

        // if too far apart & object is weavable
        interactInput = false;
    }

    public IEnumerator WeaveAesthetics()
    {
        // vfx
        weaveVisualizer.ActivateWeave();

        // Audio
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, startWeaveClip);

        yield return new WaitForSeconds(.1f);
        AudioManager.instance.PlaySound(AudioManagerChannels.weaveLoopingChannel, weavingIntroClip);
        yield return new WaitForSeconds(.732f);
        AudioManager.instance.PlaySound(AudioManagerChannels.weaveLoopingChannel, weavingLoopClip);
        yield break;
    }

    private IInteractable determineInteractability()
    {
        playerPosition = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z);
        Vector3 rayDirection = TargetingArrow.transform.forward;

        // the actual raycast from the player, this can be used for the line render 
        //      but it takes the raycast origin and the direction of the raycast
        Ray rayPlayer = new Ray(playerPosition, rayDirection);

        //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // checks for a Weavable object within distance of Ray
        if (Physics.Raycast(rayPlayer, out hitInfo, 100, weaveObject) || Physics.Raycast(rayPlayer, out hitInfo, weaveDistance, weaveObject))
        {
            Debug.Log("Raycast hit");
            
            weaveableScript = hitInfo.collider.GetComponent<WeaveableNew>();
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            wovenObject = hitInfo.collider.gameObject;
            return interactable;
        }

        return null;
    }

    public void Uninteract()
    {
        if (interactableObject != null)
        {
            interactableObject.Uninteract();
            interactableObject = null;
            //characterAnimationHandler.ToggleWeaveAnim(isWeaving);
            StartCoroutine(EndWeaveAudio());
            relocateMode.SetActive(false); // on screen ui
            combineMode.SetActive(false); // on screen ui
            uninteract = false;
            inRelocateMode = false;
            inCombineMode = false;
            isCurrentlyWeaving = false;
        }        
    }

    public void ControllerAimTargetter(Vector2 lookDir)
    {
        if (lookDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            TargetingArrow.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            if (isCurrentlyWeaving)
            {              
                TargetingArrow.SetActive(false);
            }
            else
            {
                TargetingArrow.SetActive(true);
            }
        }
        else
        {
            TargetingArrow.SetActive(false);
        }
    }

    public void MouseAimTargetter(Vector2 lookDir)
    {
        
        TargetingArrow.SetActive(true);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 1000))
        {
            worldPosition = hitData.point;
        }

        Vector3 AdjustedVector = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);

        TargetingArrow.transform.LookAt(AdjustedVector);
    }

    void OnTriggerExit(Collider other)
    {
        //See CameraIndexScript for more information about this function
        ITriggerable trigger = other.GetComponent<ITriggerable>();
        if (trigger != null) {
            trigger.OnTrigExit(other);
        }
    }

    void OnTriggerEnter(Collider other) {
        //See CameraIndexScript for more information about this function
        ITriggerable trigger = other.GetComponent<ITriggerable>();
        if (trigger != null) {
            //Debug.Log("Hit?");
            trigger.OnTrigEnter(other);
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
        if (characterController.isGrounded && Time.timeScale != 0 && !inCutscene && canSwitch)
        {
            if (possessing == false)
            {
                //Switches to Familiar
                CameraMasterScript.instance.SwitchToFamiliarCamera();
                possessing = true;
                movementScript.active = false;
                StartCoroutine(familiarScript.ForcedDelay());

                AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, possessionClip);
            }
        }
    }

    public void SetNewPosition(Vector3 newposition, Quaternion newrotation) {
        transform.position = newposition;
        transform.rotation = newrotation;
    }

    public void Death()
    {
        transform.position = GM.WeaverCheckPointPos;

        if (GM.WeaverCheckPointNum == 0) // first checkpoint in shield puzzle - should also specify scene
        {
            respawnController.RespawnInShieldPuzzle();
        }
    }
}