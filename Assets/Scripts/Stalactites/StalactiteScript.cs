using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteScript : MonoBehaviour
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
                MoleDigScript mds = collision.gameObject.GetComponent<MoleDigScript>();
                if (mds.borrowed == false) {
                    fs.Death();
                }
                sss.SpawnStalactite();
                Instantiate(stalactiteCrashingParticle,transform.position,Quaternion.identity);
                Destroy(gameObject);
            }
            else {
                //Debug.Log(collision.gameObject.name);
                sss.SpawnStalactite();
                Instantiate(stalactiteCrashingParticle,transform.position,Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    void Update() {
        if (timerOn && sss.canFall) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                Fall();
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar")) {
            if (sss.canFall) {
                if (!timerOn) {
                    Warning();
                }
                timerOn = true;
            }
        }
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
        yield break;
    }
}
