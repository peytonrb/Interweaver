using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("For Dev Purposes")]
    public int listIndex;
    public int ID;
    private Vector2 lookDirection;
    private Vector3 worldPosition;

    [Header("References")]
    private WeaveController weaveController;
    private Camera mainCamera;
    private GameObject targetingArrow;

    void Start()
    {
        weaveController = GameObject.FindWithTag("Player").GetComponent<WeaveController>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        targetingArrow = this.transform.GetChild(0).Find("Targeting Arrow Parent").gameObject;
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
            FreezeConstraints("position", 'y');
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
        this.GetComponent<Rigidbody>().velocity = rayDirection * 10f;
        FreezeConstraints("rotation");
        FreezeConstraints("position", 'y');
    }

    // combines object with active group of weaveables
    public void CombineObject()
    {
        Debug.Log("combine");
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
