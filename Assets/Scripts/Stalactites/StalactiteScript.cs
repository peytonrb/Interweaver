using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StalactiteScript : MonoBehaviour, ITriggerable
{
    private Rigidbody rb;
    private bool isFalling;
    public GameObject stalactiteFallingParticle;
    public GameObject stalactiteCrashingParticle;
    private StalactiteSpawnerScript sss;
    [SerializeField] private float timer;
    [SerializeField] private float cooldown;
    private bool timerOn;
    private BoxCollider bc;
    private AudioSource audioSource;
    public AudioClip fallingAudio;
    public AudioClip crashAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sss = GetComponentInParent<StalactiteSpawnerScript>();
        bc = GetComponent<BoxCollider>();

        bc.size = new Vector3(1,50,1);
        bc.center = new Vector3(0,-25,0);
        bc.isTrigger = true;
        bc.enabled = false;
        isFalling = false;

        StartCoroutine(RegrowthCooldown());
    }

    public void Warning() {
        Instantiate(stalactiteFallingParticle,transform.position,Quaternion.identity);
        bc.size = new Vector3(1,1,1);
        bc.center = Vector3.zero;
        bc.isTrigger = false;
    }

    public void Fall() {
        rb.constraints = RigidbodyConstraints.None;
        isFalling = true;
    }

    void OnCollisionEnter(Collision collision) {
        if (isFalling == true) {
            if (collision.gameObject.CompareTag("Player")) {
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                pc.Death();
                sss.SpawnStalactite();
                Instantiate(stalactiteCrashingParticle,transform.position,Quaternion.identity);
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Familiar")) {
                FamiliarScript fs = collision.gameObject.GetComponent<FamiliarScript>();
                fs.Death();
                sss.SpawnStalactite();
                Instantiate(stalactiteCrashingParticle,transform.position,Quaternion.identity);
                Destroy(gameObject);
            }
            else {
                sss.SpawnStalactite();
                Instantiate(stalactiteCrashingParticle,transform.position,Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    void Update() {
        if (timerOn) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                Fall();
            }
        }
    }

    public void OnTrigEnter(Collider other) {
        if (other.gameObject.CompareTag("Stalactite")) {
            if (!timerOn) {
                Warning();
            }
            timerOn = true;
        }
    }
    
    public void OnTrigExit(Collider other) {
        
    }

    IEnumerator RegrowthCooldown() {
        yield return new WaitForSeconds(cooldown);
        if (sss.constantFalling) {
            Warning();
            timerOn = true;
        }
        else if (sss.constantFalling == false) {
            timerOn = false;
        }
        bc.enabled = true;
    }
}
