using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HoverCrystalScript : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rigidBody;
    private WeaveableObject weaveable; // make sure to change when script name gets changed as well!!!
    [Header("Variables")]
    private Vector3 pointToRiseTo = Vector3.up;
    [SerializeField] private float maxTimeToShatter = 5f; 
    [SerializeField] private float hoverHeight = 10f;
    //[SerializeField] private bool startShatterTimeBeforePeak;
    private float distance = -1f;
    private float TimeToShatter;
    public bool hoverBegan = false;
    private bool isCombined;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        weaveable = GetComponent<WeaveableObject>(); 
    }

    public void StartHover(GameObject other)
    {
        pointToRiseTo = transform.position + (Vector3.up * hoverHeight);
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        distance = Vector3.Distance(transform.position, pointToRiseTo);
        gameObject.layer = LayerMask.NameToLayer("Default");
        Debug.Log(other);
        other.layer = LayerMask.NameToLayer("Default");
        StartCoroutine(Hover(other));
    }


    public IEnumerator Hover(GameObject other)
    {
        
        if (distance >= 0.1f)
        {
            yield return new WaitForEndOfFrame();
            // actually floats up
            hoverBegan = true;
            transform.position = Vector3.MoveTowards(transform.position, pointToRiseTo, 2f * Time.deltaTime);
            distance = Vector3.Distance(transform.position, pointToRiseTo);
            StartCoroutine(Hover(other));
            yield return null;
        }
        else
        {
            //when its reached the top
            TimeToShatter = maxTimeToShatter;
            StartCoroutine(ShatterCountdown(other));
            hoverBegan = false;
        }

        yield break;
    }

    IEnumerator ShatterCountdown(GameObject other)
    {
        yield return new WaitForSeconds(TimeToShatter);
        gameObject.layer = LayerMask.NameToLayer("weaveObject");
        other.layer = LayerMask.NameToLayer("weaveObject");
        rigidBody.constraints = RigidbodyConstraints.None;
        WeaveableManager.Instance.DestroyJoints(other.GetComponent<WeaveableObject>().listIndex);
        isCombined = false;
        yield return null;
    }

    void OnDrawGizmos()
    {
        if (!hoverBegan && !isCombined)
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * hoverHeight);
        }
        else
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * distance);
        }
    }
}
