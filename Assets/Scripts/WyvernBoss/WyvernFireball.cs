using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WyvernFireball : MonoBehaviour
{
    private GameObject weaver;
    private PlayerControllerNew playercontroller;
    private GameObject familiar;
    private FamiliarScript familiarscript;
    private WeaveableObject weaveable;
    public float speed;
    private bool wasWoven;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        familiar = GameObject.FindGameObjectWithTag("Familiar");
        familiarscript = familiar.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerControllerNew>();
        weaveable = GetComponent<WeaveableObject>();

        wasWoven = false;
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
            if (weaveable.isBeingWoven == false) {
                HomingMissile();
                if (transform.position.magnitude > 1000) {
                    Destroy(gameObject);
                }
                
            }
            else {
                wasWoven = true;
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
            if (wasWoven == false) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(weaver.transform.position.x, weaver.transform.position.y + 0.75f, weaver.transform.position.z), speed * Time.deltaTime);
            }
        } 
    }

    void OnCollisionEnter(Collision other) {
        if (!other.gameObject.CompareTag("Boss")) {
            if (other.gameObject.TryGetComponent<BreakObject>(out BreakObject breakableObject)) {
                breakableObject.BreakMyObject();
                Destroy(gameObject);
            }
            else {
                if (wasWoven == false) {
                    Destroy(gameObject);
                }
            }
            
        }
        if (other.gameObject.CompareTag("Player")) {
            playercontroller.Death();
            if (weaveable.isBeingWoven == false) {
                Destroy(gameObject);
            }
        }
        if (other.gameObject.CompareTag("Familiar")) {
            familiarscript.Death();
            Destroy(gameObject);
        }
        
    }
}
