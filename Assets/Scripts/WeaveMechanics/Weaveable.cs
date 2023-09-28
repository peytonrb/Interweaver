using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Weaveable : MonoBehaviour, IInteractable

{

    [Header("Weaveable's variables")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float HoveringValue;
    [SerializeField] private float WeaveSpeed = 12;
    private bool Startfloating;
    private bool rellocate;
    public InputAction WeaveMove;
    public Vector2 weave;
    public Transform objectposition;
    private Vector3 direction;

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
        if (rellocate) 
        {
            MovingWeave();
        }
       
    }

    void MovingWeave()
    {
        weave = WeaveMove.ReadValue<Vector2>();
        //direction = new Vector3( transform.position.x + weave.x, transform.position.y, transform.position.y + weave.y);
        //transform.position = direction * WeaveSpeed * Time.deltaTime;
        Debug.Log("this is the X" + weave.x);
        Debug.Log("this is theY" + weave.y);
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
