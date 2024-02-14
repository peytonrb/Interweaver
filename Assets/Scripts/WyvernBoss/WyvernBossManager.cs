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
    public GameObject fireball;
    [SerializeField] private float fireballtimer;
    private float startingFireballTimer;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        stag = GameObject.FindGameObjectWithTag("Familiar");
        familiarScript = stag.GetComponent<FamiliarScript>();
        rb = GetComponent<Rigidbody>();

        startingFireballTimer = fireballtimer;

    }

    // Update is called once per frame
    void Update()
    {
        if (familiarScript.myTurn) {
            transform.LookAt(new Vector3(stag.transform.position.x,transform.position.y,stag.transform.position.z));
        }
        else {
            transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));
            if (fireballtimer > 0) {
                fireballtimer -= Time.deltaTime;
            }
            else {
                ThrowFireball();
                fireballtimer = startingFireballTimer;
            }
        }
    }

    void ThrowFireball() {
        Instantiate(fireball,transform.position,Quaternion.identity);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.Death();
        }
    }
}
