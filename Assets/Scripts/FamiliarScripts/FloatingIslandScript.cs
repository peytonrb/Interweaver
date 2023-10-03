using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIslandScript : MonoBehaviour
{
    
    public bool isfalling;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        isfalling = false;

        rb = gameObject.GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartFalling() {
        rb.constraints = RigidbodyConstraints.None; 
    }
            
}
