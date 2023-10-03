using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions.Must;

public class FloatingIslandScript : MonoBehaviour
{
    public bool cameraswitched;
    public bool isislandfalling;
    private Rigidbody rb;
    public CinemachineVirtualCamera vcam1; //Player Camera
    public CinemachineVirtualCamera vcam2; //Floating Island Falling
    public GameObject familiar;
    private FamiliarScript familiarScript;


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
       }
    }
    
    public void StartFalling() {
        if (cameraswitched == false) {
            SwitchCamera();
        } else {
            rb.constraints = RigidbodyConstraints.None;
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
        isislandfalling = false;
    }
            
}
