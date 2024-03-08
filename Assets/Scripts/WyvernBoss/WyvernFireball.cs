using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WyvernFireball : MonoBehaviour
{
    private GameObject weaver;
    private Rigidbody rb;
    private PlayerControllerNew playercontroller;
    private WeaveController weaveController;
    private GameObject familiar;
    private FamiliarScript familiarscript;
    private WeaveableObject weaveable;
    public float speed;
    private bool wasWoven;
    public float breakableObjectDetectionProximity;
    private bool breakableObjectFound;
    private Vector3 breakableObjectPosition;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        familiar = GameObject.FindGameObjectWithTag("Familiar");
        familiarscript = familiar.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerControllerNew>();
        weaveController = weaver.GetComponent<WeaveController>();
        weaveable = GetComponent<WeaveableObject>();
        rb = GetComponent<Rigidbody>();

        wasWoven = false;
        breakableObjectFound = false;
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
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(familiar.transform.position.x, familiar.transform.position.y + 1f, familiar.transform.position.y), speed * Time.deltaTime);
        }
        else {
            if (wasWoven == false) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(weaver.transform.position.x, weaver.transform.position.y + 1f, weaver.transform.position.z), speed * Time.deltaTime);
            }
            else {
                if (weaveController.isWeaving == false) {
                    if (breakableObjectFound == false) {
                        rb.velocity = Vector3.down * 0;
                        DetectBreakableObject();
                    }
                    else {
                        transform.position = Vector3.MoveTowards(transform.position, breakableObjectPosition, speed * Time.deltaTime);
                    }
                }
            }
        } 
    }

    //This detects if an object has a BreakObject script attached to it.
    void DetectBreakableObject() {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, breakableObjectDetectionProximity);
        foreach (Collider colliderFound in hitCollider) {
            if (colliderFound.gameObject.TryGetComponent<BreakObject>(out BreakObject breakObject)) {
                breakableObjectPosition = breakObject.gameObject.transform.position;
                breakableObjectFound = true;
                Debug.Log("Object found");
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
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Familiar")) {
            familiarscript.Death();
            Destroy(gameObject);
        }
        
    }
}
