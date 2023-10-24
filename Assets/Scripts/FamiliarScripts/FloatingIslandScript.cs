using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions.Must;

public class FloatingIslandScript : MonoBehaviour
{
   
    //Attachments
    [Header("Prereqs")]
    public CinemachineVirtualCamera vcam1; //Player Camera
    public CinemachineVirtualCamera vcam2; //Floating Island Falling
    public GameObject familiar;
    public CrystalScript myCrystal;

    private FamiliarScript familiarScript;

    //Designer Variables
    [Header("Designer Variables")] 
    [SerializeField] private bool startsFloating = true;
    public bool toggleTimer;
    [SerializeField] private float timerBeforeSwap;
    [SerializeField] private Transform floatTransform;
    [SerializeField] private Transform sitTransform;


    [Header("Animation Variables")]
    public float verticalOffset = 0;

    //Internal Reqs
    private bool cameraswitched;
    public bool isislandfalling;
    private Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        //Assign Components
        rb = gameObject.GetComponent<Rigidbody>();
        familiarScript = familiar.GetComponent<FamiliarScript>();
        anim = GetComponent<Animator>();
        myCrystal.AssignFloatingIsland(this);

        //Set Variables
        cameraswitched = false;
        isislandfalling = false;

        //Set Animator
        if (startsFloating)
        {
            anim.SetTrigger("Float");
        }
        else
        {
            anim.SetTrigger("Sit");
        }
    }

    // Update is called once per frame
    void Update()
    {
       //if (isislandfalling == false) {
       //     familiarScript.islandisfalling = false;
            
       //} else {
       //     familiarScript.islandisfalling = true;
       //     if (toggleTimer) {
       //         timer -= Time.deltaTime;
       //         if (timer <= 0) {
       //             ReturnCamera();
       //         }
       //     }
       //}

       if (verticalOffset != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + verticalOffset, transform.position.z);
        }
    }
    
    public void StartFalling() 
    {
        anim.SetTrigger("Fall");

        StartCoroutine(TimerBeforeRespawn(true));
    }

    public IEnumerator TimerBeforeRespawn(bool isFalling)
    {
        yield return new WaitForSeconds(timerBeforeSwap);

        if (isFalling)
        {
            transform.position = sitTransform.position;
            anim.SetTrigger("Sit");
        }
        else
        {
            transform.position = floatTransform.position;
            anim.SetTrigger("Float");
        }
        
        yield break;
    }

    public void StartRising()
    {
        //Camera is switched to a new view which watches the whole island rise. (Lasts about 2 seconds)
        if (cameraswitched == false)
        {
            SwitchCamera();
        }
        else
        {
            //Island starts to fall once the camera has been switched
            //Needs to be changed to play rise animation instead
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            cameraswitched = false;
        }
    }

    void SwitchCamera() {
        vcam1.Priority = 0;
        vcam2.Priority = 1;
        cameraswitched = true;
        StartFalling();
    }

    public void ReturnCamera() {
        vcam1.Priority = 1;
        vcam2.Priority = 0;
        if (toggleTimer == false) {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        isislandfalling = false;
    }
            
}
