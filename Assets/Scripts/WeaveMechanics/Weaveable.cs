using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Weaveable : MonoBehaviour, IInteractable

{

    [Header("Weaveable's variables")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float HoveringValue;
    private bool Startfloating;
    private bool rellocate;
    public InputAction WeaveMove;
    public Vector2 weave;
    public Transform objectposition;

    void start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        WeaveMove.Enable();        
    }

    void OnDisable()
    {
        WeaveMove.Disable();       
    }

    void Update()
    {
        if (Startfloating) 
        {
           transform.position = transform.position + new Vector3 (0, HoveringValue*Time.deltaTime, 0);
            Startfloating = false;
        }     
    }

    void MovingWeave()
    {
        weave = WeaveMove.ReadValue<Vector2>();
    }

    public void Interact()
    {
        Debug.Log("This is interactable");
        rigidbody.useGravity = false;
        Startfloating = true;
       rellocate = true;
    }
    
    public void Uninteract()
    {
        Debug.Log("this is now not woven");
        rigidbody.useGravity = true;
        rellocate = false;
    }



}
