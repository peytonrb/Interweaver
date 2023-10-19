using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions.Must;

public class FloatingIslandScript : MonoBehaviour
{
    private bool cameraswitched;
    public bool isislandfalling;
    private Rigidbody rb;
    public CinemachineVirtualCamera vcam1; //Player Camera
    public CinemachineVirtualCamera vcam2; //Floating Island Falling
    public GameObject familiar;
    private FamiliarScript familiarScript;
    public bool toggleTimer;
    public float timer;


    // Start is called before the first frame update
    void Start()
    {
        cameraswitched = false;
        isislandfalling = false;

        rb = gameObject.GetComponent<Rigidbody>();
        familiarScript = familiar.GetComponent<FamiliarScript>();  
    }

    // Update is called once per frame
    void Update()
    {
       if (isislandfalling == false) {
            familiarScript.islandisfalling = false;
            
       } else {
            familiarScript.islandisfalling = true;
            if (toggleTimer) {
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    ReturnCamera();
                }
            }
       }
    }
    
    public void StartFalling() {
        //Camera is switched to a new view which watches the whole island fall from the sky. (Lasts about 2 seconds)
        if (cameraswitched == false) {
            SwitchCamera();
        } else {
            //Island starts to fall once the camera has been switched
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
