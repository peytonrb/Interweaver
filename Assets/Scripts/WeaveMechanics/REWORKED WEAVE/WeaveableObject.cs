using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaveableObject : MonoBehaviour
{
    [Header("State Variables")]
    public bool isBeingWoven; // accessed by WeaveController
    [SerializeField][Range(1, 5)] private float hoverHeight = 2f;
    public enum ObjectMoveOverrides { Default, ThisAlwaysMoves, ThisNeverMoves }
    public ObjectMoveOverrides objectMoveOverride;
    [SerializeField] private LayerMask weaveableLayers;
    [SerializeField] private float maxWeaveDistance = 25f;
    private bool isHovering = false;

    // rotation
    [HideInInspector] public enum rotateDir { forward, back, left, right }
    private float rotAmount = 45f;
    private GameObject weaveableObj;

    // snapping
    private Transform[] myTransformPoints;
    public GameObject activeSnapPoint; // hide in inspector
    private GameObject otherActiveSnapPoint;
    private WeaveableObject objectToSnapTo;
    private Transform targetTransform;
    private Vector3 targetPos;
    private float nearestDistance = 50f;

    [Header("For Dev Purposes - DO NOT EDIT")]
    public int listIndex;
    public int ID;
    private Vector2 lookDirection;
    private Vector3 worldPosition;

    [Header("References")]
    private WeaveController weaveController;
    private Camera mainCamera;
    [HideInInspector] public GameObject targetingArrow;

    void Start()
    {
        weaveController = GameObject.FindWithTag("Player").GetComponent<WeaveController>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        targetingArrow = this.transform.GetChild(0).Find("Targeting Arrow Parent").gameObject;
        weaveableObj = this.transform.GetChild(0).gameObject;

        // set snap points
        myTransformPoints = new Transform[6];
        myTransformPoints = this.GetComponentsInChildren<Transform>();
        myTransformPoints = myTransformPoints.Where(child => child.tag == "SnapPoint").ToArray();
    }

    void Update()
    {
        // if objects can be moved and they are being woven, move objects based on joystick/mouse input.
        // variables set by MoveWeaveable(), which is called by WeaveController if object is deemed as a
        //      valid weaveable.
        if (isBeingWoven)
        {
            // lifts weaveable immediately once woven. only runs once, altitude of weaveable determined by
            //      MoveWeaveable()
            if (!isHovering)
            {
                isHovering = true;
                transform.position = transform.position + new Vector3(0, hoverHeight, 0);
            }

            // actually moves the weaveable with joystick or mouse
            if (!InputManagerScript.instance.isGamepad)
                MoveWeaveableToMouse();
            else
                MoveWeaveableToTarget(lookDirection);
        }
    }

    // weaveable moves in the direction of mouse position
    // triggers Update() if-blocks to run, allows player to move weaveable with mouse
    // drops object if player is too far or moving object too fast
    public void MoveWeaveableToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // if the mouse position is over weaveable terrain or any weaveable object -- WALLS NEED TO BE TAGGED AS RAYCASTONLYFLOOR
        if (Physics.Raycast(ray, out hitInfo, 1000f, weaveableLayers))
        {
            // enables and sets object's targeting arrow
            targetingArrow.SetActive(true);
            weaveController.targetingArrow.SetActive(false);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 100f))
                worldPosition = hitData.point;

            // if object is past the max distance allowed from player, object is disconnected
            if (Vector3.Distance(transform.position, weaveController.transform.position) > maxWeaveDistance)
            {
                weaveController.OnDrop();
                return;
            }

            Vector3 adjustedVector = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
            targetingArrow.transform.LookAt(adjustedVector);

            // move object to mouse position
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(hitData.point.x - rb.position.x,
                                      transform.position.y - rb.position.y,
                                      hitData.point.z - rb.position.z);

            FreezeConstraints("rotation");
        }
    }

    // weaveable moves in direction of controller cursor
    // triggers Update() if-blocks to run, allows player to move weaveable with joystick
    // <param> the direction that the player is looking in
    public void MoveWeaveableToTarget(Vector2 lookDir)
    {
        // if this object does not have the most updated look direction saved...
        if (lookDir != lookDirection)
        {
            lookDirection = lookDir;
        }

        // enables targeting arrow and determines direction to fire boxcast
        targetingArrow.SetActive(true);
        weaveController.targetingArrow.SetActive(false);
        float targetAngle = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

        if (lookDir == Vector2.zero) // if player releases RS, targeting arrow is hidden
        {
            targetingArrow.SetActive(false);
        }
        else // otherwise, it moves with RS's position
        {
            targetingArrow.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        Vector3 rayDirection = targetingArrow.transform.forward;

        // need BOXCAST FOR COMBINING

        // if object is past the max distance allowed from player, object is disconnected
        if (Vector3.Distance(transform.position, weaveController.transform.position) > maxWeaveDistance)
        {
            weaveController.OnDrop();
            return;
        }

        // move object with joystick movement
        this.GetComponent<Rigidbody>().velocity = rayDirection * 6f;
        FreezeConstraints("rotation");
    }

    // rotates the object after being called by InputManager
    // <param> enum that determines direction to rotate
    public void RotateObject(rotateDir r)
    {
        // Assigns the rotation amount to the original xyz values every time the input is called.
        float xAmount = rotAmount;
        float yAmount = rotAmount;
        float zAmount = rotAmount;

        // Gets the forward vector of the camera and the forward and right vectors of the parent 
        //      object to check the angle between them.
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 objectForward = transform.forward;
        Vector3 objectRight = transform.right;

        float forwardAngle = Vector3.Angle(Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized,
                                           Vector3.ProjectOnPlane(objectForward, Vector3.up).normalized);
        float rightAngle = Vector3.Angle(Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized,
                                         Vector3.ProjectOnPlane(objectRight, Vector3.up).normalized);

        //Assigns x and z amounts based on the angle of the camera forward along the xz plane of the parent object.
        if (forwardAngle >= 0f && forwardAngle <= 45f && rightAngle >= 45f && rightAngle <= 135f)
        {
            xAmount = rotAmount;
            zAmount = 0f;
        }
        else if (forwardAngle >= 45f && forwardAngle <= 135f && rightAngle >= 0f && rightAngle <= 45f)
        {
            xAmount = 0f;
            zAmount = -rotAmount;
        }
        else if (forwardAngle >= 135f && forwardAngle <= 180f && rightAngle >= 45f && rightAngle <= 135f)
        {
            xAmount = -rotAmount;
            zAmount = 0f;
        }
        else if (forwardAngle >= 45f && forwardAngle <= 135f && rightAngle >= 135f && rightAngle <= 180f)
        {
            xAmount = 0f;
            zAmount = rotAmount;
        }

        switch (r)
        {
            case rotateDir.forward:
                weaveableObj.transform.RotateAround(transform.position, transform.right, xAmount);
                weaveableObj.transform.RotateAround(transform.position, transform.forward, zAmount);
                break;
            case rotateDir.back:
                weaveableObj.transform.RotateAround(transform.position, transform.right, -xAmount);
                weaveableObj.transform.RotateAround(transform.position, transform.forward, -zAmount);
                break;
            case rotateDir.right:
                transform.Rotate(0f, yAmount, 0f, Space.World);
                break;
            case rotateDir.left:
                transform.Rotate(0f, -yAmount, 0f, Space.World);
                break;
        }
    }

    // combines object with active group of weaveables
    public void CombineObject()
    {
        // set variables
        FindSnapPoints();

        //check object move overrides to determine how the combined objects will move
        switch (objectMoveOverride)
        {
            case ObjectMoveOverrides.ThisAlwaysMoves: // move this weavable 
                StartCoroutine(MoveToPoint(this, objectToSnapTo));
                StartCoroutine(BackUpForceSnap(this));
                break;

            case ObjectMoveOverrides.ThisNeverMoves:
                StartCoroutine(MoveToPoint(objectToSnapTo, this));
                StartCoroutine(BackUpForceSnap(objectToSnapTo));
                break;

            default: // move other weaveable
                StartCoroutine(MoveToPoint(objectToSnapTo, this));
                StartCoroutine(BackUpForceSnap(objectToSnapTo));
                break;
        }
    }

    private void FindSnapPoints()
    {
        float distance;
        float nearestDistance = Mathf.Infinity;
        objectToSnapTo = weaveController.selectedWeaveable; // needs to be set to null asap

        // calculate nearest snap points
        for (int i = 0; i < myTransformPoints.Length; i++)
        {
            for (int t = 0; t < objectToSnapTo.myTransformPoints.Length; t++)
            {
                distance = Vector3.Distance(objectToSnapTo.myTransformPoints[t].position, myTransformPoints[i].position);

                if (distance < nearestDistance)
                {
                    activeSnapPoint = myTransformPoints[i].gameObject;
                    otherActiveSnapPoint = objectToSnapTo.myTransformPoints[t].gameObject;
                    nearestDistance = distance;
                }
            }
        }

        // sets other weaveable's variables for movement
        objectToSnapTo.otherActiveSnapPoint = activeSnapPoint;
        objectToSnapTo.activeSnapPoint = otherActiveSnapPoint;
        objectToSnapTo.nearestDistance = nearestDistance;

        // determines target position for combining movement
        targetTransform = otherActiveSnapPoint.transform;
    }

    // moves weaveable to desired location
    // <param> the moving weaveable and the static weaveable
    IEnumerator MoveToPoint(WeaveableObject movingObject, WeaveableObject staticObject)
    {
        movingObject.GetComponent<Rigidbody>().useGravity = false;
        Vector3 targetRotation = staticObject.transform.rotation.eulerAngles;

        float x = ((targetRotation.x) / 90) * 90;
        float y = ((targetRotation.y) / 90) * 90;
        float z = ((targetRotation.z) / 90) * 90;

        // set constraints
        if (movingObject == this)
        {
            UnfreezeConstraints("all");
        }
        else
        {
            FreezeConstraints("position");
        }

        targetPos = staticObject.transform.localPosition + targetTransform.localPosition;
        Quaternion nearestangle = Quaternion.Euler(x, y, z);
        movingObject.transform.rotation = nearestangle;

        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            movingObject.transform.localPosition = Vector3.Lerp(movingObject.transform.localPosition,
                                                          targetPos, timeSinceStarted);

            if (Vector3.Distance(movingObject.transform.position, targetTransform.position) < 1f)
            {
                movingObject.transform.position = targetTransform.position;

                if (!TryGetComponent<FixedJoint>(out FixedJoint fJ))
                {
                    Debug.Log("weave together");
                    movingObject.GetComponent<Rigidbody>().useGravity = true;
                    //WeaveTogether(staticObject.gameObject);

                    // reset constraints
                    if (movingObject == this)
                    {
                        FreezeConstraints("rotation");
                    }
                    else
                    {
                        UnfreezeConstraints("position");
                    }
                }

                yield break;
            }

            yield return null;
        }
    }

    // forces object together if positions are off after 2 seconds
    // <param> the moving weaveable
    IEnumerator BackUpForceSnap(WeaveableObject movingObject)
    {
        yield return new WaitForSeconds(2f);
        movingObject.transform.position = targetTransform.position;

        if (!TryGetComponent<FixedJoint>(out FixedJoint fJ))
        {
            Debug.Log("backup called");

            // reset constraints
            movingObject.GetComponent<Rigidbody>().useGravity = true;
            if (movingObject == this)
            {
                FreezeConstraints("rotation");
            }
            else
            {
                UnfreezeConstraints("position");
            }
        }
    }

    // adds object to array of combined object
    public void AddToWovenObjects()
    {
        Vector2 returnValue = WeaveableManager.Instance.AddWeaveableToList(this.gameObject, false);

        // AddWeaveableToList() returns a Vector2 to get both ints from the return value
        ID = (int)returnValue.x;
        listIndex = (int)returnValue.y;

        // vfx/audio
        weaveController.weaveFXScript.WeaveableSelected(this.gameObject); // not sure how this is going to go with the combined weaveables
    }

    // resets all variables of the object to default
    public void ResetWeaveable()
    {
        WeaveableManager.Instance.RemoveWeaveableFromList(listIndex, ID);
        isBeingWoven = false;
        isHovering = false;
        listIndex = 0;
        ID = 0;
        targetingArrow.SetActive(false);
        weaveController.targetingArrow.SetActive(true);

        // reset rotation
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        weaveableObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        UnfreezeConstraints("all");
    }

    /*******************************************************
    *                                                      *
    *                   HELPER FUNCTIONS                   *
    *                                                      *
    *******************************************************/

    // call this method with either "position" or "rotation", or null in the parameter
    // freezes Rigidbody constraints
    private void FreezeConstraints(string command)
    {
        switch (command)
        {
            case "position":
                this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePosition;
                break;
            case "rotation":
                this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotation;
                break;
            default:
                this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeAll;
                break;
        }
    }

    // overload of previous method, accepts a specific orientation to freeze
    private void FreezeConstraints(string command, char orientation)
    {
        switch (command)
        {
            case "position":
                if (orientation == 'x')
                    this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionX;
                else if (orientation == 'y')
                    this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;
                else if (orientation == 'z')
                    this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionZ;
                break;
            case "rotation":
                if (orientation == 'x')
                    this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationX;
                else if (orientation == 'y')
                    this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationY;
                else if (orientation == 'z')
                    this.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationZ;
                break;
        }
    }

    // call this method with either "position", "rotation", or null in the parameter
    // unfreezes Rigidbody constraints
    private void UnfreezeConstraints(string command)
    {
        switch (command)
        {
            case "position": // refreezes rotation if player only wants to unfreeze position
                this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePosition;
                break;
            case "rotation": // refreezes position if player only wants to unfreeze rotation
                this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotation;
                break;
            default:
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                break;
        }
    }

    // overload of previous method, accepts a specific orientation to unfreeze
    private void UnfreezeConstraints(string command, char orientation)
    {
        switch (command)
        {
            case "position":
                if (orientation == 'x')
                    this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
                else if (orientation == 'y')
                    this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
                else if (orientation == 'z')
                    this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
                break;
            case "rotation":
                if (orientation == 'x')
                    this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationX;
                else if (orientation == 'y')
                    this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationY;
                else if (orientation == 'z')
                    this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                break;
        }
    }

    // determines how far the ground is while hovering
    // <returns> the minimum y position of the weaveable
    private float DetermineGroundDistance()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (this.transform.GetChild(0).GetComponent<Renderer>().bounds.min.y - hit.transform.position.y < hoverHeight + 0.5f)
            {
                return this.transform.GetChild(0).GetComponent<Renderer>().bounds.min.y + hoverHeight + hit.transform.position.y;
            }
        }

        return 100;
    }
}
