using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public SensorController[] sensors;
    public PressurePlateScript[] pressurePlate;
    public bool opensVertically;
    public bool opensRight;
    public bool opensForward;
    public bool doesDoorClose;
    public float openingSize;
    public float doorSpeed = 2f;
    private Vector3 targetPoint;
    private Vector3 originalPosition;
    private float distance = -1f;
    private bool doorOpening = false;
    private bool doorClosing = false;
    private bool doorIsOpen = false;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (sensors != null && sensors.Length > 0 && doesDoorClose && doorIsOpen)
        {
            foreach (SensorController sensor in sensors)
            {
                if (!sensor.isActive)
                    CloseDoor();
            }
        }
    }

    public void MoveUpwards()
    {
        if (!doorIsOpen)
        {
            targetPoint = transform.position + (Vector3.up * openingSize);
            distance = Vector3.Distance(transform.position, targetPoint);
        }

        StartCoroutine(OpenDoor());
    }

    public void MoveSideways()
    {
        // does door open to the right or the left
        if (!doorIsOpen)
        {
            if (opensRight)
            {
                targetPoint = transform.position + (Vector3.right * openingSize);
            }
            else if (!opensRight && !opensForward)
            {
                targetPoint = transform.position + (Vector3.left * openingSize);
            }
            else if (opensForward)
            {
                targetPoint = transform.position + (Vector3.forward * openingSize);
            }
            else
            {
                targetPoint = transform.position + (Vector3.back * openingSize);
            }

            distance = Vector3.Distance(transform.position, targetPoint);
            StartCoroutine(OpenDoor());
        }
    }

    public void CloseDoor()
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
                else if (!opensRight && !opensForward)
                {
                    targetPoint = transform.position + (Vector3.right * openingSize);
                }
                else if (opensForward)
                {
                    targetPoint = transform.position + (Vector3.back * openingSize);
                }
                else
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
        if (distance >= 0.1f)
        {
            yield return new WaitForEndOfFrame();
            doorOpening = true;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, doorSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, targetPoint);
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
        if (distance >= 0.1f)
        {
            yield return new WaitForEndOfFrame();
            doorClosing = true;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, (doorSpeed / 2) * Time.deltaTime);
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
                else if (!opensRight && !opensForward)
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
                else if (!opensRight && !opensForward)
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
