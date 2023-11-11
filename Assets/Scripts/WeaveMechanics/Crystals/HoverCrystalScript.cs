using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class HoverCrystalScript : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rigidBody;
    private WeaveableNew weaveable; // make sure to change when script name gets changed as well!!!
    private Vector3 pointToRiseTo = Vector3.up;
    private float hoverDistance = 10f;
    private float distance = -1f;
    private float TimeToShatter;
    [SerializeField] private float maxTimeToShatter = 5f; 
    private Vector3 startPoint;
    bool scrunk;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        weaveable = GetComponent<WeaveableNew>(); 
        
        StartCoroutine(Hover());
    }

    IEnumerator Hover()
    {
        Debug.Log("WEEEEEE");
        yield return new WaitUntil(CrystalIsCombined);
        distance = Vector3.Distance(transform.position, pointToRiseTo);
        
        if (!scrunk)
        {
            pointToRiseTo = transform.position + (Vector3.up * hoverDistance);
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
        if (distance <= 0.1f)
        {
            TimeToShatter = maxTimeToShatter;
            StartCoroutine(ShatterCountdown());
            yield return null;
        }
        else
        {
            scrunk = true;
            transform.position = Vector3.MoveTowards(transform.position, pointToRiseTo, 2f * Time.deltaTime);
            StartCoroutine(Hover());
        }
        Debug.Log("WAHOOO");
    }

    IEnumerator ShatterCountdown()
    {
        yield return new WaitForSeconds(TimeToShatter);
        rigidBody.constraints = RigidbodyConstraints.None;
        yield return null;
    }

    private bool CrystalIsCombined()
    {
        if (weaveable.isCombined && !weaveable.isWoven)
        {
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (distance <= 0)
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * hoverDistance);
        }
        else
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * distance);
        }
    }
}
