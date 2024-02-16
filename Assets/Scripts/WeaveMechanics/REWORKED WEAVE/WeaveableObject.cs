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
    [SerializeField] private float maxWeaveDistance = 50f;
    private bool isHovering = false;
    private bool isWeavingWithMouse;

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
            if (isWeavingWithMouse)
                MoveWeaveableToMouse();
            else if (!isWeavingWithMouse)
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

            if (Physics.Raycast(ray, out hitData, maxWeaveDistance))
                worldPosition = hitData.point;
            else // if object is past raycast distance (out of bounds)
            {
                if (Vector3.Distance(transform.position, weaveController.transform.position) > maxWeaveDistance)
                {
                    weaveController.OnDrop();
                    Debug.Log("here");
                }
    
                return;
            }

            Vector3 adjustedVector = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
            targetingArrow.transform.LookAt(adjustedVector);

            // move object to mouse position
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(hitData.point.x - rb.position.x,
                                      transform.position.y - rb.position.y,
                                      hitData.point.z - rb.position.z);

            FreezeContraints("rotation");
        }
    }

    // weaveable moves in direction of controller cursor
    // triggers Update() if-blocks to run, allows player to move weaveable with joystick
    // <param> the direction that the player is looking in
    public void MoveWeaveableToTarget(Vector2 lookDir)
    {

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
    }

    // call this method with either "position" or "rotation", or null in the parameter
    // freezes Rigidbody constraints
    private void FreezeContraints(string command)
    {
        switch (command)
        {
            case "position":
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                break;
            case "rotation":
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                break;
            default:
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                break;
            case "rotation": // refreezes position if player only wants to unfreeze rotation
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                break;
            default:
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                break;
        }
    }
}
