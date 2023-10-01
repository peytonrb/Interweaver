using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIslandScript : MonoBehaviour
{
    public int islandIndex; //Which island is falling
    public bool isfalling;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        isfalling = false;

        rb = this.GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
       if (isfalling) {
            StartFalling(islandIndex);
            isfalling = false;
       } 
    }
    
    public void StartFalling(int isindex) {
        if (isindex == this.islandIndex) {
            rb.constraints = RigidbodyConstraints.None;
        }
        
    }
            
}
