using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIndexScript : MonoBehaviour, ITriggerable
{
    public int cameraIndex;
    public bool triggered;
    public bool isLoop; //Reference only, set in inspector
    public bool isZaxisTrigger; //If false, then its an X axis trigger
    public bool goingOppositeDirection;
    [Header ("Do Not Touch!")]
    public bool enteredFromNorth; //Determines if the player entered from the south side or the north side of the collider
    

    public void OnTrigEnter(Collider collision)
    {
        //CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
    }
    public void OnTrigExit(Collider collision)
    {
        //BEANS
    }
}
