using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaveableNew : MonoBehaviour, IInteractable, ICombineable
{
    [Header("Weavables")]
    [CannotBeNullObjectField][SerializeField] private Rigidbody rb;
    [SerializeField] private float hoveringValue = 1f;
    [CannotBeNullObjectField][SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] public WeaveInteraction weaveInteraction;
    public int ID;
    public bool canRotate;
    public bool canCombine { get; private set; }
    private bool startFloating;
    private bool relocate;
    private bool inWeaveMode;
    public bool isWoven;
    public float rotationSpeed = 0.5f;
    public bool canBeRelocated = true;

    [Header("Snapping feature")]
    public GameObject[] myTransformPoints;
    public GameObject nearestPoint;
    private float distance;
    [SerializeField] private float snapDistance;
    [SerializeField] private float nearestDistance;

    [Header("Floating Islands + Crystals")]
    private bool onFloatingIsland;
    private GameObject snapPoint;

    [Header("VFX")]
    public Material originalMat; // accessed by WeaveFX

    [Header("Respawn")] // accessed by RespawnController
    public bool isCombined;
    public Vector3 spawnPos;
    public Quaternion spawnRotation;

    [Header("Inputs")]
    [CannotBeNullObjectField] public InputActionAsset inputs;
    private InputAction combineInput;

    [Header("References")]
    public WeaveableNew weaveableScript; // has to be public for respawn / attempting to make this obsolete
    public List<WeaveableNew> wovenObjects;
    [SerializeField] private PlayerController player;
    private GameObject wovenFloatingIsland;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        weaveableScript = gameObject.GetComponent<WeaveableNew>();
        isCombined = false;
        onFloatingIsland = false;
        originalMat = gameObject.GetComponent<Renderer>().material;
        spawnPos = transform.position;
        spawnRotation = transform.rotation;
    }

    void Update()
    {
        if (startFloating)
        {
            if (gameObject.tag != "FloatingIsland")
            {
                transform.position = transform.position + new Vector3(0, hoveringValue, 0);
            }

            startFloating = false;
        }

        if (isWoven)
        {
            MovingWeaveMouse(); // fix drag on object here  
        }

        if (onFloatingIsland && gameObject.tag == "Breakable")
        {
            relocate = false;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            rb.velocity = new Vector3(snapPoint.transform.position.x - rb.position.x, rb.position.y, snapPoint.transform.position.z - rb.position.z);
            float distanceToSnap = Vector3.Distance(rb.position, snapPoint.transform.position);

            if (distanceToSnap <= 2f) // if crystal is close enough to snap point
            {
                gameObject.transform.SetParent(wovenFloatingIsland.transform);
                player.uninteract = true;

                if (TryGetComponent<CrystalScript>(out CrystalScript crystal))
                {
                    if (onFloatingIsland)
                    {
                        weaveInteraction.OnWeave(wovenFloatingIsland, gameObject);
                    }
                }

                rb.isKinematic = true;
                canBeRelocated = false;
                isWoven = true;
                onFloatingIsland = false;
            }
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
                if (canBeRelocated)
                {
                    canRotate = true;
                    rb.velocity = new Vector3(raycastHit.point.x - rb.position.x, transform.position.y - rb.position.y, raycastHit.point.z - rb.position.z);
                    rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    //this freezes the Y position so that the combined objects won't drag it down because of gravity and it freezes in all rotation so it won't droop because of the gravity  from the objects
                }
            }

            if (inWeaveMode)
            {
                ICombineable combineable = raycastHit.collider.GetComponent<ICombineable>();
                canRotate = true;

                if (combineable != null && !weaveableScript.canCombine)
                {
                    canCombine = true;
                    //inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed += OnCombineInput;
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

            wovenObjects.Add(collision.gameObject.GetComponent<WeaveableNew>());
        }
    }

    public void CallRotate(Vector3 dir, float angle)
    {
        StartCoroutine(Rotate(dir, angle));
    }

    IEnumerator Rotate(Vector3 axis, float angle)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);
        float elapsed = 0.0f;

        while (elapsed < rotationSpeed && canRotate)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / rotationSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = to;
    }
    //**********************************************************************************


    //this section is from the IInteractable interface
    //********************************************************************
    public void Interact()
    {
        Debug.Log("This is interactable");
        startFloating = true;
        transform.rotation = Quaternion.identity;

        if (!isCombined)
        {
            wovenObjects.Add(this.GetComponent<WeaveableNew>());
        }

        // if objects are combined, vfx needs to show up for both
        if (this.isCombined)
        {
            foreach (WeaveableNew weaveable in wovenObjects)
            {
                player.weaveVisualizer.WeaveableSelected(weaveable.gameObject);
            }
        }
        else
        {
            player.weaveVisualizer.WeaveableSelected(gameObject);
        }
    }

    public void Uninteract()
    {
        if (isWoven)
        {
            player.floatingIslandCrystal = false;
            Debug.Log("this is now not woven");
            rb.isKinematic = false;
            rb.useGravity = true;
            player.inRelocateMode = false;
            player.inCombineMode = false;
            relocate = false;
            inWeaveMode = false;
            isWoven = false;
            startFloating = false;
            canCombine = false;
            canRotate = false;
            rb.constraints = RigidbodyConstraints.None;
            player.weaveVisualizer.StopAura(gameObject);

            // disables vfx on all woven objects
            foreach (WeaveableNew weaveable in wovenObjects)
            {
                player.weaveVisualizer.StopAura(weaveable.gameObject);
            }
        }
    }

    public void Relocate()
    {
        if (canBeRelocated)
        {
            isWoven = true;
            rb.useGravity = false;
            relocate = true;
            inWeaveMode = false;
            rb.isKinematic = false;
            canRotate = true;
            //Debug.Log("Relocate Mode");
        }
    }

    public void WeaveMode()
    {
        relocate = false;
        inWeaveMode = true;
        rb.isKinematic = true;
        canRotate = true;
        //Debug.Log("in weave Mode");
    }
    //********************************************************************

    public void OnCombineInput()
    {
        if (weaveableScript.ID == ID && !weaveableScript.isWoven && canCombine)
        {
            Debug.Log("OnCombineInput");
            player.weaveVisualizer.WeaveableSelected(weaveableScript.gameObject);

            Combine();
            isCombined = true;
            weaveableScript.isCombined = true;
        }
    }

    //this section is from the IInteractable interface
    //********************************************************************
    public void Uncombine()
    {
        isCombined = false;
        Debug.Log("This is the Uncombine code");
        Destroy(GetComponent<FixedJoint>());
        canCombine = false;
        canRotate = false;

        if (weaveableScript != null)
        {
            weaveableScript.rb.useGravity = true;
            weaveableScript.rb.constraints = RigidbodyConstraints.None;
            weaveableScript.rb.freezeRotation = false;
        }

        player.inRelocateMode = false;
        player.inCombineMode = false;
        player.uninteract = true;
        wovenObjects.Clear();
    }

    public void Combine()
    {
        Debug.Log("This is the combine code");
        canCombine = true;

        if (weaveableScript.canBeRelocated)
        {
            weaveableScript.startFloating = true;
            Snapping();
        }
        else
        {
            if (weaveableScript.gameObject.tag == "FloatingIsland")
            {
                onFloatingIsland = true;
                player.floatingIslandCrystal = true; // for input manager

                wovenFloatingIsland = weaveableScript.gameObject;
            }

            snapPoint = weaveableScript.gameObject.transform.GetChild(0).gameObject;
        }

        weaveableScript.rb.useGravity = false;

    }
    //********************************************************************

    //the snapping method
    void Snapping()
    {
        weaveableScript.nearestDistance = Mathf.Infinity; //this is made the be infinite so that when it calculates the distance it wouldn't cap itself
        GameObject closestPoint = null;
        for (int i = 0; i < myTransformPoints.Length; i++)
        {
            distance = Vector3.Distance(weaveableScript.transform.position, myTransformPoints[i].transform.position);
            if (distance < nearestDistance)
            {
                closestPoint = myTransformPoints[i];
                nearestDistance = distance;
                Debug.Log("this is the distance between points " + distance + ",this is the closestpoint" + myTransformPoints[i]);
            }
        }
        weaveableScript.nearestPoint = closestPoint; //this two variables are stored outside of the for loop so it wouldn't reset and get the latest element
        weaveableScript.nearestDistance = nearestDistance;
        weaveableScript.rb.velocity = weaveableScript.nearestPoint.transform.position - weaveableScript.transform.position;

        if (weaveableScript.nearestDistance < weaveableScript.snapDistance)
        {
            weaveableScript.rb.transform.position = new Vector3(weaveableScript.nearestPoint.transform.position.x, weaveableScript.nearestPoint.transform.position.y - weaveableScript.transform.position.y, weaveableScript.nearestPoint.transform.position.z); ;
        }
    }
}