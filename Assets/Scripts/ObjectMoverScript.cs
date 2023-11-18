using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjectMoverScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] waypoints; // waypoints for the path of the object

    [Header("Variables")]
    [SerializeField][Range(1f, 10f)] private float speed = 2.0f; // speed of object
    [SerializeField] private bool active; // is the moving object active (currently moving)
    [SerializeField] private bool canReverse;
    private bool reversing;
    private int nextWaypoint = 0; // the current target for the moving object

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active) // only iterate if object is active
        {
            Debug.Log(nextWaypoint);
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
}
