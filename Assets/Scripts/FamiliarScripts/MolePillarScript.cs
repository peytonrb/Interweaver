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
    [HideInInspector] public bool digInputPressed;
    [HideInInspector] public bool build;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (build) // this is bad. I know this is bad. Sorry.
        {
            RaisePillar();
        }
    }

    public void DeployPillar()
    {
        if (pillarList.Count() >= maxPillarCount)
        {
            DestroyPillar();
        }

        GameObject newPillar = Instantiate(dirtPillar);
        newPillar.transform.position = transform.position;
        pillarList.Add(newPillar);
    }

    private void DestroyPillar()
    {
        Destroy(pillarList[0]);
        pillarList.RemoveAt(0);
    }

    public void RaisePillar()
    {
        if (!digInputPressed)
        {
            PillarBuildEnd();
        }
        else
        {
            distance = Vector3.Distance(pillarList[pillarList.Count-1].transform.position, pointToRiseTo);
            if (!pillarBuilding)
            {
                movementScript.active = false;
                pointToRiseTo = transform.position + (Vector3.up * maxPillarHeight);
                pillarBuilding = true;
            }
            else if (distance > 0.1f && pillarBuilding)
            {
                pillarList[pillarList.Count-1].transform.position = Vector3.MoveTowards(pillarList[pillarList.Count-1].transform.position, pointToRiseTo, pillarBuildSpeed * Time.deltaTime);
            }
            else
            {
                PillarBuildEnd();
            }
        }
    }

    public void PillarBuildEnd()
    {
        movementScript.active = true;
        build = false;
        pillarBuilding = false;
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
