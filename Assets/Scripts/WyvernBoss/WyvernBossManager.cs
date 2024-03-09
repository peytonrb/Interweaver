using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WyvernBossManager : MonoBehaviour
{
    private GameObject weaver;
    private PlayerControllerNew playercontroller;
    private GameObject stag;
    private FamiliarScript familiarScript;
    public int phases; //0 = no phase, 1 = fireball, 2 = magic circle, 3 = flamethrower, 4 = flee
    //private Rigidbody rb;
    private NavMeshAgent navMeshAgent;
    private bool moveToNextRoom; //If moving to next room
    private int currentRoom; //Gets current room
    [SerializeField] private Transform[] roomDestinations; //Room destinations that the boss moves towards when changing rooms
    private bool gotDestination;

    [Header("Fireballs")]
    public GameObject fireball;
    [SerializeField] [Tooltip("Wait time between throwing fireballs.")] private float fireballtimer; //Wait time between fireballs
    [SerializeField] [Tooltip("Amount of fireballs in a single phase.")] private int fireballAmount; //Amount of fireballs in a single "phase"

    [Header("Magic Circles")]
    public GameObject magicCircle;
    [SerializeField] [Tooltip("Radius of randomized spawner.")] private float spawnradius;
    [SerializeField] [Tooltip("Wait time between magic circle spawning.")] private float magicCircleTimer; //Wait time between magic circles
    [SerializeField] private float magicCircleCooldown;
    [Tooltip("Amount of magic circles to spawn in a single phase.")] public int magicCircleAmount;
    [Tooltip("If true, magic circles will spawn in predetermined arrangements")] public bool useConfigurations;
    [SerializeField] private GameObject[] configurations;
    [HideInInspector] public bool configurationIsActive;
    private bool spawnedConfiguration;

    [Header("Flamethrower")]
    public GameObject flamethrower;
    private bool windup;
    private bool blowFire;
    [SerializeField] [Tooltip("Amount of time before the wyvern winds up to attack.")] private float windupTimer; //The amount of time before the windup
    [SerializeField] [Tooltip("The amount of wind up before blowing fire.")] private float windupAngle; //The angle amount the wyvern turns before blowing fire
    [SerializeField] [Tooltip("Rotation speed of the windup.")] private float windupRotationSpeed; //The speed of winding rotation
    [SerializeField] [Tooltip("The total angle the wyvern blows fire.")] private float blowFireAngle; //The angle amount the wyvern turns while blowing fire
    [SerializeField] [Tooltip("Rotation speed of the blowing fire rotation.")] private float blowFireRotationSpeed; //The speed of blowing fire while rotating
    private float newrotation;
    private bool gotNewRotation;
    [HideInInspector] public bool reseting;

    private float startingFireballTimer;
    private int startingFireballAmount;
    private float startingMagicCircleTimer;
    private float startingMagiCircleCooldown;
    private int startingMagicCircleAmount;
    private float startingWindupTimer;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        stag = GameObject.FindGameObjectWithTag("Familiar");
        familiarScript = stag.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerControllerNew>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //transform.position = new Vector3(roomDestinations[currentRoom].transform.position.x, transform.position.y, roomDestinations[currentRoom].transform.position.z);
        //rb = GetComponent<Rigidbody>();

        startingFireballTimer = fireballtimer;
        startingFireballAmount = fireballAmount;
        startingMagicCircleTimer = magicCircleTimer;
        startingMagicCircleAmount = magicCircleAmount;
        startingMagiCircleCooldown = magicCircleCooldown;
        startingWindupTimer = windupTimer;
        windup = false;
        blowFire = false;
        reseting = false;
        configurationIsActive = false;
        spawnedConfiguration = false;
        moveToNextRoom = false;

        transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveToNextRoom == false) {
            if (familiarScript.myTurn) {
                if (windup == false) {
                    if (reseting == true) {
                        reseting = false;
                    }
                    else {
                        transform.LookAt(new Vector3(stag.transform.position.x,transform.position.y,stag.transform.position.z));
                    }
                }
            }
            else {
                if (windup == false) {
                    if (reseting == true) {
                        reseting = false;
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
                    else {
                        if (fireballtimer > 0) {
                            fireballtimer -= Time.deltaTime;
                        }
                        else {
                            if (!familiarScript.isDead) {
                                ThrowFireball();
                            }
                            fireballtimer = startingFireballTimer;
                        }
                    }
                break;
                //MAGIC CIRCLE
                case 2:
                    if (!familiarScript.myTurn) {
                        if (reseting == false) {
                            if (useConfigurations) {
                                if (spawnedConfiguration == false) {
                                    if (!playercontroller.isDead) {
                                        SpawnMagicCircle();
                                    } 
                                }
                            }
                            else {
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
                        }
                    }
                    else {
                        if (reseting == false) {
                            if (useConfigurations) {
                                if (spawnedConfiguration == false) {
                                    if (!familiarScript.isDead) {
                                        SpawnMagicCircle();
                                    } 
                                }
                            }
                            else {
                                if (magicCircleTimer > 0) {
                                    magicCircleTimer -= Time.deltaTime;
                                }
                                else {
                                    if (!familiarScript.isDead) {
                                        SpawnMagicCircle();
                                    }
                                    magicCircleTimer = startingMagicCircleTimer;
                                }
                            }
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
        else {
            if (gotDestination == false) {
                ChangeRooms(currentRoom);
            }
            else {
                if (transform.position.x == roomDestinations[currentRoom].transform.position.x && transform.position.z == roomDestinations[currentRoom].transform.position.z) {
                    gotDestination = false;
                    moveToNextRoom = false;
                }
            }
        }
        
    }

    void ChangeRooms(int currentroom) {
        int newroom = currentroom + 1;
        Vector3 newdestination = new Vector3(roomDestinations[newroom].transform.position.x, transform.position.y, roomDestinations[newroom].transform.position.z);
        navMeshAgent.SetDestination(newdestination);
        currentRoom = newroom;
        gotDestination = true;
    }

    void ChooseRandom(int previousPhase) {
        int newPhase = Random.Range(0,2);

        if (spawnedConfiguration) {
            spawnedConfiguration = false;
        }

        switch (previousPhase) {
            case 0:
                newPhase = Random.Range(0,2);
                if (newPhase == 0) {
                    phases = 1;
                }
                else if (newPhase == 1) {
                    phases = 2;
                }
            break;

            //FIREBALL | PHASE = 1
            case 1:
                if (newPhase == 0) {
                    phases = 2;
                }
                else {
                    phases = 3;
                }
            break;

            //MAGIC CIRCLE | PHASE = 2
            case 2:
                if (newPhase == 0) {
                    phases = 1;
                }
                else {
                    phases = 3;
                }
            break;

            //FLAMETHROWER | PHASE = 3
            case 3:
                if (newPhase == 0) {
                    phases = 1;
                }
                else {
                    phases = 2;
                }
            break;
        }
    }

    void ThrowFireball() {
        if (fireballAmount > 0) {
            Instantiate(fireball,transform.position,Quaternion.identity);
            fireballAmount -= 1;
        }
        else {
            reseting = true;
            fireballAmount = startingFireballAmount;
        }
        
    }

    void SpawnMagicCircle() {
        if (useConfigurations == false) {
            if (magicCircleAmount > 0) {
                if (!familiarScript.myTurn) {
                    Vector3 randomposition = Random.insideUnitCircle * spawnradius;
                    Vector3 newposition = new Vector3(weaver.transform.position.x + randomposition.x, weaver.transform.position.y - 7f, weaver.transform.position.z + randomposition.y);
                    Instantiate(magicCircle,newposition,Quaternion.identity);
                }
                else {
                    Vector3 randomposition = Random.insideUnitCircle * spawnradius;
                    Vector3 newposition = new Vector3(stag.transform.position.x + randomposition.x, stag.transform.position.y - 1f, stag.transform.position.z + randomposition.y);
                    Instantiate(magicCircle,newposition,Quaternion.identity);
                }
                magicCircleAmount -= 1;
            }
            else {
                StartCoroutine(Cooldown());
                reseting = true;
                magicCircleAmount = startingMagicCircleAmount;
            }
        }
        else {
            int randomConfig = Random.Range(0, configurations.Length);
            Vector3 newposition = new Vector3(weaver.transform.position.x, weaver.transform.position.y - 7f, weaver.transform.position.z);
            Instantiate(configurations[randomConfig],newposition,Quaternion.identity);
            spawnedConfiguration = true;
        }
        
    }

    public IEnumerator Cooldown() {
        switch (phases) {
            case 2:
                yield return new WaitForSeconds(magicCircleCooldown);
            break;
        }
        Debug.Log("Cooldown finished");
        magicCircleCooldown = startingMagiCircleCooldown;
        reseting = false;
        yield break;
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
        //Debug.Log("euler angles y = " + transform.eulerAngles.y);
        //Debug.Log("new rotation = " + newrotation);
        if (transform.eulerAngles.y >= newrotation) {
            SpawnFire();
            blowFire = true;
            gotNewRotation = false;
        }
    }

    void BlowFire() {
        if (gotNewRotation == false) {
            newrotation = transform.eulerAngles.y + -blowFireAngle;
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

    public void SwitchToPhase(int newphase, int previousphase) {
        phases = newphase;
        switch (previousphase) {
            case 1:
                fireballAmount = startingFireballAmount;
                fireballtimer = startingFireballTimer;
            break;
            case 2:
                magicCircleAmount = startingMagicCircleAmount;
                magicCircleTimer = startingMagicCircleTimer;
                magicCircleCooldown = startingMagiCircleCooldown;
            break;
            case 3:
                WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
                wyvernFlamethrower.KillThyself();
                ResetPhase3();
            break;
        }
        reseting = true;

    }

    public void WeaverStagPhaseSwap() {
        
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerControllerNew player = other.gameObject.GetComponent<PlayerControllerNew>();
            if (player != null) {
                player.Death();
            }
            
        }
    }
}
