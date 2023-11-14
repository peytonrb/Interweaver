using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaveableNew : MonoBehaviour, IInteractable, ICombineable
{
    [Header("Weavables")]
    [CannotBeNullObjectField][SerializeField] private Rigidbody rb;
    [SerializeField] private float hoveringValue = 4f;
    [CannotBeNullObjectField][SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] public WeaveInteraction weaveInteraction;
    public int ID; //THIS IS INDENTIFIER FOR DAYBLOCK COMBINING
    public bool canRotate;
    public bool canCombine = true;
    private bool startFloating;
    private bool relocate;
    private bool inWeaveMode;
    public bool isWoven;
    public bool isHovering = false;
    public float rotationSpeed = 0.5f;
    public bool canBeRelocated = true;
    private Vector3 worldPosition;
    [SerializeField] private GameObject TargetingArrow;
    private bool isRotating = false;
    private LayerMask originalLayerMask;

    // for combining multiple objects
    public bool isParent;
    public WeaveableNew parentWeaveable;

    [Header("Snapping feature")]
    public GameObject[] myTransformPoints;
    public GameObject nearestPoint;
    public GameObject myNearestPoint;
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
    public Vector3 startPos;
    public Quaternion startRot;

    [Header("Inputs")]
    [CannotBeNullObjectField] public InputActionAsset inputs;
    private InputAction combineInput;

    [Header("References")]
    public WeaveableNew weaveableScript; // attempting to make this obsolete
    public List<WeaveableNew> wovenObjects;
    [SerializeField] private PlayerController player;
    private GameObject wovenFloatingIsland;
    [SerializeField] private GameObject dayblockPuzzle;
    private DayblockPuzzleManager dpm;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        weaveableScript = gameObject.GetComponent<WeaveableNew>();
        if (dayblockPuzzle != null)
        {
            dpm = dayblockPuzzle.GetComponent<DayblockPuzzleManager>();
        }
        isCombined = false;
        onFloatingIsland = false;
        originalMat = gameObject.GetComponent<Renderer>().material;
        if (canBeRelocated)
        {
            TargetingArrow.SetActive(false);
        }

        // for respawnables
        startPos = transform.localPosition;
        startRot = transform.rotation;

        originalLayerMask = gameObject.layer;
    }

    void Update()
    {
        if (startFloating)
        {
            isHovering = true;
            if (gameObject.tag != "FloatingIsland")
            {
                transform.position = transform.position + new Vector3(0, hoveringValue, 0);
            }

            startFloating = false;
        }

        if (isHovering)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, new Vector3(0f, -90f, 0f), out hit, 2f))
            {
                Vector3 rayDirection = Vector3.down;
                Debug.Log(-rayDirection * Physics.gravity.y * 2f);
                rb.AddForce(-rayDirection * Physics.gravity.y * 2f);
            }
        }

        if (isWoven)
        {
            if (!InputManagerScript.instance.isGamepad)
                MovingWeaveMouse();
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
                    TargetingArrow.SetActive(true);
                    canRotate = true;
                    rb.velocity = new Vector3(raycastHit.point.x - rb.position.x, transform.position.y - rb.position.y, raycastHit.point.z - rb.position.z);
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    //this freezes the Y position so that the combined objects won't drag it down because of gravity and it freezes in all rotation so it won't droop because of the gravity  from the objects

                    //Targeting Arrow
                    TargetingArrow.SetActive(true);

                    RaycastHit hitData;
                    if (Physics.Raycast(ray, out hitData, 1000))
                    {
                        float rayDistance = Vector3.Distance(gameObject.transform.position, hitData.point);

                        if (rayDistance > 50f) // drops object if you hover over the void
                        {
                            Uncombine();
                        }
                        else
                        {
                            worldPosition = hitData.point;
                        }
                    }

                    Vector3 AdjustedVector = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);

                    TargetingArrow.transform.LookAt(AdjustedVector);
                }
            }

            if (inWeaveMode)
            {
                ICombineable combineable = raycastHit.collider.GetComponent<ICombineable>();
                canRotate = true;

                TargetingArrow.SetActive(false);

                if (combineable != null && !weaveableScript.canCombine)
                {
                    canCombine = true;
                    //inputs.FindActionMap("weaveableObject").FindAction("CombineAction").performed += OnCombineInput;
                }
            }
        }
    }

    public void MovingWeaveController(Vector2 lookDir)
    {
        //Add a raycast coming from directional arrow
        float targetAngle = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

        TargetingArrow.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        Vector3 rayDirection = TargetingArrow.transform.forward;

        // the actual raycast from the player, this can be used for the line render 
        //      but it takes the raycast origin and the direction of the raycast
        Ray arrowRay = new Ray(transform.position, rayDirection);
        RaycastHit hitInfo;
        if (relocate)
        {
            if (canBeRelocated)
            {
                canRotate = true;
                Debug.Log(lookDir);

                //change this to send velocity in direction of (RS)
                if (lookDir.magnitude >= 0.1f)
                {
                    rb.velocity = rayDirection * 6;
                    TargetingArrow.SetActive(true);
                }
                else
                {

                    rb.velocity = Vector3.zero;
                    TargetingArrow.SetActive(false);
                }


                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                //this freezes the Y position so that the combined objects won't drag it down because of gravity and it freezes in all rotation so it won't droop because of the gravity  from the objects

            }
        }

        if (inWeaveMode)
        {
            if (Physics.Raycast(arrowRay, out hitInfo, 100, layersToHit))
            {
                ICombineable combineable = hitInfo.collider.GetComponent<ICombineable>();
                canRotate = true;

                TargetingArrow.SetActive(false);

                if (combineable != null && !weaveableScript.canCombine)
                {
                    canCombine = true;
                }
            }
        }
    }

    // this will need to be refactored later but for now when the weaveable collides with another 
    //     weaveable it will make a fixed joint component and then add itself as the rigidbody to be connected
    void OnCollisionEnter(Collision collision)
    {
        weaveableScript = GetComponent<WeaveableNew>();

        //Return if I'm a floating island and hitting my own crystal
        if (TryGetComponent<FloatingIslandScript>(out FloatingIslandScript islandScript))
        {
            if (islandScript.myCrystal == collision.gameObject.TryGetComponent<CrystalScript>(out CrystalScript crystalScript))
            {
                return;
            }
        }

        // for objects that are being connected that are NOT the parent
        if (collision.gameObject.GetComponent<Rigidbody>() != null && canCombine && !isParent && weaveableScript.ID == ID)
        {
            WeaveableNew[] allWeaveables = FindObjectsOfType<WeaveableNew>();

            foreach (WeaveableNew weaveable in allWeaveables)
            {
                if (weaveable.isParent)
                {
                    parentWeaveable = weaveable;
                    //dpm.FoundParent();
                }
            }

            if (parentWeaveable != null && !parentWeaveable.wovenObjects.Contains(this.GetComponent<WeaveableNew>()))
            {
                parentWeaveable.wovenObjects.Add(this.GetComponent<WeaveableNew>());
            }
        }

        // both parent and non parent weaveables - DOES NOT AFFECT CRYSTALS
        if (gameObject.tag != "Breakable" && collision.gameObject.GetComponent<Rigidbody>() != null && parentWeaveable.inWeaveMode && canCombine && weaveableScript.ID == ID)
        {
            Debug.Log("1st: " + collision.gameObject);
            Debug.Log("2nd: " + collision.gameObject.GetComponent<Rigidbody>()); // having these debugs here.... fixes issues???????????????

            // only adds fixed joints to parent weaveable to be removed nicely in Uncombine()
            if (collision.gameObject != parentWeaveable.gameObject && collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                var fixedJoint = parentWeaveable.gameObject.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = collision.rigidbody;
                collision.rigidbody.useGravity = true;
            }

            if (weaveInteraction != null)
            {
                weaveInteraction.OnWeave(collision.gameObject);
            }
        }
    }

    public void CallRotate(Vector3 dir, float angle)
    {
        if (!isRotating)
        {
            isRotating = true;
            StartCoroutine(Rotate(dir, angle));
        }
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
        isRotating = false;
    }
    //**********************************************************************************


    //this section is from the IInteractable interface
    //********************************************************************
    public void Interact()
    {
        Debug.Log("This is interactable");
        startFloating = true;
        transform.rotation = Quaternion.identity;
        if (!wovenObjects.Contains(this.GetComponent<WeaveableNew>()))
        {
            wovenObjects.Add(this.GetComponent<WeaveableNew>());
        }
        

        // if objects are combined, vfx needs to show up for both
        if (this.isCombined)
        {
            foreach (WeaveableNew weaveable in parentWeaveable.wovenObjects)
            {
                player.weaveVisualizer.WeaveableSelected(weaveable.gameObject);
            }
        }
        else
        {
            player.weaveVisualizer.WeaveableSelected(gameObject);
        }

        if (!isCombined)
        {
            isParent = true;
            parentWeaveable = this.GetComponent<WeaveableNew>();
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
            isHovering = false;
            inWeaveMode = false;
            isWoven = false;
            startFloating = false;
            canRotate = false;

            rb.constraints = RigidbodyConstraints.None;
            player.weaveVisualizer.StopAura(gameObject);
            TargetingArrow.SetActive(false);

            if (parentWeaveable != null)
            {
                // disables vfx on all woven objects
                foreach (WeaveableNew weaveable in parentWeaveable.wovenObjects)
                {
                    player.weaveVisualizer.StopAura(weaveable.gameObject);
                }
            }

            if (!isCombined)
            {
                isParent = false;
                parentWeaveable = null;
            }
        }
    }

    public void Relocate()
    {
        Debug.Log("this is the relocate method");
        if (canBeRelocated)
        {
            isWoven = true;
            rb.useGravity = false;
            relocate = true;
            rb.isKinematic = false;
            canRotate = true;
            //inWeaveMode = false;
        }

        if (inWeaveMode)
        {
            StartCoroutine(WeaveModeTimer());
        }
    }

    IEnumerator WeaveModeTimer() // sets variable after 1 second to account for combine - need this variable
    {
        yield return new WaitForSeconds(1);
        inWeaveMode = false;
    }

    public void WeaveMode()
    {
        relocate = false;
        inWeaveMode = true;
        rb.isKinematic = true;
        canRotate = true;
    }
    //********************************************************************

    public void OnCombineInput()
    {
        inWeaveMode = true;
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
        weaveableScript = this.GetComponent<WeaveableNew>(); // hardcoded to prevent nulls
        isCombined = false;
        Debug.Log("This is the Uncombine code");

        // removes all instances of joints on the parent weaveable
        FixedJoint[] joints = GetComponents<FixedJoint>();
        foreach (FixedJoint joint in joints)
        {
            Destroy(joint);
        }

        // attempt to stop random movement after uncombine
        foreach (WeaveableNew weaveable in wovenObjects)
        {
            weaveable.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        }

        canRotate = false;
        weaveableScript.rb.useGravity = true;
        weaveableScript.rb.constraints = RigidbodyConstraints.None;
        weaveableScript.rb.freezeRotation = false;
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
            //weaveableScript.startFloating = true; //this is commented out so that the snapping can actually work
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

    void Snapping()
    {
        nearestDistance = Mathf.Infinity; //this is made the be infinite so that when it calculates the distance it wouldn't cap itself
        GameObject closestPoint = null;
        GameObject weaveableClosestPoint = null;

        for (int i = 0; i < myTransformPoints.Length; i++)
        {
            for (int t = 0; t < weaveableScript.myTransformPoints.Length; t++)
            {
                distance = Vector3.Distance(weaveableScript.myTransformPoints[t].transform.position, myTransformPoints[i].transform.position);

                if (distance < nearestDistance)
                {
                    closestPoint = myTransformPoints[i];
                    weaveableClosestPoint = weaveableScript.myTransformPoints[t];
                    nearestDistance = distance;
                    Debug.Log("this is the distance between points " + distance + ",this is the closestpoint" + myTransformPoints[i]);
                }
            }

        }

        weaveableScript.nearestPoint = closestPoint;
        weaveableScript.myNearestPoint = weaveableClosestPoint;
        weaveableScript.nearestDistance = nearestDistance;
        weaveableScript.rb.velocity = weaveableScript.nearestPoint.transform.position - weaveableScript.myNearestPoint.transform.position;

        if (nearestDistance < weaveableScript.snapDistance)
        {
            weaveableScript.rb.transform.position = weaveableScript.myNearestPoint.transform.position;
            // weaveableScript.rb.transform.position = weaveableScript.nearestPoint.transform.position - 
            //                                         (weaveableScript.nearestPoint.transform.position - 
            //                                          weaveableScript.myNearestPoint.transform.position).normalized;
        }
    }


    //********************************************************************

    public void RestoreOriginalLayer()
    {
        gameObject.layer = originalLayerMask;
    }

    public List<WeaveableNew> GetListOfWovenObjects()
    {
        return wovenObjects;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawArrow.ForGizmo(transform.position, Vector3.down * hoveringValue);
    }
}