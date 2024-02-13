using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WyvernBossManager : MonoBehaviour
{
    private GameObject weaver;
    private GameObject stag;
    private FamiliarScript familiarScript;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        stag = GameObject.FindGameObjectWithTag("Familiar");
        familiarScript = stag.GetComponent<FamiliarScript>();
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (familiarScript.myTurn) {
            transform.LookAt(new Vector3(stag.transform.position.x,transform.position.y,stag.transform.position.z));
        }
        else {
            transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.Death();
        }
    }
}
