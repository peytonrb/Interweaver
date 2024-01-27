using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolePillarScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    [SerializeField] private GameObject dirtPillar;
    public List<GameObject> pillarList = new List<GameObject>();
    [Header("Variables")]
    [SerializeField] private int maxPillarCount = 1;
    [SerializeField] [Range (10f, 50)] private float maxPillarHeight = 25f;
    [SerializeField] private float pillarBuildSpeed = 1.5f;
    private bool pillarBuildBegan;
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
        GameObject newPillar = Instantiate(dirtPillar);
        newPillar.transform.position = transform.position;
        pillarList.Add(newPillar);
    }

    private void DestroyPillar()
    {

    }

    public void RaisePillar()
    {
        pillarList[maxPillarCount - 1].transform.position = Vector3.MoveTowards(pillarList[0].transform.position, Vector3.up * 50f, pillarBuildSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        /*if (!pillarBuildBegan)
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * maxPillarHeight);
        }
        else
        {
            DrawArrow.ForGizmo(transform.position, Vector3.up * distance);
        }*/
    }
}
