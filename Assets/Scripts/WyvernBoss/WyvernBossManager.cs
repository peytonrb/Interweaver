using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WyvernBossManager : MonoBehaviour
{
    private GameObject weaver;
    private PlayerController playercontroller;
    private GameObject stag;
    private FamiliarScript familiarScript;
    private Rigidbody rb;

    [Header("Fireballs")]
    public GameObject fireball;
    [SerializeField] private float fireballtimer;

    [Header("Magic Circles")]
    public GameObject magicCircle;
    [SerializeField] private float spawnradius;
    [SerializeField] private float magicCircleTimer;

    private float startingFireballTimer;
    private float startingMagicCircleTimer;
    [SerializeField] private int phases; //0 = no phase, 1 = fireball, 2 = magic circle, 3 = flamethrower, 4 = flee

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        stag = GameObject.FindGameObjectWithTag("Familiar");
        familiarScript = stag.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        startingFireballTimer = fireballtimer;
        startingMagicCircleTimer = magicCircleTimer;
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

        switch (phases) {
            //FIREBALL
            case 1:
                if (!familiarScript.myTurn) {
                    if (fireballtimer > 0) {
                        fireballtimer -= Time.deltaTime;
                    }
                    else {
                        if (!playercontroller.isDead) {
                            ThrowFireball();
                        }
                        fireballtimer = startingFireballTimer;
                    }
                }
            break;
            //MAGIC CIRCLE
            case 2:
                if (!familiarScript.myTurn) {
                    if (magicCircleTimer > 0) {
                        magicCircleTimer -= Time.deltaTime;
                    }
                    else {
                        if (!playercontroller.isDead) {
                            SpawnMagicCircle();
                        }
                        magicCircleTimer = startingMagicCircleTimer;
                    }
                }
            break;
        }
        
    }

    void ThrowFireball() {
        Instantiate(fireball,transform.position,Quaternion.identity);
    }

    void SpawnMagicCircle() {
        Vector3 randomposition = Random.insideUnitCircle * spawnradius;
        Vector3 newposition = new Vector3(weaver.transform.position.x + randomposition.x, -5f, weaver.transform.position.z + randomposition.y);
        Instantiate(magicCircle,newposition,Quaternion.identity);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) {
                player.Death();
            }
            
        }
    }
}
