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
     private float timer;
    [SerializeField] private float cooldown;
    public float currentTimer;
    public bool timerOn;   
    private AudioSource audioSource;
    public AudioClip fallingAudio;
    public AudioClip crashAudio;
    private bool hasInvoked;

    void Start()
    {
        timerOn = false;
        rb = GetComponent<Rigidbody>();
        sss = GetComponentInParent<StalactiteSpawnerScript>();
        timer = currentTimer;
        Warning();
    }

    public void Warning() 
    {
        Instantiate(stalactiteFallingParticle,transform.position,Quaternion.identity);
        rb.velocity = Vector3.zero;
    }

    public void Fall() 
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        isFalling = true;
    }

    void OnCollisionEnter(Collision collision) 
    {
        GameObject stalactiteCrashVFX = Instantiate(stalactiteCrashingParticle, transform.position, Quaternion.identity);
        if (isFalling)
        {
            if (collision.gameObject.CompareTag("Player")) 
            {
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                pc.Death();
                
                Destroy(stalactiteCrashVFX.gameObject, 1f);
           
                gameObject.SetActive(false);                
                timer = currentTimer;
                hasInvoked = false;
                timerOn = false;
                rb.velocity = Vector3.zero;
            }
            else if (collision.gameObject.CompareTag("Familiar")) 
            {
                FamiliarScript fs = collision.gameObject.GetComponent<FamiliarScript>();
                MoleDigScript mds = collision.gameObject.GetComponent<MoleDigScript>();
                if (!mds.borrowed) 
                {
                    fs.Death();
                }
                Destroy(stalactiteCrashVFX.gameObject, 1f);
                
                gameObject.SetActive(false);                
                timer = currentTimer;
                hasInvoked = false;
                timerOn = false;
                rb.velocity = Vector3.zero;
            }
            else 
            {
                //Debug.Log(collision.gameObject.name);
                Destroy(stalactiteCrashVFX.gameObject, 1f);
               
                gameObject.SetActive(false);                
                timer = currentTimer;
                hasInvoked = false;
                timerOn = false;
                rb.velocity = Vector3.zero;
            }
        }
    }

    void Update() 
    {
        if (!hasInvoked) 
        {
            StartCoroutine(RegrowthCooldown());
           
        }
        
        if (timerOn && hasInvoked) 
        {           
            timer -= Time.deltaTime;
            if (timer <= 0) 
            {
                Fall();                
            }
        }
    }
   

    IEnumerator RegrowthCooldown() 
    {
        Warning();
        rb.constraints = RigidbodyConstraints.FreezePosition;
        hasInvoked = true;
        yield return new WaitForSeconds(cooldown);

        if (sss.canFall && !timerOn)
        {            
            timerOn = true;            
            Debug.Log("I want to kil lmyself");
        }
       
        yield break;
    }
}
