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
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // if multiple sensors/pressure plates need to be active
        if (sensors != null && sensors.Length > 0 && doesDoorClose)
        {
            foreach (SensorController sensor in sensors)
            {
                // if the player unpowers sensor in middle of opening
                if (!sensor.isActive && doorOpening || !sensor.isActive && doorIsOpen)
                {
                    if (doorOpening)
                    {
                        StopCoroutine(OpenDoor());
                        stopCoroutine = true;
                    }

                    CloseDoor();
                    doorIsOpen = false;
                    doorOpening = false;
                }
            }
        }
        else if (pressurePlates != null && pressurePlates.Length > 0 && doesDoorClose)
        {
            foreach (PressurePlateScript pplate in pressurePlates)
            {
                // if player gets off pressure plate in middle of opening
                if (!pplate.standingOnPlate && doorOpening || !pplate.standingOnPlate && doorIsOpen)
                {
                    if (doorOpening)
                    {
                        doorOpening = false;
                        StopCoroutine(OpenDoor());
                        stopCoroutine = true;
                    }

                    CloseDoor();
                    doorIsOpen = false;
                }
            }
        }

        // if sensors are turned off by above function but all are active, turn back on
        if (sensors != null && sensors.Length > 0)
        {
            bool allActive = true;

            foreach (SensorController sensor in sensors)
            {
                if (!sensor.isActive)
                {
                    allActive = false;
                }
            }

            if (allActive && !stopCoroutine)
            {
                StartCoroutine(OpenDoor());
            }
        }
        else if (pressurePlates != null && pressurePlates.Length > 0)
        {
            bool allActive = true;

            foreach (PressurePlateScript pplate in pressurePlates)
            {
                if (!pplate.standingOnPlate)
                {
                    allActive = false;
                }
            }

            if (allActive && !stopCoroutine)
            {
                StartCoroutine(OpenDoor());
            }
        }

        // catch-all
        if (doorIsOpen && doorOpening)
        {
            doorOpening = false;
        }
    }

    public void MoveDoor()
    {
        if (opensVertically)
        {
            MoveUpwards();
        }
        else if (opensForward || opensRight || opensLeft)
        {
            MoveSideways();
        }
    }

    private void MoveUpwards()
    {
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

        doorIsOpen = false;

        // pressure plates continuously call their events. this event should only be called once. 
        if (pressurePlates != null && !pplateTriggered && !doorIsOpen && !doNotOpenDoor)
        {
            targetPoint = transform.position + (Vector3.up * openingSize);
            distance = Vector3.Distance(transform.position, targetPoint);
            StartCoroutine(OpenDoor());
            pplateTriggered = true;
        }
        else if (pressurePlates == null && !doorIsOpen)
        {
            targetPoint = transform.position + (Vector3.up * openingSize);
            distance = Vector3.Distance(transform.position, targetPoint);
            StartCoroutine(OpenDoor());
        }
    }

    private void MoveSideways()
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

        // all relevant sensors/pplates need to be active at this point
        if (!doorIsOpen && !doNotOpenDoor && doorOpening && !pplateTriggered)
        {
            if (opensRight)
            {
                targetPoint = transform.position + (Vector3.right * openingSize);
            }
            else if (opensLeft)
            {
                targetPoint = transform.position + (Vector3.left * openingSize);
            }
            else if (opensForward)
            {
                targetPoint = transform.position + (Vector3.forward * openingSize);
            }
            else if (!opensRight && !opensLeft && !opensForward)
            {
                targetPoint = transform.position + (Vector3.back * openingSize);
            }

            distance = Vector3.Distance(transform.position, targetPoint);

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
            if (opensVertically)
            {
                targetPoint = transform.position + (Vector3.down * openingSize);
            }
            else
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
                else if (!opensRight && !opensLeft && !opensForward)
                {
                    targetPoint = transform.position + (Vector3.forward * openingSize);
                }
            }

            distance = Vector3.Distance(transform.position, targetPoint);
        }

        StartCoroutine(MoveBack());
    }

    IEnumerator OpenDoor()
    {
        if (distance >= 0.5f)
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
        if (distance >= 0.5f)
        {
            yield return new WaitForEndOfFrame();
            doorClosing = true;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, doorSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.localPosition, originalPosition);

            // stop coroutine only works at a yield break, which would not happen until too late otherwise
            if (stopCoroutine)
            {
                stopCoroutine = false;
                yield break;
            }

            StartCoroutine(MoveBack());
            yield return null;
        }
        else
        {
            //when its fully closed
            doorClosing = false;
            doorIsOpen = false;

            // door can be triggered again by pressure plate event
            if (pressurePlates != null)
            {
                pplateTriggered = false;
            }
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
