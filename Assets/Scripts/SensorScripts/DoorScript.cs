using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public SensorController[] sensors;
    public PressurePlateScript[] pressurePlates;

    [Header("Which direction does door open?")]
    public bool opensVertically;
    public bool opensRight;
    public bool opensLeft;
    public bool opensForward;

    [Header("Other door settings")]
    public bool doesDoorClose;
    public float openingSize;
    public float doorSpeed = 2f;

    private bool doNotOpenDoor;
    private Vector3 targetPoint;
    private Vector3 originalPosition;
    private float distance = -1f;
    private bool doorOpening = false;
    private bool doorClosing = false;
    private bool doorIsOpen = false;
    private bool pplateTriggered = false;
    private bool stopCoroutine = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // if the player unpowers any of the sensors while the door is open/opening
        if (sensors != null && sensors.Length > 0 && doesDoorClose)
        {
            foreach (SensorController sensor in sensors)
            {
                if (!sensor.isActive && doorOpening || !sensor.isActive && doorIsOpen)
                {
                    if (doorOpening && !doorIsOpen)
                    {
                        doorOpening = false;
                        stopCoroutine = true;
                        StopCoroutine(OpenDoor());
                    }

                    CloseDoor();
                    doorIsOpen = false;
                }
            }
        }
        // if the player unpowers any of the pressure plates while the door is open/opening
        else if (pressurePlates != null && pressurePlates.Length > 0 && doesDoorClose)
        {
            foreach (PressurePlateScript pplate in pressurePlates)
            {
                if (!pplate.standingOnPlate && doorOpening || !pplate.standingOnPlate && doorIsOpen)
                {
                    if (doorOpening && !doorIsOpen)
                    {
                        doorOpening = false;
                        stopCoroutine = true;
                        pplateTriggered = false;
                        StopCoroutine(OpenDoor());
                    }

                    CloseDoor();
                    doorIsOpen = false;
                }
            }
        }

        bool allActive = true;

        // if the sensors are all active but the door is closing...
        if (sensors != null && sensors.Length > 0)
        {
            foreach (SensorController sensor in sensors)
            {
                if (!sensor.isActive)
                {
                    allActive = false;
                }
            }

            if (allActive && !stopCoroutine && !doorIsOpen && !doorOpening)
            {
                doorClosing = false;
                StartCoroutine(OpenDoor());
            }
        }
        // if the pressure plates are all active but the door is closing...
        else if (pressurePlates != null && pressurePlates.Length > 0)
        {
            foreach (PressurePlateScript pplate in pressurePlates)
            {
                if (!pplate.standingOnPlate)
                {
                    allActive = false;
                }
            }

            if (allActive && !stopCoroutine && !doorIsOpen && !doorOpening)
            {
                doorClosing = false;
                StartCoroutine(OpenDoor());
            }
        }

        // catch-all: if the door is opening and the door is close to mark (in case of weird bugs)
        if (doorOpening && distance < 0.3f)
        {
            doorOpening = false;
            doorIsOpen = true;
        }

        // catch-all: if the door is neither opening nor is open already, it cannot be closed
        // if (!doorOpening)
        // {
        //     stopCoroutine = false;
        // }
    }

    public void MoveDoor()
    {
        doNotOpenDoor = false;

        // if multiple sensors are required to open the door
        if (sensors != null && sensors.Length > 1)
        {
            foreach (SensorController sensor in sensors)
            {
                if (!sensor.isActive)
                {
                    doNotOpenDoor = true;
                }
            }
        }
        // if multiple pplates are required to open the door
        else if (pressurePlates != null && pressurePlates.Length > 1)
        {
            foreach (PressurePlateScript pplate in pressurePlates)
            {
                if (!pplate.activated)
                {
                    doNotOpenDoor = true;
                }
            }
        }

        doorOpening = true;
        doorIsOpen = false;

        // all relevant sensors/pplates need to be active at this point
        if (!doorIsOpen && !doNotOpenDoor && doorOpening && !pplateTriggered)
        {
            if (opensRight)
            {
                targetPoint = originalPosition + (Vector3.right * openingSize);
            }
            else if (opensLeft)
            {
                targetPoint = originalPosition + (Vector3.left * openingSize);
            }
            else if (opensForward)
            {
                targetPoint = originalPosition + (Vector3.forward * openingSize);
            }
            else if (opensVertically)
            {
                targetPoint = originalPosition + (Vector3.up * openingSize);
            }
            else if (!opensRight && !opensLeft && !opensForward && !opensVertically)
            {
                targetPoint = originalPosition + (Vector3.back * openingSize);
            }

            distance = Vector3.Distance(originalPosition, targetPoint);

            // pressure plates continuously call their events. this event should only be called once. 
            if (pressurePlates != null && !pplateTriggered && !doorIsOpen)
            {
                StartCoroutine(OpenDoor());
                pplateTriggered = true;
            }
            else if (pressurePlates == null && !doorIsOpen)
            {
                StartCoroutine(OpenDoor());
            }
        }
    }

    private void CloseDoor()
    {
        if (doorIsOpen && doesDoorClose)
        {
            if (opensRight)
            {
                targetPoint = transform.position + (Vector3.left * openingSize);
            }
            else if (opensLeft)
            {
                targetPoint = transform.position + (Vector3.right * openingSize);
            }
            else if (opensForward)
            {
                targetPoint = transform.position + (Vector3.back * openingSize);
            }
            else if (opensVertically)
            {
                targetPoint = transform.position + (Vector3.down * openingSize);
            }
            else if (!opensRight && !opensLeft && !opensForward && !opensVertically)
            {
                targetPoint = transform.position + (Vector3.forward * openingSize);
            }

            distance = Vector3.Distance(transform.position, targetPoint);
        }

        StartCoroutine(MoveBack());
    }

    IEnumerator OpenDoor()
    {
        if (distance >= 0.3f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, doorSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, targetPoint);

            // stop coroutine only works at a yield break, which would not happen until too late otherwise
            if (stopCoroutine)
            {
                stopCoroutine = false;
                yield break;
            }

            StartCoroutine(OpenDoor());
            yield return null;
        }
        else
        {
            //when its fully open
            doorOpening = false;
            doorIsOpen = true;
        }

        yield break;
    }

    IEnumerator MoveBack()
    {
        // door can be triggered again by pressure plate event
        if (pressurePlates != null && pplateTriggered)
        {
            pplateTriggered = false;
        }

        if (distance >= 0.3f)
        {
            yield return new WaitForEndOfFrame();
            doorClosing = true;
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, doorSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, originalPosition);            
            StartCoroutine(MoveBack());
            yield return null;
        }
        else
        {
            //when its fully closed
            doorClosing = false;
            doorIsOpen = false;
        }

        yield break;
    }

    // debug arrow in scene
    void OnDrawGizmos()
    {
        if (!doorOpening)
        {
            if (opensVertically)
            {
                DrawArrow.ForGizmo(transform.position, Vector3.up * openingSize);
            }
            else
            {
                if (opensRight)
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.right * openingSize);
                }
                else if (opensLeft)
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.left * openingSize);
                }
                else if (opensForward)
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.forward * openingSize);
                }
                else
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.back * openingSize);
                }
            }
        }
        else
        {
            if (opensVertically)
            {
                DrawArrow.ForGizmo(transform.position, Vector3.up * distance);
            }
            else
            {
                if (opensRight)
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.right * distance);
                }
                else if (opensLeft)
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.left * distance);
                }
                else if (opensForward)
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.forward * distance);
                }
                else
                {
                    DrawArrow.ForGizmo(transform.position, Vector3.back * distance);
                }
            }
        }
    }
}
