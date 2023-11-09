using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private GameMasterScript GM;

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GM.WeaverCheckPointPos = transform.position;
        }
        else if (other.CompareTag("Familiar"))
        {
            GM.FamiliarCheckPointPos = transform.position;
        }
    }
}
