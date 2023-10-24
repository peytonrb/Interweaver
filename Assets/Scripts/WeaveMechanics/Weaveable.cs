using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Weaveable : MonoBehaviour, IInteractable, ICombineable

{

    [Header("Weaveable's variables")]
    [SerializeField] private Rigidbody rigidbody; //this is grabbing the reffrence from the  rigidbody if we're going to use gravity (this can be deleted but just make sure that when the uninteract activates do the inverse)
    [SerializeField] private float HoveringValue; // thee value for hovering over the floor    
    [SerializeField] private Camera mainCamera; // grabbing the main camera
    [SerializeField] private LayerMask LayerstoHit; //a layermask
    [SerializeField] private float distance = 12;
    [SerializeField] private float TooCloseDistance = 6; //this is for if the weaveable get's too close to the weaver  
    public int ID; //an ID for objects
    public int specialID; //this is forthe combineable objects that's going to be used for puzzles
    private Vector3 WeaveablePos;
    public bool HasJoint;
    public bool CanCombine{get; private set;}
    public bool CanWeave { get; private set; }
    private bool Startfloating; //a bool to detect if the weaveable is interacted and will start floating
    private bool relocate; // bool for relocate
    private bool Weave; //bool for weaving the weaveables
    public bool Woven;//bool for starting the weave

    //inputs
    //**************************************
    public InputActionAsset inputs;
    private InputAction combineInput;
    //**************************************

    //refrence
    //**************************************
    private Weaveable weaveableScript;
    //**************************************

    private Vector2 weave;
    public Transform PlayerPrefab;
    private Vector3 direction;

    void start()
    {
        rigidbody = GetComponent<Rigidbody>();
        HasJoint = false;
    }


    void OnEnable()
    {
        inputs.FindActionMap("weaveableObject").Enable();
    }
    void OnDisable()
    {
        inputs.FindActionMap("weaveableObject").Disable();
    }

    void Update()
    {       
        if (Startfloating) 
        {
           transform.position = transform.position + new Vector3 (0, HoveringValue, 0);
            Startfloating = false;          
        }
      
        if (Woven) 
        {
            MovingWeaveMouse();
        }
      
    }


    void DistanceCheck() // this is for the distance check if the weaveable is too close or too far away
    {
        float distanceBetween = Vector3.Distance(PlayerPrefab.transform.position, transform.position);
        if ( distanceBetween <= TooCloseDistance) // this freezes the weaveable object in place
        {
            rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        }
        if (distanceBetween >= distance) //this uninteracts the weaveable object if it's too far away
        {
            Uninteract();
        }
        else if (distanceBetween >= TooCloseDistance && distanceBetween <= distance) // it will start moving if it's in the right distance
        {
            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    void MovingWeaveMouse() //this method is for using the mouse to move around the object
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100, LayerstoHit)) //  this shoots a raycast from the camera to the 3D plane to get the position of the mouse
        {
            weaveableScript = raycastHit.collider.GetComponent<Weaveable>();

            if (relocate)
            {
                rigidbody.velocity = new Vector3(raycastHit.point.x - rigidbody.position.x, transform.position.y - rigidbody.position.y, raycastHit.point.z - rigidbody.position.z);
                DistanceCheck();
                rigidbody.freezeRotation = true;
            }
            if (Weave)
            {
                DistanceCheck();
                ICombineable combineable = raycastHit.collider.GetComponent<ICombineable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
                if (combineable != null && !weaveableScript.CanCombine)// this is the band aid solution will need a more concrete solution later on
                {
                    inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed += OnCombineInput;
                    inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed += OnUncombineInput;
                }
            }
        }
    }


    void OnCollisionEnter(Collision collision) //this will need to be refactored later but for now when the weaveable collides with another weaveable it will stick
    {
        weaveableScript = GetComponent<Weaveable>();
        if (collision.gameObject.GetComponent<Rigidbody>() != null && !HasJoint && weaveableScript.CanCombine && weaveableScript.ID == ID)
        {
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            HasJoint = true;
        }

        else if (weaveableScript.ID != ID && !weaveableScript.CanCombine && !CompareTag("Player") || HasJoint)
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.useGravity = true;
            HasJoint = false;
        }

        
    }

        //this section is from the IInteractable interface
        //********************************************************************
        public void Interact()
        {
         Debug.Log("This is interactable");
         Startfloating = true;
        }
    
    public void Uninteract()
    {
        Debug.Log("this is now not woven");
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        relocate = false;
        Weave = false;
        Woven = false;
        Startfloating = false;
        CanCombine = false;
        rigidbody.constraints = RigidbodyConstraints.None;
        inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed -= OnCombineInput;
        inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed -= OnUncombineInput;
    }

    public void Relocate()
    {
        Woven = true;
        rigidbody.useGravity = false;
        relocate = true;
        Weave = false;
        rigidbody.isKinematic = false;
        Debug.Log("Relocate Mode");       
    }

    public void WeaveMode()
    {
        relocate = false;
        Weave = true;
        rigidbody.isKinematic = true; 
        Debug.Log("weave Mode");       
    }
    //********************************************************************
    private void OnUncombineInput(InputAction.CallbackContext context)
    {
        if (HasJoint)
        {
            Uncombine();
        }
    }

    private void OnCombineInput(InputAction.CallbackContext context)
    {
        if (weaveableScript.ID == ID)
        {
            Combine();
        }       
    }

    //this section is from the IInteractable interface
    //********************************************************************

    public void Uncombine()
    {
        Debug.Log("This is the Uncombine code");
        HasJoint = false;
        weaveableScript.HasJoint = false;
        Destroy(GetComponent<FixedJoint>());
        CanCombine = false;
        weaveableScript.rigidbody.useGravity = true;
    }

    public void Combine()
    {
        if (!HasJoint) 
        {
            Debug.Log("This is the combine code");
            weaveableScript.Startfloating = true;
            CanCombine = true;
            weaveableScript.HasJoint = true;
            weaveableScript.rigidbody.velocity = new Vector3(transform.position.x - weaveableScript.rigidbody.transform.position.x,0, transform.position.z - weaveableScript.rigidbody.transform.position.z);
            weaveableScript.rigidbody.useGravity = false;
        }
        
    }
    //********************************************************************
}
