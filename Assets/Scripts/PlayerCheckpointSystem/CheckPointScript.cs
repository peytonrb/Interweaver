using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private GameMasterScript GM;
    private CameraMasterScript cms;
    public int checkpointNum;

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        cms = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GM.WeaverCheckPointPos = transform.position;
            GM.WeaverCheckPointNum = checkpointNum;
            cms.ResetWeaverCamerasTriggered();
        }
        else if (other.CompareTag("Familiar"))
        {
            GM.FamiliarCheckPointPos = transform.position;
            GM.FamiliarCheckPointNum = checkpointNum;
            cms.ResetFamiliarCamerasTriggered();
        }
    }
}
