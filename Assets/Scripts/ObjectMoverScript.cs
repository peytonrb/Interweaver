using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectMoverScript : MonoBehaviour
{
    [Header("Generic References")]
    [SerializeField] private GameObject[] waypoints; // waypoints for the path of the object

    [Header("Generic Variables")]
    private int nextWaypoint = 0; // the current target for the moving object
    [SerializeField][Range(1f, 10f)] private float speed = 2.0f; // speed of object
    [SerializeField] private bool active; // is the moving object active (currently moving)
    [Header("Reverse Settings")]
    [SerializeField] private bool canReverse;
    private bool reversing;
    [Header("Player Activation Settings")]
    [SerializeField][Tooltip("Check this if you want the movement to start when a character enters the")] private bool movementBasedOnCharacterPresense;
    private enum characterMissingBehavior
    {
        Continue,
        Return
    }
    [SerializeField][Tooltip("Will the platorm continue along the path or will it return to the start to wait for the player")] private characterMissingBehavior _characterMissingBehavior;
    [SerializeField][Tooltip("The time the platform waits for the character before continuing on")][Range(0f, 15f)] private float timeBeforeResuming;
    [SerializeField] private GameObject stickyGameObjectReference;

    void Start()
    {
        if (movementBasedOnCharacterPresense)
        {
            if (active)
            {
                Debug.Log("Object movement based on character presense, turning off active at Start");
                active = false;
            }
            StartCoroutine(WaitForCharacterOnObject());
        }
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        




        if (active) // only iterate if object is active
        {
            if (nextWaypoint < waypoints.Length && nextWaypoint >= 0)
            {
                float distanceToNextWaypoint = Vector3.Distance(transform.position, waypoints[nextWaypoint].transform.position); // gets distance between object and next waypoint

                if (distanceToNextWaypoint < 0.1f)
                {
                    if (!reversing)
                    {
                        nextWaypoint++;
                    }

                    else
                    {
                        nextWaypoint--;
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position, speed * Time.deltaTime);
                }
            }
            else
            {
                if (canReverse)
                {
                    if (reversing)
                    {
                        reversing = false;
                        nextWaypoint = 0;
                    }
                    else
                    {
                        reversing = true;
                        nextWaypoint = waypoints.Length - 1;
                    }
                }
                else
                {
                    nextWaypoint = 0; // return to start
                }
            }
        }
    }

    IEnumerator WaitForCharacterOnObject()
    {
        yield return new WaitUntil(StickyContainsCharacter);
        active = true;
        StartCoroutine(WaitForCharacterOffObject());
    }

    IEnumerator WaitForCharacterOffObject()
    {
        yield return new WaitUntil(StickyDoesntContainsCharacter);
        if (StickyDoesntContainsCharacter() && timeBeforeResuming > 0)
        {
            timeBeforeResuming -= Time.deltaTime;
            StartCoroutine(WaitForCharacterOffObject());
        }
        else if (timeBeforeResuming <= 0)
        {
            active = true;
        }

        
        StartCoroutine(WaitForCharacterOnObject());
    }

    private bool StickyContainsCharacter()
    {
        if (stickyGameObjectReference.transform.childCount > 0)
        {
            return true;
        }
        
        return false;
    }

    private bool StickyDoesntContainsCharacter()
    {
        if (stickyGameObjectReference.transform.childCount > 0)
        {
            return false;
        }
        
        return true;
    }

    void OnDrawGizmos()
    {
        
    }
}
