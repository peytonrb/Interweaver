using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WyvernFireball : MonoBehaviour
{
    private GameObject weaver;
    private GameObject wyvern;
    private WyvernBossManager wyvernBossManager;
    private GameObject phaseTriggerManager;
    private WyvernPhaseTriggerManager wptm;
    private Rigidbody rb;
    private PlayerControllerNew playercontroller;
    private WeaveController weaveController;
    private GameObject familiar;
    private FamiliarScript familiarscript;
    private WeaveableObject weaveable;
    public float speed;
    private bool wasWoven;
    private bool breakableObjectFound;
    private Vector3 breakableObjectPosition;
    private Vector3 weaverposition;
    private Vector3 familiarposition;
    private bool whosturn;

    [Header("VFX")]
    public ParticleSystem impactPS;
    private bool isDoomed = false;
    private Vector3 isDoomedPosition;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballSound;
    private AudioSource fireballSource;

    // Start is called before the first frame update
    void Start()
    {
        fireballSource = null;
        weaver = GameObject.FindGameObjectWithTag("Player");
        familiar = GameObject.FindGameObjectWithTag("Familiar");
        wyvern = GameObject.FindGameObjectWithTag("Boss");
        fireballSource = AudioManager.instance.AddSFX(fireballSound, false, fireballSource);
        wyvernBossManager = wyvern.GetComponent<WyvernBossManager>();
        phaseTriggerManager = wyvernBossManager.wyvernTriggerManager;
        wptm = phaseTriggerManager.GetComponent<WyvernPhaseTriggerManager>();
        familiarscript = familiar.GetComponent<FamiliarScript>();
        playercontroller = weaver.GetComponent<PlayerControllerNew>();
        weaveController = weaver.GetComponent<WeaveController>();
        weaveable = GetComponent<WeaveableObject>();
        rb = GetComponent<Rigidbody>();

        weaverposition = new Vector3(weaver.transform.position.x, weaver.transform.position.y + 1, weaver.transform.position.z);
        familiarposition = new Vector3(familiar.transform.position.x, familiar.transform.position.y + 1, familiar.transform.position.z);

        whosturn = familiarscript.myTurn; //If true, its familiar's turn

        wasWoven = false;
        breakableObjectFound = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (familiarscript.myTurn)
        {
            HomingMissile();
            if (transform.position.magnitude > 1000)
            {
                isDoomed = true;
                isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
                Destroy(gameObject);
            }
        }
        else
        {
            if (weaveable.isBeingWoven == false)
            {
                HomingMissile();
                if (transform.position.magnitude > 1000)
                {
                    isDoomed = true;
                    isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
                    Destroy(gameObject);
                }

            }
            else
            {
                wasWoven = true;
            }
        }

        // dont ask
        if (isDoomed)
        {
            ParticleSystem ps = Instantiate(impactPS, isDoomedPosition, Quaternion.identity);
            ps.Play();
            isDoomed = false;
        }
    }

    //Function that moves the fireball towards the player/familiar
    void HomingMissile()
    {
        //If familiar's turn, then move towards the familiar.
        if (whosturn == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, familiarposition, speed * Time.deltaTime);
            if (transform.position == familiarposition)
            {
                isDoomed = true;
                isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
                Destroy(gameObject);
            }
        }
        //Move towards Weaver
        else
        {
            if (wasWoven == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, weaverposition, speed * Time.deltaTime);
                if (transform.position == weaverposition)
                {
                    isDoomed = true;
                    isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
                    Destroy(gameObject);
                }
            }
            else
            {
                if (weaveController.isWeaving == false)
                {
                    if (breakableObjectFound == false)
                    {
                        DetectBreakableObject();
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, breakableObjectPosition, speed * Time.deltaTime);
                    }
                }
            }
        }
    }

    //This detects if an object has a BreakObject script attached to it.
    void DetectBreakableObject()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider colliderFound in hitCollider)
        {
            if (colliderFound.gameObject.TryGetComponent<BreakObject>(out BreakObject breakObject))
            {
                breakableObjectPosition = breakObject.gameObject.transform.position;
                breakableObjectFound = true;
                Debug.Log("Object found");
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Vector3 pos;

        if (!other.gameObject.CompareTag("Boss"))
        {
            if (other.gameObject.TryGetComponent<BreakObject>(out BreakObject breakableObject))
            {
                breakableObject.BreakMyObject();
            }

            isDoomed = true;
            isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            playercontroller.Death();
            wptm.UpdateTriggersOnDeath(true);
            isDoomed = true;
            isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Familiar"))
        {
            familiarscript.Death();
            wptm.UpdateTriggersOnDeath(false);
            isDoomed = true;
            isDoomedPosition = new Vector3(this.transform.position.x, this.transform.position.y - 1f, this.transform.position.z);
            Destroy(gameObject);
        }
    }
}
