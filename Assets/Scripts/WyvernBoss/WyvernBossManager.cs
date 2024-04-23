using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WyvernBossManager : MonoBehaviour
{
    private GameObject weaver;
    private CharacterController weavercontroller;
    private PlayerControllerNew playercontroller;
    private GameObject stag;
    private CharacterController familiarcontroller;
    private FamiliarScript familiarScript;
    public int phases; //0 = no phase, 1 = fireball, 2 = magic circle, 3 = flamethrower, 4 = flee
    [HideInInspector] public int startingPhase;
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
    private float weaverStartingY;
    private float familiarStartingY;
    private bool waitUntilWeaverIsGrounded;
    private bool waitUntilFamiliarIsGrounded;

    [Header("Flamethrower")]
    public GameObject flamethrower;
    private bool windup;
    private bool blowFire;
    [SerializeField] [Tooltip("Amount of time before the wyvern winds up to attack.")] private float preWindupTimer; //The amount of time before the windup
    [SerializeField] [Tooltip("Amount of time to windup.")] private float windupTimer;
    [SerializeField] [Tooltip("Rotation speed of the windup.")] private float windupRotationSpeed; //The speed of winding rotation
    [SerializeField] [Tooltip("Amount of time to blow fire.")] private float blowFireTimer;
    [SerializeField] [Tooltip("Rotation speed of the blowing fire rotation.")] private float blowFireRotationSpeed; //The speed of blowing fire while rotating
    [HideInInspector] public bool reseting;

    [Header("PUT WYVERN TRIGGER MANAGER HERE")]
    public GameObject wyvernTriggerManager;
    private WyvernPhaseTriggerManager triggerManager;
    [SerializeField] private GameObject endCutsceneController;
    private EndCutsceneTrigger ect;

    [Header("Method invoke for wyvern injury")]
    public UnityEvent[] onHurtWyvern;

    private float startingFireballTimer;
    private int startingFireballAmount;
    private float startingMagicCircleTimer;
    private int startingMagicCircleAmount;
    private float startingPreWindupTimer;
    private float startingWindupTimer;
    private float startingBlowFireTimer;
    private int familiarCurrentPhase;
    private int weaverCurrentPhase;
    [HideInInspector] public bool stagWasLaunched;

    [Header("Audio")]
    [SerializeField] private AudioClip wyvernHurtSound;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        stag = GameObject.FindGameObjectWithTag("Familiar");
        weavercontroller = weaver.GetComponent<CharacterController>();
        familiarcontroller = stag.GetComponent<CharacterController>();
        familiarScript = stag.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerControllerNew>();
        ect = endCutsceneController.GetComponent<EndCutsceneTrigger>();
        if (wyvernTriggerManager != null) {
            triggerManager = wyvernTriggerManager.GetComponent<WyvernPhaseTriggerManager>();
        }

        startingFireballTimer = fireballtimer;
        startingFireballAmount = fireballAmount;
        startingMagicCircleTimer = magicCircleTimer;
        startingMagicCircleAmount = magicCircleAmount;
        startingPreWindupTimer = preWindupTimer;
        startingWindupTimer = windupTimer;
        startingBlowFireTimer = blowFireTimer;
        startingPhase = phases;
        windup = false;
        blowFire = false;
        reseting = false;
        configurationIsActive = false;
        spawnedConfiguration = false;
        moveToNextRoom = false;
        waitUntilWeaverIsGrounded = false;
        waitUntilFamiliarIsGrounded = false;
        familiarCurrentPhase = phases;
        weaverCurrentPhase = phases;

        transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));

         if (phases == 2) {
            if (weavercontroller.isGrounded) {
                weaverStartingY = weaver.transform.position.y;
                Debug.Log("Got Weaver y");
            }
            else {
                waitUntilWeaverIsGrounded = true;
            }
       
            if (familiarcontroller.isGrounded) {
                familiarStartingY = stag.transform.position.y;
                Debug.Log("Got familiar y");
            }
            else {
                waitUntilFamiliarIsGrounded = true;
            }
            
        }
        
    }

    public void StartingPosition() {
        transform.position = roomDestinations[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveToNextRoom == false) {
            if (familiarScript.myTurn) {
                if (windup == false) {
                    if (reseting == true) {
                        if (phases != 2) {
                            reseting = false;
                        }
                    }
                    else {
                        if (stagWasLaunched == false) {
                            transform.LookAt(new Vector3(stag.transform.position.x,transform.position.y,stag.transform.position.z));
                        }
                    }
                }
            }
            else {
                if (windup == false) {
                    if (reseting == true) {
                        if (phases != 2) {
                            reseting = false;
                        }
                    }
                    else {
                        if (stagWasLaunched == false) {
                            transform.LookAt(new Vector3(weaver.transform.position.x,transform.position.y,weaver.transform.position.z));
                        }
                    }
                }
            }

            switch (phases) {
                //FIREBALL
                case 1:
                    if (fireballtimer > 0) {
                        fireballtimer -= Time.deltaTime;
                    }
                    else {
                        if (!playercontroller.isDead) {
                            ThrowFireball();
                        }
                        fireballtimer = startingFireballTimer;
                    }
                break;
                //MAGIC CIRCLE
                case 2:
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
                break;
                //FLAMETHROWER
                case 3:
                    if (!blowFire) {
                        if (windup == true) {
                            WindingUp();
                        }
                        else {
                            preWindupTimer -= Time.deltaTime;
                            if (preWindupTimer <= 0) {
                                windup = true;
                            }
                        }
                    }
                    else {
                        BlowFire();
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

        if (waitUntilWeaverIsGrounded == true) {
            if (weavercontroller.isGrounded) {
                weaverStartingY = weaver.transform.position.y;
                waitUntilWeaverIsGrounded = false;
            }
        }
        if (waitUntilFamiliarIsGrounded == true) {
            if (familiarcontroller.isGrounded) {
                familiarStartingY = stag.transform.position.y;
                waitUntilFamiliarIsGrounded = false;
            }
        }

        // VFX for stag
        if (phases == 2 && !familiarScript.transform.Find("StagShieldParent").gameObject.activeSelf)
        {
            familiarScript.transform.Find("StagShieldParent").gameObject.SetActive(true); // sets shield on for stag
            familiarScript.transform.Find("StagShieldParent").GetComponent<ShieldVFXController>().shouldBeOn = true;
        }
        else if (phases != 2 && familiarScript.transform.Find("StagShieldParent").gameObject.activeSelf)
        {
            familiarScript.transform.Find("StagShieldParent").GetComponent<ShieldVFXController>().shouldBeOn = false; // sets shield off for stag
            StartCoroutine(WaitForVFX());
        }
    }

    /// <summary>
    /// Sends the wyvern to a new room via teleportation.
    /// </summary>
    /// <param name="currentroom"></param>
    void ChangeRooms(int currentroom) {
        int newroom = currentroom + 1;
        if (newroom <= roomDestinations.Length) {
            Vector3 newdestination = new Vector3(roomDestinations[newroom].transform.position.x, transform.position.y, roomDestinations[newroom].transform.position.z);
            transform.position = newdestination;
            currentRoom = newroom;
        }
        else {
            ect.StartCutscene();
            //SceneHandler.instance.LoadLevel("AnimaticCutscenes");
        }
        gotDestination = true;
    }

    void ThrowFireball() {
        if (fireballAmount > 0) {
            Instantiate(fireball,transform.position + new Vector3(0,6f,0),Quaternion.identity);
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
                    if (weaver.transform.position.y >= weaverStartingY - 0.5f && weaver.transform.position.y <= weaverStartingY + 0.5f) {
                        Vector3 randomposition = Random.insideUnitCircle * spawnradius;
                        Vector3 newposition = new Vector3(weaver.transform.position.x + randomposition.x, weaver.transform.position.y, weaver.transform.position.z + randomposition.y);
                        Instantiate(magicCircle,newposition,Quaternion.identity);
                        magicCircleAmount -= 1;
                    }
                }
                else {
                    if (stag.transform.position.y >= familiarStartingY - 0.5f && stag.transform.position.y <= familiarStartingY + 0.5f) {
                        Vector3 randomposition = Random.insideUnitCircle * spawnradius;
                        Vector3 newposition = new Vector3(stag.transform.position.x + randomposition.x, stag.transform.position.y, stag.transform.position.z + randomposition.y);
                        Instantiate(magicCircle,newposition,Quaternion.identity);
                        magicCircleAmount -= 1;
                    }
                }
            }
            else {
                StartCoroutine(Cooldown());
                reseting = true;
                magicCircleAmount = startingMagicCircleAmount;
            }
        }
        else {
            int randomConfig = Random.Range(0, configurations.Length);
            Vector3 newposition = new Vector3(weaver.transform.position.x, weaver.transform.position.y, weaver.transform.position.z);
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
        reseting = false;
        yield break;
    }

    void SpawnFire() {
        flamethrower.GetComponent<FlamethrowerController>().isActive = true;
    }

    void WindingUp() {
        //Use a quaternion instead of movetowardsangle
        if (windupTimer > 0) {
            transform.Rotate(Vector3.up * windupRotationSpeed * Time.deltaTime);
            windupTimer -= Time.deltaTime;
        }
        else {
            SpawnFire();
            blowFire = true;
        }
    }

    void BlowFire() {
        if (blowFireTimer > 0) {
            transform.Rotate(Vector3.down * blowFireRotationSpeed * Time.deltaTime);
            blowFireTimer -= Time.deltaTime;
        }
        else {
            ResetPhase3();
            //WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
            //wyvernFlamethrower.KillThyself();
            flamethrower.GetComponent<FlamethrowerController>().isActive = false;
        }
    }

    void ResetPhase3() {
        preWindupTimer = startingPreWindupTimer;
        windupTimer = startingWindupTimer;
        blowFireTimer = startingBlowFireTimer;
        blowFire = false;
        windup = false;
        reseting = true;
    }

    public void ResetPhase2GroundLevel(bool isWeaversTurn) {
        if (isWeaversTurn) {
            if (weavercontroller.isGrounded) {
                weaverStartingY = weaver.transform.position.y;
                Debug.Log("Reset Weaver Ground Level");
            }
            else {
                waitUntilWeaverIsGrounded = true;
            }
        }
        else {
            if (familiarcontroller.isGrounded) {
                familiarStartingY = stag.transform.position.y;
                Debug.Log("Reset Familiar Ground Level");
            }
            else {
                waitUntilFamiliarIsGrounded = true;
            }
        }
    }

    /// <summary>
    /// Switches to new phase.
    /// </summary>
    /// <param name="newphase"> 
    /// A reference to the phase that you are entering
    /// </param>
    /// <param name="previousphase">
    /// A reference to the phase that you are exiting
    /// </param>
    public void SwitchToPhase(int newphase, int previousphase, bool isWeaversTurn) {
        phases = newphase;

        //This does not account for if the weaver or stag jumps through a trigger yet.
        if (newphase == 2) {
            if (isWeaversTurn) {
                if (weavercontroller.isGrounded) {
                    weaverStartingY = weaver.transform.position.y;
                }
                else {
                    waitUntilWeaverIsGrounded = true;
                }
            }
            else {
                if (familiarcontroller.isGrounded) {
                    familiarStartingY = stag.transform.position.y;
                }
                else {
                    waitUntilFamiliarIsGrounded = true;
                }
            }
        }

        if (isWeaversTurn) {
            weaverCurrentPhase = newphase;
        }
        else {
            familiarCurrentPhase = newphase;
        }
        switch (previousphase) {
            case 1:
                fireballAmount = startingFireballAmount;
                fireballtimer = startingFireballTimer;
            break;
            case 2:
                magicCircleAmount = startingMagicCircleAmount;
                magicCircleTimer = startingMagicCircleTimer;
            break;
            case 3:
                //WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
                //if (wyvernFlamethrower != null) {
                //    wyvernFlamethrower.KillThyself();
                //}
                if (flamethrower.GetComponent<FlamethrowerController>().isActive)
                {
                    flamethrower.GetComponent<FlamethrowerController>().isActive = false;
                }

                ResetPhase3();
            break;
        }
        reseting = true;

    }

    /// <summary>
    /// Reads whos turn it was before the swap occured.
    /// </summary>
    /// <param name="familiarsTurn">
    /// Should only read the bool myTurn from FamiliarScript.
    /// </param>
    public void WeaverOrFamiliar(bool familiarsTurn) {
        if (familiarsTurn == false) {
            //WHEN SWAPPING TO FAMILIAR
            phases = familiarCurrentPhase;

            //This resets the weaver's phase
            switch (weaverCurrentPhase) {
                case 1:
                    fireballAmount = startingFireballAmount;
                    fireballtimer = startingFireballTimer;
                break;
                case 2:
                    magicCircleAmount = startingMagicCircleAmount;
                    magicCircleTimer = startingMagicCircleTimer;
                    StopCoroutine(Cooldown());
                break;
                case 3:
                    //WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
                    //if (wyvernFlamethrower != null) {
                    //    wyvernFlamethrower.KillThyself();
                    //}
                    if (flamethrower.GetComponent<FlamethrowerController>().isActive)
                    {
                        flamethrower.GetComponent<FlamethrowerController>().isActive = false;
                    }
                    ResetPhase3();
                break;
            }

            //Updates phase on all triggers
            if (triggerManager != null) {
                triggerManager.UpdatePhase();
            }
        }
        else {
            //WHEN SWAPPING TO WEAVER
            phases = weaverCurrentPhase;

            //This resets the familiar's phase
            switch (familiarCurrentPhase) {
                case 1:
                    fireballAmount = startingFireballAmount;
                    fireballtimer = startingFireballTimer;
                break;
                case 2:
                    magicCircleAmount = startingMagicCircleAmount;
                    magicCircleTimer = startingMagicCircleTimer;
                    StopCoroutine(Cooldown());
                break;
                case 3:
                    //WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
                    //if (wyvernFlamethrower != null) {
                    //    wyvernFlamethrower.KillThyself();
                    //}
                    if (flamethrower.GetComponent<FlamethrowerController>().isActive)
                    {
                        flamethrower.GetComponent<FlamethrowerController>().isActive = false;
                    }
                    ResetPhase3();
                break;
            }

            //Updates phase on all triggers
            if (triggerManager != null) {
                triggerManager.UpdatePhase();
            }
            
        }
    }

    //When the stag launches from the cannon and strikes the wyvern, it will change rooms
    public void HurtWyvern() {
        switch (phases) {
            case 1:
                fireballAmount = startingFireballAmount;
                fireballtimer = startingFireballTimer;
            break;
            case 2:
                magicCircleAmount = startingMagicCircleAmount;
                magicCircleTimer = startingMagicCircleTimer;
                StopCoroutine(Cooldown());
            break;
            case 3:
                //WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
                //if (wyvernFlamethrower != null) {
                //    wyvernFlamethrower.KillThyself();
                //}
                if (flamethrower.GetComponent<FlamethrowerController>().isActive)
                {
                    flamethrower.GetComponent<FlamethrowerController>().isActive = false;
                }
                ResetPhase3();
            break;
        }
        ActivateOnHurt(); 
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, wyvernHurtSound, 1f);
        moveToNextRoom = true;
        Debug.Log("Wyvern is hurt! Ouch!");
    }

    /// <summary>
    /// Hurts the wyvern only if the invoke on the current room's index does not equal null.
    /// </summary>
    /// <param name="room"></param>
    void ActivateOnHurt() {
        if (onHurtWyvern[currentRoom] != null) {
            onHurtWyvern[currentRoom].Invoke();
        }
    }

    /// <summary>
    /// Changes the phases around on a Stag Swap. Weaver phase becomes familiar's phase, and familiar's phase becomes weaver's phase.
    /// </summary>
    public void StagSwapPhaseSwap(int previousphase) {
        //Familiar's phase is set to the weaver's current phase
        familiarCurrentPhase = weaverCurrentPhase;
        phases = weaverCurrentPhase;

        if (weaverCurrentPhase == 2) {
            if (familiarcontroller.isGrounded) {
                familiarStartingY = stag.transform.position.y;
            }
            else {
                waitUntilFamiliarIsGrounded = true;
            }
        }
        
        //Weavers current phase is set to the previous familiar phase
        weaverCurrentPhase = previousphase;
        
        switch (previousphase) {
            case 1:
                fireballAmount = startingFireballAmount;
                fireballtimer = startingFireballTimer;
            break;
            case 2:
                magicCircleAmount = startingMagicCircleAmount;
                magicCircleTimer = startingMagicCircleTimer;
                StopCoroutine(Cooldown());
            break;
            case 3:
                //WyvernFlamethrower wyvernFlamethrower = GetComponentInChildren<WyvernFlamethrower>();
                //if (wyvernFlamethrower != null) {
                //    wyvernFlamethrower.KillThyself();
                //}
                if (flamethrower.GetComponent<FlamethrowerController>().isActive)
                {
                    flamethrower.GetComponent<FlamethrowerController>().isActive = false;
                }
                ResetPhase3();
            break;
        }

        //Updates phase on all triggers
        if (triggerManager != null) {
            triggerManager.UpdatePhase();
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerControllerNew player = other.gameObject.GetComponent<PlayerControllerNew>();
            if (player != null) {
                player.Death();
            }
        }
    }

    IEnumerator WaitForVFX()
    {
        yield return new WaitForSeconds(3);
        familiarScript.transform.Find("StagShieldParent").gameObject.SetActive(false); // sets shield off for stag
    }
}