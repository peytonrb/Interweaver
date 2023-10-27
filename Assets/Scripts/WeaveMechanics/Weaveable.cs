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
    public bool CanRotate;//a new bool for  the rotate function
    public bool CanCombine { get; private set; } //this is ia private set so that no other script could accidentally change this bool
    public bool CanWeave { get; private set; } //this is ia private set so that no other script could accidentally change this bool
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
        CanRotate = false;
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
            transform.position = transform.position + new Vector3(0, HoveringValue, 0);
            Startfloating = false;
        }

        if (Woven)
        {
            MovingWeaveMouse();
        }

        if (CanRotate)
        {
            inputs.FindActionMap("weaveableObject").FindAction("RotateCW").performed += OnRotateCWInput; //this finds the the different action map called weaveableObject and then subscribes to the method 
            inputs.FindActionMap("weaveableObject").FindAction("RotateCtrW").performed += OnRotateCtrWInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateUP").performed += OnRotateUPInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateDOWN").performed += OnRotateDownInput;
        }

        else if (!CanRotate)
        {
            inputs.FindActionMap("weaveableObject").FindAction("RotateCW").performed -= OnRotateCWInput; //this finds the the different action map called weaveableObject and then unsubscribes to the method, it's there so theat it can prevent memory leakage 
            inputs.FindActionMap("weaveableObject").FindAction("RotateCtrW").performed -= OnRotateCtrWInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateUP").performed -= OnRotateUPInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateDOWN").performed -= OnRotateDownInput;
        }
    }


    void DistanceCheck() // this is for the distance check if the weaveable is too close or too far away
    {
        float distanceBetween = Vector3.Distance(PlayerPrefab.transform.position, transform.position);
        if (distanceBetween <= TooCloseDistance) // this freezes the weaveable object in place
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
                 
                rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; //this freezes the Y position so that the combined objects won't drag it down because of gravity and it freezes in all rotation so it won't droop because of the gravity  from the objects
            }
            if (Weave)
            {
                DistanceCheck();
                ICombineable combineable = raycastHit.collider.GetComponent<ICombineable>(); //this will detect if the object it hits has the IInteractable interface  and will do some stuff
                CanRotate = true;
                if (combineable != null && !weaveableScript.CanCombine)// this is the band aid solution will need a more concrete solution later on
                {
                    inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed += OnCombineInput; //this finds the the different action map called weaveableObject and then subscribes to the method 
                    inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed += OnUncombineInput;
                }
            }
        }
    }


    void OnCollisionEnter(Collision collision) //this will need to be refactored later but for now when the weaveable collides with another weaveable it will make a fixed joint component and then add itself as the rigidbody to be connected
    {
        weaveableScript = GetComponent<Weaveable>();
        if (collision.gameObject.GetComponent<Rigidbody>() != null && weaveableScript.CanCombine && weaveableScript.ID == ID)
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = collision.rigidbody;
            collision.rigidbody.useGravity = true;
            
        }

      
    }

    //section for rotate function
   //**********************************************************************************
    private void OnRotateCWInput(InputAction.CallbackContext context) //
    {
        OnRotateCW();
    }
    private void OnRotateCtrWInput(InputAction.CallbackContext context)
    {
        OnRotateCtrW();
    }
    private void OnRotateUPInput(InputAction.CallbackContext context)
    {
        OnRotateUP();
    }
    private void OnRotateDownInput(InputAction.CallbackContext context)
    {
        OnRotateDown();
    }

    private void OnRotateCW()
    {
        StartCoroutine(Rotate(Vector3.up, 45, 1.0f));
    }
    private void OnRotateCtrW()
    {
        StartCoroutine(RotatectrW(Vector3.up, -45, 1.0f));
    }
    private void OnRotateUP()
    {
        StartCoroutine(RotateUP(Vector3.forward, 45, 1.0f));
    }
    private void OnRotateDown()
    {
        StartCoroutine(Rotatedown(Vector3.forward, -45, 1.0f));
    }

    IEnumerator Rotate(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && CanRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        CanRotate = false;
    }

    IEnumerator RotatectrW(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && CanRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        CanRotate = false;
    }

    IEnumerator RotateUP(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && CanRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        CanRotate = false;
    }

    IEnumerator Rotatedown(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && CanRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        CanRotate = false;
    }


    //**********************************************************************************


    //this section is from the IInteractable interface
    //********************************************************************
    public void Interact()
    {
        Debug.Log("This is interactable");
        Startfloating = true;
        transform.rotation = Quaternion.identity;
    }

    public void Uninteract()
    {
        if (Woven)
        {
            Debug.Log("this is now not woven");
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            relocate = false;
            Weave = false;
            Woven = false;
            Startfloating = false;
            CanCombine = false;
            CanRotate = false;
            rigidbody.constraints = RigidbodyConstraints.None;
            inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed -= OnCombineInput; //this finds the the different action map called weaveableObject and then unsubscribes to the method, this is there so that there won't be any memory leakage
            inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed -= OnUncombineInput;
        }
       
    }

    public void Relocate()
    {
        Woven = true;
        rigidbody.useGravity = false;
        relocate = true;
        Weave = false;
        rigidbody.isKinematic = false;
        CanRotate = false;
        Debug.Log("Relocate Mode");
    }

    public void WeaveMode()
    {
        relocate = false;
        Weave = true;
        rigidbody.isKinematic = true;
        CanRotate = true;
        Debug.Log("weave Mode");
    }
    //********************************************************************
    private void OnUncombineInput(InputAction.CallbackContext context)
    {               
      Uncombine();       
    }

    private void OnCombineInput(InputAction.CallbackContext context)
    {
        if (weaveableScript.ID == ID && !weaveableScript.Woven)
        {
            Combine();
        }
    }

    //this section is from the IInteractable interface
    //********************************************************************

    public void Uncombine()
    {
        Debug.Log("This is the Uncombine code");    
        Destroy(GetComponent<FixedJoint>());
        CanCombine = false;
        weaveableScript.rigidbody.useGravity = true;
        weaveableScript.rigidbody.constraints = RigidbodyConstraints.None;
        weaveableScript.rigidbody.freezeRotation = false;
    }

    public void Combine()
    {              
            Debug.Log("This is the combine code");
            weaveableScript.Startfloating = true;
            CanCombine = true;
            weaveableScript.rigidbody.velocity = new Vector3(transform.position.x - weaveableScript.rigidbody.transform.position.x, 0, transform.position.z - weaveableScript.rigidbody.transform.position.z);
            weaveableScript.rigidbody.useGravity = false;        
    }
    //********************************************************************
}