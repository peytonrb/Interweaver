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
    [SerializeField] private int phases; //0 = no phase, 1 = fireball, 2 = magic circle, 3 = flamethrower, 4 = flee
    private Rigidbody rb;

    [Header("Fireballs")]
    public GameObject fireball;
    [SerializeField] private float fireballtimer; //Wait time between fireballs

    [Header("Magic Circles")]
    public GameObject magicCircle;
    [SerializeField] private float spawnradius;
    [SerializeField] private float magicCircleTimer; //Wait time between magic circles

    [Header("Flamethrower")]
    public GameObject flamethrower;
    private bool windup;
    private bool blowFire;
    [SerializeField] private float windupTimer; //The amount of time before the windup
    [SerializeField] private float windupAngle; //The angle amount the wyvern turns before blowing fire
    [SerializeField] private float windupRotationSpeed; //The speed of winding rotation
    [SerializeField] private float blowFireAngle; //The angle amount the wyvern turns while blowing fire
    [SerializeField] private float blowFireRotationSpeed; //The speed of blowing fire while rotating
    private float newrotation;
    private bool gotNewRotation;
    private bool reseting;

    private float startingFireballTimer;
    private float startingMagicCircleTimer;
    private float startingWindupTimer;

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
        startingWindupTimer = windupTimer;
        windup = false;
        blowFire = false;
        reseting = false;

        if (windupAngle > 0 && blowFireAngle > 0) {
            blowFireAngle = -blowFireAngle; 
        }
        else if (windupAngle < 0 && blowFireAngle < 0) {
            blowFireAngle = -blowFireAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (familiarScript.myTurn) {
            if (windup == false) {
                if (reseting == true) {
                    
                }
                else {
                    transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));
                }
            }
        }
        else {
            if (windup == false) {
                if (reseting == true) {
                    
                }
                else {
                    transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));
                }
            }
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
            //FLAMETHROWER
            case 3:
                if (!familiarScript.myTurn) {
                    if (!blowFire) {
                        if (windup == true) {
                            WindingUp();
                        }
                        else {
                            windupTimer -= Time.deltaTime;
                            if (windupTimer <= 0) {
                                windup = true;
                            }
                        }
                    }
                    else {
                        BlowFire();
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

    void SpawnFire() {
        Instantiate(flamethrower,transform,false);
        
    }

    void WindingUp() {
        if (gotNewRotation == false) {
            newrotation = transform.eulerAngles.y + windupAngle;
            gotNewRotation = true;
        }

        transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, newrotation, windupRotationSpeed * Time.deltaTime), 0);
        if (transform.eulerAngles.y >= newrotation) {
            SpawnFire();
            blowFire = true;
            gotNewRotation = false;
        }
    }

    void BlowFire() {
        if (gotNewRotation == false) {
            newrotation = transform.eulerAngles.y + blowFireAngle;
            gotNewRotation = true;
        }

        transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, newrotation, blowFireRotationSpeed * Time.deltaTime), 0);
        if (transform.eulerAngles.y <= newrotation) {
            Debug.Log("Flamethrower complete");
            ResetPhase3();
            WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
            wyvernFlamethrower.KillThyself();
        }
    }

    void ResetPhase3() {
        windupTimer = startingWindupTimer;
        blowFire = false;
        windup = false;
        gotNewRotation = false;
        reseting = true;
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
