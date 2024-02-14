using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFireball : MonoBehaviour
{
    private GameObject weaver;
    private PlayerController playercontroller;
    private GameObject familiar;
    private FamiliarScript familiarscript;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        familiar = GameObject.FindGameObjectWithTag("Familiar");
        familiarscript = familiar.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerController>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (familiarscript.myTurn) {
            HomingMissile();
            if (transform.position.magnitude > 1000) {
                Destroy(gameObject);
            }
        }
        else {
            HomingMissile();
            if (transform.position.magnitude > 1000) {
                Destroy(gameObject);
            }
        }
    }

    //Function that moves the fireball towards the player/familiar
    void HomingMissile() {
        //If familiar's turn, then move towards the familiar.
        if (familiarscript.myTurn) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(familiar.transform.position.x, familiar.transform.position.y + 0.75f, familiar.transform.position.y), speed * Time.deltaTime);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(weaver.transform.position.x, weaver.transform.position.y + 0.75f, weaver.transform.position.z), speed * Time.deltaTime);
        } 
    }

    void OnCollisionEnter(Collision other) {
        if (!other.gameObject.CompareTag("Boss")) {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player")) {
            playercontroller.Death();
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Familiar")) {
            familiarscript.Death();
            Destroy(gameObject);
        }
        
    }
}
