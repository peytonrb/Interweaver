using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCrystalScript : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rigidBody;
    private WeaveableNew weaveable; // make sure to change when script name gets changed as well!!!
    [Header("Variables")]
    private Vector3 pointToRiseTo = Vector3.up;
    [SerializeField] private float maxTimeToShatter = 5f; 
    [SerializeField] private float hoverHeight = 10f;
    //[SerializeField] private bool startShatterTimeBeforePeak;
    private float distance = -1f;
    private float TimeToShatter;
    private bool hoverBegan;
    private bool isCombined;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        weaveable = GetComponent<WeaveableNew>(); 
        
        StartCoroutine(Hover());
    }

    IEnumerator Hover()
    {
        yield return new WaitUntil(CrystalIsCombined);
        distance = Vector3.Distance(transform.position, pointToRiseTo);
        
        if (!hoverBegan)
        {
            pointToRiseTo = transform.position + (Vector3.up * hoverHeight);
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            RemoveLayerOfWovenObjects();
        }
        if (distance <= 0.1f)
        {
            TimeToShatter = maxTimeToShatter;
            StartCoroutine(ShatterCountdown());
            hoverBegan = false;
            yield return null;
        }
        else
        {
            hoverBegan = true;
            transform.position = Vector3.MoveTowards(transform.position, pointToRiseTo, 2f * Time.deltaTime);
            StartCoroutine(Hover());
        }
    }

    IEnumerator ShatterCountdown()
    {
        yield return new WaitForSeconds(TimeToShatter);
        rigidBody.constraints = RigidbodyConstraints.None;
        RestoreLayerOfWovenObjects();
        weaveable.Uncombine();
        isCombined = false;
        StartCoroutine(Hover());
        yield return null;
    }

    private bool CrystalIsCombined()
    {
        if (weaveable.isCombined && !weaveable.isWoven)
        {
            isCombined = true;
            return true;
        }
        return false;
    }

    private void RemoveLayerOfWovenObjects()
    {
        List<WeaveableNew> wovenObjects = weaveable.GetListOfWovenObjects();
        foreach(WeaveableNew weaveable in wovenObjects)
        {   
            weaveable.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    } 

    private void RestoreLayerOfWovenObjects()
    {
        List<WeaveableNew> wovenObjects = weaveable.GetListOfWovenObjects();
        foreach(WeaveableNew weaveable in wovenObjects)
        {   
            //weaveable.Uncombine();
            weaveable.RestoreOriginalLayer();
        }
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
