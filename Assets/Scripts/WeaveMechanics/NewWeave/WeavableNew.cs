using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaveableNew : MonoBehaviour, IInteractable, ICombineable
{
    [Header("Weavables")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float hoveringValue = 1f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private float distance = 12f;
    [SerializeField] private float tooCloseDistance = 6f;
    [SerializeField] public WeaveInteraction weaveInteraction;
    public int ID;
    public bool canRotate;
    public bool canCombine { get; private set; }
    public bool canWeave { get; private set; }
    private bool startFloating;
    private bool relocate;
    private bool inWeaveMode;
    public bool isWoven;

    [Header("Inputs")]
    public InputActionAsset inputs;
    private InputAction combineInput;

    [Header("References")]
    public Transform PlayerPrefab;
    private WeaveableNew weaveableScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canRotate = false;
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
        if (startFloating)
        {
            transform.position = transform.position + new Vector3(0, hoveringValue, 0);
            startFloating = false;
        }

        if (isWoven)
        {
            MovingWeaveMouse();
        }

        if (canRotate)
        {
            // this finds the the different action map called weaveableObject and then subscribes to the method 
            inputs.FindActionMap("weaveableObject").FindAction("RotateCW").performed += OnRotateCWInput; 
            inputs.FindActionMap("weaveableObject").FindAction("RotateCtrW").performed += OnRotateCtrWInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateUP").performed += OnRotateUPInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateDOWN").performed += OnRotateDownInput;
        }

        else if (!canRotate)
        {
            // this finds the the different action map called weaveableObject and then unsubscribes to the method, 
            //      it's there so that it can prevent memory leakage 
            inputs.FindActionMap("weaveableObject").FindAction("RotateCW").performed -= OnRotateCWInput; 
            inputs.FindActionMap("weaveableObject").FindAction("RotateCtrW").performed -= OnRotateCtrWInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateUP").performed -= OnRotateUPInput;
            inputs.FindActionMap("weaveableObject").FindAction("RotateDOWN").performed -= OnRotateDownInput;
        }
    }

    // this is for the distance check if the weaveable is too close or too far away
    void DistanceCheck()
    {
        float distanceBetween = Vector3.Distance(PlayerPrefab.transform.position, transform.position);
        if (distanceBetween <= tooCloseDistance)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }

        // this uninteracts the weaveable object if it's too far away
        if (distanceBetween >= distance) 
        {
            Uninteract();
        }
        // it will start moving if it's in the right distance
        else if (distanceBetween >= tooCloseDistance && distanceBetween <= distance) 
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    // this method is for using the mouse to move around the object
    private void MovingWeaveMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // this shoots a raycast from the camera to the 3D plane to get the position of the mouse
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100, layersToHit)) 
        {
            weaveableScript = raycastHit.collider.GetComponent<WeaveableNew>();

            if (relocate)
            {
                rb.velocity = new Vector3(raycastHit.point.x - rb.position.x, transform.position.y - rb.position.y, raycastHit.point.z - rb.position.z);
                DistanceCheck();

                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; //this freezes the Y position so that the combined objects won't drag it down because of gravity and it freezes in all rotation so it won't droop because of the gravity  from the objects
            }
            if (inWeaveMode)
            {
                DistanceCheck();
                ICombineable combineable = raycastHit.collider.GetComponent<ICombineable>();
                canRotate = true;

                // this is the band aid solution will need a more concrete solution later on
                if (combineable != null && !weaveableScript.canCombine)
                {
                    //this finds the the different action map called weaveableObject and then subscribes to the method 
                    inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed += OnCombineInput; 
                    inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed += OnUncombineInput;
                }
            }
        }
    }

     // this will need to be refactored later but for now when the weaveable collides with another 
     //     weaveable it will make a fixed joint component and then add itself as the rigidbody to be connected
    void OnCollisionEnter(Collision collision)
    {
        weaveableScript = GetComponent<WeaveableNew>();
        if (collision.gameObject.GetComponent<Rigidbody>() != null && weaveableScript.canCombine && weaveableScript.ID == ID)
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = collision.rigidbody;
            collision.rigidbody.useGravity = true;

            if (weaveInteraction != null)
            {
                weaveInteraction.OnWeave(collision.gameObject);
            }
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
        while (elapsed < duration && canRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        canRotate = false;
    }

    IEnumerator RotatectrW(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && canRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        canRotate = false;
    }

    IEnumerator RotateUP(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && canRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        canRotate = false;
    }

    IEnumerator Rotatedown(Vector3 axis, float angle, float duration = 1.0f)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration && canRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        canRotate = false;
    }
    //**********************************************************************************


    //this section is from the IInteractable interface
    //********************************************************************
    public void Interact()
    {
        Debug.Log("This is interactable");
        startFloating = true;
        transform.rotation = Quaternion.identity;
    }

    public void Uninteract()
    {
        if (isWoven)
        {
            Debug.Log("this is now not woven");
            rb.isKinematic = false;
            rb.useGravity = true;
            relocate = false;
            inWeaveMode = false;
            isWoven = false;
            startFloating = false;
            canCombine = false;
            canRotate = false;
            rb.constraints = RigidbodyConstraints.None;
            inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed -= OnCombineInput; //this finds the the different action map called weaveableObject and then unsubscribes to the method, this is there so that there won't be any memory leakage
            inputs.FindActionMap("weaveableObject").FindAction("UncombineAction").performed -= OnUncombineInput;
        }

    }

    public void Relocate()
    {
        isWoven = true;
        rb.useGravity = false;
        relocate = true;
        inWeaveMode = false;
        rb.isKinematic = false;
        canRotate = false;
        Debug.Log("Relocate Mode");
    }

    public void WeaveMode()
    {
        relocate = false;
        inWeaveMode = true;
        rb.isKinematic = true;
        canRotate = true;
        Debug.Log("weave Mode");
    }
    //********************************************************************

    private void OnUncombineInput(InputAction.CallbackContext context)
    {
        Uncombine();
    }

    private void OnCombineInput(InputAction.CallbackContext context)
    {
        if (weaveableScript.ID == ID && !weaveableScript.isWoven)
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
        canCombine = false;
        weaveableScript.rb.useGravity = true;
        weaveableScript.rb.constraints = RigidbodyConstraints.None;
        weaveableScript.rb.freezeRotation = false;
    }

    public void Combine()
    {
        Debug.Log("This is the combine code");
        weaveableScript.startFloating = true;
        canCombine = true;
        weaveableScript.rb.velocity = new Vector3(transform.position.x - weaveableScript.rb.transform.position.x, 0, transform.position.z - weaveableScript.rb.transform.position.z);
        weaveableScript.rb.useGravity = false;
    }
    //********************************************************************
}