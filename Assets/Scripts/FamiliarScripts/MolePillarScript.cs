using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MolePillarScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    [SerializeField] private GameObject dirtPillar;
    public List<GameObject> pillarList = new List<GameObject>();
    [Header("Variables")]
    [SerializeField] [Range (1, 10)] private int maxPillarCount = 1;
    [SerializeField] [Range (10f, 50)] private float maxPillarHeight = 25f;
    [SerializeField] private float pillarBuildSpeed = 1.5f;
    private bool pillarBuilding;
    private Vector3 pointToRiseTo = Vector3.up;
    private float distance = -1f;
    public bool deploy;
    public bool scronk;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (scronk)
        {
            //movementScript.active = false;
            RaisePillar();
        }
        if (deploy)
        {
            DeployPillar();
            deploy = false;
        }
    }

    public void DeployPillar()
    {
        if (pillarList.Count() >= maxPillarCount)
        {
            Destroy(pillarList[0]);
            pillarList.RemoveAt(0);
        }

        GameObject newPillar = Instantiate(dirtPillar);
        newPillar.transform.position = transform.position;
        pillarList.Add(newPillar);
    }

    private void DestroyPillar()
    {

    }

    public void RaisePillar()
    {
        distance = Vector3.Distance(pillarList[pillarList.Count-1].transform.position, pointToRiseTo);
        if (!pillarBuilding)
        {
            pointToRiseTo = transform.position + (Vector3.up * maxPillarHeight);
            pillarBuilding = true;
        }
        else if (distance > 0.1f)
        {
            pillarList[pillarList.Count-1].transform.position = Vector3.MoveTowards(pillarList[pillarList.Count-1].transform.position, pointToRiseTo, pillarBuildSpeed * Time.deltaTime);
        }
        else
        {
            scronk = false; // auto stop input
            pillarBuilding = false;
        }
    }

    void OnDrawGizmos()
    {
        if (!pillarBuilding)
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * maxPillarHeight);
        }
        else
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * distance);
        }
    }
}
