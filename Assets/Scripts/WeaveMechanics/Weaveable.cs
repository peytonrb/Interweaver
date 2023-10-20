using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Weaveable : MonoBehaviour, IInteractable, ICombineable

{

    [Header("Weaveable's variables")]
    [SerializeField] private Rigidbody rb; //this is grabbing the reffrence from the  rigidbody if we're going to use gravity (this can be deleted but just make sure that when the uninteract activates do the inverse)
    [SerializeField] private float HoveringValue; // thee value for hovering over the floor    
    [SerializeField] private Camera mainCamera; // grabbing the main camera
    [SerializeField] private LayerMask LayerstoHit; //a layermask
    [SerializeField] private float distance = 12;
    [SerializeField] private float TooCloseDistance = 6; //this is for if the weaveable get's too close to the weaver  
    public int ID; //an ID for objects
    public int specialID; //this is forthe combineable objects that's going to be used for puzzles
    private Vector3 WeaveablePos;
    private bool HasJoint;
    public bool CanCombine;
    public bool CanWeave;
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
        rb = GetComponent<Rigidbody>();
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
           transform.position = transform.position + new Vector3 (0, HoveringValue*Time.deltaTime, 0);
            Startfloating = false;          
        }
      
        if (Woven) 
        {
            MovingWeaveMouse();
        }
      
    }


    void FreezeDistance()
    {
        float distanceBetween = Vector3.Distance(PlayerPrefab.transform.position, transform.position);
        if (distanceBetween > distance || distanceBetween < TooCloseDistance)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }
        else if (distanceBetween >= TooCloseDistance && distanceBetween <= distance)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void MovingWeaveMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100, LayerstoHit))
        {
            if (relocate)
            {
                rb.velocity = new Vector3(raycastHit.point.x - rb.position.x, transform.position.y - rb.position.y, raycastHit.point.z - rb.position.z);
                FreezeDistance();
                rb.freezeRotation = true;
            }
            if (Weave)
            {
                //transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y , raycastHit.point.z));
                WeaveWeaveables();
            }
        }
    }

    void WeaveWeaveables() //method for weaving the weaveables
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, LayerstoHit))// the value 100 is for the raycast distance
        {
            weaveableScript = hitInfo.collider.GetComponent<Weaveable>();
            ICombineable combineable = hitInfo.collider.GetComponent<ICombineable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
            if (combineable != null && !weaveableScript.CanCombine)// this is the band aid solution will need a more concrete solution later on
            {
                inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed += OnCombineInput;
                inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed += OnUncombineInput;
            }
        }
       
    }

    void OnCollisionEnter(Collision collision)
    {
        weaveableScript = GetComponent<Weaveable>();
        if (collision.gameObject.GetComponent<Rigidbody>() != null && !HasJoint && weaveableScript.CanCombine && weaveableScript.ID == ID)
        {
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            HasJoint = true;
            weaveableScript.rb.velocity = new Vector3(0, 0, 0);
            weaveableScript.rb.useGravity = true;
        }

        else
        {
            rb.velocity = new Vector3(0,0,0);
            weaveableScript.rb.useGravity = true;
        }
    }

        //this section is from the IInteractable interface
        //********************************************************************
        public void Interact()
        {
          Debug.Log("This is interactable");
          rb.useGravity = false;
          Startfloating = true;
          Woven = true;
         rb.isKinematic = true;
         }
    
    public void Uninteract()
    {
        Debug.Log("this is now not woven");
        rb.isKinematic = false;
        rb.useGravity = true;
        relocate = false;
        Weave = false;
        Woven = false;
        Startfloating = false;
        rb.constraints = RigidbodyConstraints.None;
        inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed -= OnCombineInput;
        inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed -= OnUncombineInput;
    }

    public void Relocate()
    {
        relocate = true;
        Weave = false;
        rb.isKinematic = false;
        Debug.Log("Relocate Mode");       
    }

    public void WeaveMode()
    {
        relocate = false;
        Weave = true;
        rb.isKinematic = true; 
        Debug.Log("weave Mode");       
    }
    //********************************************************************
    private void OnUncombineInput(InputAction.CallbackContext context)
    {
        if (weaveableScript.ID == ID && HasJoint)
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
        Destroy(GetComponent<FixedJoint>());
        CanCombine = false;
        weaveableScript.rb.useGravity = true;
    }

    public void Combine()
    {
        Debug.Log("This is the combine code");
        
        CanCombine = true;
        weaveableScript.rb.velocity = transform.position - weaveableScript.rb.transform.position;
        weaveableScript.rb.useGravity = false;
    }
    //********************************************************************
}
