using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions.Must;

public class FloatingIslandScript : MonoBehaviour
{
   
    //Attachments
    [Header("Prereqs")]
    //public CinemachineVirtualCamera vcam1; //Player Camera
    //public CinemachineVirtualCamera vcam2; //Floating Island Falling
    public CrystalScript myCrystal;
    public CinemachineVirtualCamera myFloatCamera;

    //Designer Variables
    [Header("Designer Variables")] 
    [SerializeField] private bool isFloating = true;
    //public bool toggleTimer;
    [SerializeField] private float timerBeforeSwap;
    [SerializeField] private Transform floatTransform;
    [SerializeField] private Transform sitTransform;


    [Header("Animation Variables")]
    public float verticalOffset = 0;
    [SerializeField] private float RaiseSpeed = 1;

    //Internal Reqs
    public bool cameraswitched;
    public bool isislandfalling;
    public Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        //Assign Components
        anim = GetComponent<Animator>();
        myCrystal.AssignFloatingIsland(this);

        //Set Variables
        cameraswitched = false;
        isislandfalling = false;

        //Set Animator
        if (isFloating)
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
       if (verticalOffset != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + verticalOffset, transform.position.z);
        }
    }
    
    public void StartFalling() 
    {
        anim.SetTrigger("Fall");
        StartCoroutine(TimerBeforeRespawn(true));
        cameraswitched = false;
       
    }

    public IEnumerator TimerBeforeRespawn(bool isFalling)
    {
        yield return new WaitForSeconds(timerBeforeSwap / 2);

        if (!isFalling)
        {
            CameraMasterScript cameraMasterScript = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
            cameraMasterScript.FloatingIslandCameraReturn(myFloatCamera);
        }
        

        yield return new WaitForSeconds(timerBeforeSwap /2);

        if (isFalling)
        {
            anim.SetTrigger("Sit");
            verticalOffset = 0;
            transform.position = sitTransform.position;
            
        }
        else
        {
            anim.SetTrigger("Float");
            verticalOffset = 0;
            transform.position = floatTransform.position;
            
        }
        
        yield break;
    }

    public void SwapToRiseCamera()
    {
        //Camera is switched to a new view which watches the whole island rise. (Lasts about 2 seconds)
        if (cameraswitched == false)
        {
            CameraMasterScript cameraMasterScript = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
            cameraMasterScript.FloatingIslandCameraSwitch(myFloatCamera, this);
        }

    }

    public void RaiseIsland()
    {
        StartCoroutine(TimerBeforeRespawn(false));
        cameraswitched = false;

        anim.SetTrigger("Rise");
    }


            
}
