using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaveable : MonoBehaviour, IInteractable

{

    [Header("Weaveable's variables")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float HoveringValue;
    private bool Startfloating;
    

    void start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Startfloating) 
        {
           transform.position = transform.position + new Vector3 (0, HoveringValue*Time.deltaTime, 0);
            Startfloating = false;
        }     
    }

    public void Interact()
    {
        Debug.Log("This is interactable");
        rigidbody.useGravity = false;
        Startfloating = true;
       
    }
    
    public void Uninteract()
    {
        Debug.Log("this is now not woven");
        rigidbody.useGravity = true;
        
    }
}
