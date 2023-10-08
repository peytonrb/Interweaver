using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaverController : MonoBehaviour
{
    public InputActionAsset inputs;
    [SerializeField] private Camera mainCamera;
    private InputAction possessInput; //Used for possession of familiar, or switching between weaver and familiar
    private bool possessButton; //State that checks if the possess button is being pressed
    private bool possessing; //Determines if the weaver is using possessing at the moment
    private MovementController movementController;// references the movement controller script
    private CharacterController characterController; //references the character controller component
    public FamiliarController familiarController;

    //Weave Variables
    //**********************************************************
    [Header("Weave Variables")]
    public float WeaveDistance = 12f;
    public LayerMask weaveObject;
    private bool IsWeaving;
    [SerializeField] private int WeaveModeNumbers = 1;
    [SerializeField] private InputAction interactInput;
    [SerializeField] private InputAction WeaveModeSwitch;
    [SerializeField] private InputAction UninteractInput;
    [SerializeField] private float TooCloseDistance;
    //**********************************************************

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementController = GetComponent<MovementController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Section reserved for initiating inputs
        interactInput = inputs.FindAction("Player/Interact");
        UninteractInput = inputs.FindAction("Player/Uninteract");
        WeaveModeSwitch = inputs.FindAction("Player/WeaveModeSwitch");
        possessInput = inputs.FindAction("Player/Switch");
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
        if (Time.timeScale != 0)
        {
            Weaving();

            if (!possessing)
            {
                possessButton = possessInput.WasPressedThisFrame();
                Possession();
            }

            if (familiarController.depossessing && familiarController != null)
            {
                movementController.virtualCam.m_Follow = gameObject.transform;
                possessing = false;
                familiarController.depossessing = false;
            }
        }
    }

    private void Weaving() //this method will shoot out a raycast that will see if there are objects with the weaeObject layermask and the IInteractable interface
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, weaveObject))// the value 100 is for the raycast distance
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
            if (interactable != null)
            {
                if (interactInput.WasPressedThisFrame()) //this is the interact button that is taking from the player inputs
                {
                    interactable.Interact();
                    IsWeaving = true;
                    UninteractInput.Enable();//Enables the input
                    interactInput.Disable();//disables the interactInput so  that the player can't press it multiple times
                    //weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // start weaving animations 

                }
                if (UninteractInput.WasPressedThisFrame())
                {
                    interactable.Uninteract();
                    IsWeaving = false;
                    interactInput.Enable();//renables the inputs
                    WeaveModeSwitch.Disable();//disables the weavemodeswitch inputs
                    UninteractInput.Disable();//disables the uninteract inputs
                    //weaverAnimationHandler.ToggleWeaveAnim(IsWeaving); // end weaving animations
                }
                float distanceBetween = Vector3.Distance(hitInfo.collider.transform.position, transform.position);
                if (distanceBetween > WeaveDistance || distanceBetween < TooCloseDistance)
                {
                    IsWeaving = false;
                    interactable.Uninteract();
                    interactInput.Enable();
                    WeaveModeSwitch.Disable();
                }

                switch (WeaveModeNumbers)
                {
                    case 1:
                        if (WeaveModeSwitch.WasPressedThisFrame())
                        {
                            interactable.Relocate();
                            WeaveModeNumbers += 1;
                        }
                        break;

                    case 2:
                        if (WeaveModeSwitch.WasPressedThisFrame())
                        {
                            interactable.WeaveMode();
                            WeaveModeNumbers -= 1;
                        }
                        break;
                }
                
            }
        }

        if (IsWeaving == true) //if the player is weaving an object thet will look at the object
        {
            gameObject.transform.LookAt(new Vector3(hitInfo.collider.transform.position.x, 0, hitInfo.collider.transform.position.z));
            WeaveModeSwitch.Enable();
            interactInput.Disable(); //disables the inputs
        }
    }

    private void Possession()
    {
        if (possessButton)
        {
            movementController.active = false;
            movementController.virtualCam.m_Follow = familiarController.transform;
            possessing = true;
            StartCoroutine(familiarController.ForcedDelay());
        }
    }
}
