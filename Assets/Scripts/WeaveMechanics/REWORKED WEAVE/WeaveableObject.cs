using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveableObject : MonoBehaviour
{
    public int listIndex;
    public int ID;
    public enum ObjectMoveOverrides { Default, ThisAlwaysMoves, ThisNeverMoves }
    public ObjectMoveOverrides objectMoveOverride;

    // weaveable moves in direction of controller cursor
    // <param> the direction that the player is looking in
    public void MoveWeaveableToTarget(Vector2 lookDir)
    {

    }

    // weaveable moves in the direction of mouse position
    public void MoveWeaveableToMouse()
    {

    }

    // adds object to array of combined object
    public void AddToWovenObjects()
    {

    }

    // resets all variables of the object to default
    public void ResetWeaveable()
    {

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
