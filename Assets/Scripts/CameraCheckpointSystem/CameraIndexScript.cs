using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIndexScript : MonoBehaviour, ITriggerable
{
    public int cameraIndex;
    public bool triggered;
    public bool isLoop; //Reference only, set in inspector

    public void OnTrigEnter(Collider collision)
    {
        //CameraMasterScript.instance.SwitchFamiliarCameras(cameraIndex);
    }
    public void OnTrigExit(Collider collision)
    {
        //BEANS
    }
}
