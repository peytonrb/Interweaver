using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveableObject : MonoBehaviour
{
    [Header("State Variables")]
    public bool isBeingWoven; // accessed by WeaveController
    [SerializeField] [Range(1, 10)] private float hoverHeight = 4f;
    public enum ObjectMoveOverrides { Default, ThisAlwaysMoves, ThisNeverMoves }
    public ObjectMoveOverrides objectMoveOverride;
    private bool isHovering = false;
    private bool isWeavingWithMouse;

    [Header("For Dev Purposes")]
    public int listIndex;
    public int ID;
    private Vector2 lookDirection;

    void Update()
    {
        // if objects can be moved and they are being woven, move objects based on joystick/mouse input.
        // variables set by MoveWeaveable(), which is called by WeaveController if object is deemed as a
        //      valid weaveable.
        if (objectMoveOverride == ObjectMoveOverrides.ThisAlwaysMoves || 
            objectMoveOverride == ObjectMoveOverrides.Default)
        {
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
    }

    // triggers Update() if-blocks to run, allows player to move weaveable with either mouse or joystick
    // <param> is the player using a controller or not
    public void MoveWeaveable(bool isGamepad, Vector2 lookDir)
    {
        lookDirection = lookDir;

        if (isGamepad)
        {
            isWeavingWithMouse = false;
        }
        else
        {
            isWeavingWithMouse = true;
        }
    }

    // weaveable moves in the direction of mouse position
    public void MoveWeaveableToMouse()
    {

    }

    // weaveable moves in direction of controller cursor
    // <param> the direction that the player is looking in
    public void MoveWeaveableToTarget(Vector2 lookDir)
    {

    }

    // adds object to array of combined object
    public void AddToWovenObjects()
    {

    }

    // resets all variables of the object to default
    public void ResetWeaveable()
    {
        isBeingWoven = false;
        isHovering = false;
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
