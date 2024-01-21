using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterScript : MonoBehaviour
{
    private static GameMasterScript instance;
    [HideInInspector] public GameObject[] weaverCheckpoints;
    [HideInInspector] public GameObject[] familiarCheckpoints;
    public int totalLostSouls;
    public Vector3 WeaverCheckPointPos;
    public int WeaverCheckPointNum;
    public Vector3 FamiliarCheckPointPos;
    public int FamiliarCheckPointNum;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        weaverCheckpoints = GameObject.FindGameObjectsWithTag("WeaverCheckpoint");
        familiarCheckpoints = GameObject.FindGameObjectsWithTag("FamiliarCheckpoint");

        for (int i = 0; i < weaverCheckpoints.Length; i++) {
            CheckPointScript cps = weaverCheckpoints[i].GetComponent<CheckPointScript>();
            cps.checkpointNum = i;
        }
        for (int i = 0; i < familiarCheckpoints.Length; i++) {
            CheckPointScript cps = familiarCheckpoints[i].GetComponent<CheckPointScript>();
            cps.checkpointNum = i;
        }
    }

    public void GoToNextCheckpoint() {
        GameObject weaver = GameObject.FindGameObjectWithTag("Player");
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");

        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        
        if (familiarScript.myTurn) {
            if (FamiliarCheckPointNum < familiarCheckpoints.Length - 1) {
                familiar.transform.position = familiarCheckpoints[FamiliarCheckPointNum+1].transform.position;
            }
        }
        else {
            if (WeaverCheckPointNum < weaverCheckpoints.Length - 1) {
                weaver.transform.position = weaverCheckpoints[WeaverCheckPointNum+1].transform.position;
            }
        }

    }

    public void GoToPreviousCheckpoint() {
        GameObject weaver = GameObject.FindGameObjectWithTag("Player");
        GameObject familiar = GameObject.FindGameObjectWithTag("Familiar");

        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();

        if (familiarScript.myTurn) {
            if (FamiliarCheckPointNum > 0) {
                familiar.transform.position = familiarCheckpoints[FamiliarCheckPointNum-1].transform.position;
            }
        }
        else {
            if (WeaverCheckPointNum > 0) {
                weaver.transform.position = weaverCheckpoints[WeaverCheckPointNum-1].transform.position;
            }
        }
        
    }
}
