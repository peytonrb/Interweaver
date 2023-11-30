using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaveableNew : MonoBehaviour, IInteractable, ICombineable
{
    [Header("Weavables")]
    [CannotBeNullObjectField][SerializeField] private Rigidbody rb;
    [SerializeField] private float hoveringValue = 4f;
    private Camera mainCamera;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private LayerMask layersToHitController;
    [SerializeField] public WeaveInteraction weaveInteraction;
    public bool alwaysMovesWhenWoven = false;
    public bool neverMovesWhenWoven = false;
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
    private Collider hitCol = null;


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
    public Transform targetTransform;
    private bool resetQuaternion;

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
    public PlayerController player;
    private GameObject wovenFloatingIsland;
    [SerializeField] private GameObject dayblockPuzzle;
    private DayblockPuzzleManager dpm;

    [Header("Audio")]
    [SerializeField] private AudioClip rotateClip;

    private Vector2 lastLookDir;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Debug.Log(GameObject.FindGameObjectWithTag("Player").name);
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

        mainCamera = Camera.main;
        Uncombine();
    }

    void Update()
    {
        if (startFloating)
        {
            isHovering = true;
            if (canBeRelocated)
            {
                transform.position = transform.position + new Vector3(0, hoveringValue, 0);
            }

            startFloating = false;
        }

        if (isWoven)
        {
            if (!InputManagerScript.instance.isGamepad)
                MovingWeaveMouse();
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
                            //Uncombine();
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

                TargetingArrow.SetActive(true);

                if (combineable != null && !weaveableScript.canCombine)
                {
                    canCombine = true;
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
        ExtDebug.DrawBox(TargetingArrow.transform.GetChild(0).transform.position, new Vector3(1,3,12), TargetingArrow.transform.rotation, Color.green);

        if (lookDir.magnitude >= 0.1f)
        {
            TargetingArrow.SetActive(true);

            // the actual raycast from the player, this can be used for the line render 
            //      but it takes the raycast origin and the direction of the raycast
            Ray arrowRay = new Ray(transform.position, rayDirection);
           
            if (relocate)
            {
                if (canBeRelocated)
                {
                    canRotate = true;
                    //Debug.Log(lookDir);

                    //change this to send velocity in direction of (RS)
                    rb.velocity = rayDirection * 6;

                    //this freezes the Y position so that the combined objects won't drag it down because of gravity and it freezes in all rotation so it won't droop because of the gravity  from the objects
                    rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }
            }
            if (inWeaveMode)
            {
                RaycastHit[] hits = Physics.BoxCastAll(TargetingArrow.transform.GetChild(0).transform.position, new Vector3(1, 3, 12), rayDirection, Quaternion.LookRotation(rayDirection), 12, layersToHitController);                
                float lastDistance = 100;

                foreach (RaycastHit hit in hits)
                {                   
                    if (Vector3.Distance(hit.transform.position, transform.position) < lastDistance)
                    {
                        Debug.Log("this is getting called inWeaveMode raycast stuff");
                        hitCol = hit.collider;
                        lastDistance = Vector3.Distance(hit.transform.position, transform.position);
                    }
                }
                SetControllerCombineable(hitCol);
            }
        }
        else
        {
            TargetingArrow.SetActive(false);
            rb.velocity = Vector3.zero;
        }
    }


    public void SetControllerCombineable(Collider col)
    {
        if (col != null)
        {
            Debug.Log(col.name);
            ICombineable combineable = col.GetComponent<ICombineable>();
            canRotate = true;
            weaveableScript = col.GetComponent<WeaveableNew>();

            TargetingArrow.SetActive(true);

            if (combineable != null && !weaveableScript.canCombine)
            {
                canCombine = true;
            }
        }
    }

    public void CallRotate(Vector3 dir, float angle)
    {
        if (!isRotating)
        {
            isRotating = true;
            StartCoroutine(Rotate(dir, angle));
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, rotateClip);
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
                weaveable.rb.useGravity = false;
            }
        }
        else
        {
            Debug.Log(player);
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
            Debug.Log("ahduiasbsayuffbslafyb");
            player.floatingIslandCrystal = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            player.isCurrentlyWeaving = false;
            //player.uninteract = true;
            player.inRelocateMode = false;
            player.inCombineMode = false;
            relocate = false;
            isHovering = false;
            inWeaveMode = false;
            isWoven = false;
            startFloating = false;
            canRotate = false;
            
            if (rb.gameObject.tag != "FloatingIsland")
            {
                rb.constraints = RigidbodyConstraints.None;
            }
            
            player.weaveVisualizer.StopAura(gameObject);
            TargetingArrow.SetActive(false);
            player.weaveVisualizer.DisableWeave();

            if (parentWeaveable != null)
            {
                // disables vfx on all woven objects
                foreach (WeaveableNew weaveable in parentWeaveable.wovenObjects)
                {
                    player.weaveVisualizer.StopAura(weaveable.gameObject);
                    weaveable.rb.useGravity = true;
                    rb.isKinematic = false;
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
        //Debug.Log("this is the relocate method");
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
        yield return new WaitForSeconds(1f);
        Debug.Log("timer sets false");
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
        Debug.Log(weaveableScript.gameObject.name);
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
        //Debug.Log("This is the Uncombine code");

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

        if (rb.gameObject.tag != "FloatingIsland")
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        weaveableScript.rb.freezeRotation = false;
        player.inRelocateMode = false;
        player.inCombineMode = false;

        player.Uninteract();
        player.weaveVisualizer.DisableWeave();
        wovenObjects.Clear();
    }

    public void Combine()
    {
        canCombine = true;

        Snapping();

        wovenObjects.Add(weaveableScript);

        weaveableScript.rb.useGravity = false;
        player.inRelocateMode = true;
        player.inCombineMode = false;
    }
    //********************************************************************

    void Snapping()
    {
        nearestDistance = Mathf.Infinity; //this is made the be infinite so that when it calculates the distance it wouldn't cap itself
        GameObject myClosestPoint = null;
        GameObject weaveableClosestPoint = null;

        for (int i = 0; i < myTransformPoints.Length; i++)
        {
            for (int t = 0; t < weaveableScript.myTransformPoints.Length; t++)
            {
                distance = Vector3.Distance(weaveableScript.myTransformPoints[t].transform.position, myTransformPoints[i].transform.position);

                if (distance < nearestDistance)
                {
                    myClosestPoint = myTransformPoints[i];
                    weaveableClosestPoint = weaveableScript.myTransformPoints[t];
                    nearestDistance = distance;
                }
            }
        }

        weaveableScript.nearestPoint = myClosestPoint;
        weaveableScript.myNearestPoint = weaveableClosestPoint;
        weaveableScript.nearestDistance = nearestDistance;
        targetTransform = weaveableScript.nearestPoint.transform;

        //Check if always moves
        if (alwaysMovesWhenWoven)
        {
            //if both objects have it, dont do anything
            if (weaveableScript.alwaysMovesWhenWoven)
            {
                return;
            }
            else // move the first weavable 
            {
                targetTransform = weaveableScript.myNearestPoint.transform;

                StartCoroutine(MoveToPoint(this, weaveableScript.transform, weaveableScript));
                StartCoroutine(BackUpForceSnap(this));
            }
        } else if (neverMovesWhenWoven) //check if never moves
        {
            //if both objects have it, dont do anything
            if (weaveableScript.neverMovesWhenWoven)
            {
                return;
            } 
            else // move the second weavable
            {
                StartCoroutine(MoveToPoint(weaveableScript, transform, weaveableScript));
                StartCoroutine(BackUpForceSnap(weaveableScript));
            }


        } else if (weaveableScript.neverMovesWhenWoven) //if other object never moves
        {
            targetTransform = weaveableScript.myNearestPoint.transform;

            StartCoroutine(MoveToPoint(this, weaveableScript.transform, weaveableScript));
            StartCoroutine(BackUpForceSnap(this));
        }
        else// default case
        {
            StartCoroutine(MoveToPoint(weaveableScript, transform, weaveableScript));
            StartCoroutine(BackUpForceSnap(weaveableScript));
        }
        
    }

    //Actually combines the objects and calls any additional logic
    void WeaveTogether(GameObject other)
    {
        weaveableScript = GetComponent<WeaveableNew>();
        Debug.Log("parent: " + parentWeaveable);
        
        // for objects that are being connected that are NOT the parent
        if (other.GetComponent<Rigidbody>() != null && canCombine && !isParent && weaveableScript.ID == ID)
        {
            Debug.Log("started parent weavable thing");
            WeaveableNew[] allWeaveables = FindObjectsOfType<WeaveableNew>();

            foreach (WeaveableNew weaveable in allWeaveables)
            {
                if (weaveable.isParent)
                {
                    parentWeaveable = weaveable;
                    if (dayblockPuzzle != null)
                    {
                        dpm.FoundParent();
                    }
                }
            }

            if (parentWeaveable != null && !parentWeaveable.wovenObjects.Contains(this.GetComponent<WeaveableNew>()))
            {
                parentWeaveable.wovenObjects.Add(this.GetComponent<WeaveableNew>());
            }
        }

        if (other.GetComponent<Rigidbody>() != null && parentWeaveable.inWeaveMode && canCombine && weaveableScript.ID == ID)
        {
            // only adds fixed joints to parent weaveable to be removed nicely in Uncombine()
            if (other != parentWeaveable.gameObject && other.GetComponent<Rigidbody>() != null)
            {
                var fixedJoint = parentWeaveable.gameObject.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = other.GetComponent<Rigidbody>();
                other.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                other.GetComponent<Rigidbody>().useGravity = true;
            }

            if (weaveInteraction != null)
            {
                weaveInteraction.OnWeave(other, gameObject);
            }

            if (other.GetComponent<WeaveableNew>().weaveInteraction != null)
            {
                other.GetComponent<WeaveableNew>().weaveInteraction.OnWeave(gameObject, other);
            }
        }
    }
    // these references are passed in so when weaveableScript changes with mouse position, it still holds correct 
    //      reference

    //Moves the weavable to the target transform then calls WeaveTogether()
    IEnumerator MoveToPoint(WeaveableNew movingWeaveableRef, Transform firstObjTransform, WeaveableNew otherWeaveable)
    {
        Vector3 firstobjrotation = firstObjTransform.rotation.eulerAngles;

        float x = ((firstobjrotation.x) / 90) * 90;
        float y = ((firstobjrotation.y) / 90) * 90;
        float z = ((firstobjrotation.z) / 90) * 90;

        Quaternion nearestangle = Quaternion.Euler(x, y, z);
        movingWeaveableRef.rb.transform.rotation = nearestangle;

        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            movingWeaveableRef.transform.position = Vector3.Lerp(movingWeaveableRef.transform.position, targetTransform.position, timeSinceStarted);

            if (Vector3.Distance(movingWeaveableRef.transform.position, targetTransform.position) < 1f)
            {
                movingWeaveableRef.rb.transform.position = targetTransform.position;

                if (!TryGetComponent<FixedJoint>(out FixedJoint fJ))
                {
                    WeaveTogether(otherWeaveable.gameObject);
                }
                    
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator BackUpForceSnap( WeaveableNew weaveableRef)
    {
        yield return new WaitForSeconds(2f);
        weaveableRef.rb.transform.position = targetTransform.position;
        if (!TryGetComponent<FixedJoint>(out FixedJoint fJ))
        {
            Debug.Log("Backup called");
            //WeaveTogether(weaveableRef.gameObject);
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