using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Weaveable : MonoBehaviour, IInteractable

{

    [Header("Weaveable's variables")]
    [SerializeField] private Rigidbody rigidbody; //this is grabbing the reffrence from the  rigidbody if we're going to use gravity (this can be deleted but just make sure that when the uninteract activates do the inverse)
    [SerializeField] private float HoveringValue; // thee value for hovering over the floor
    [SerializeField] private float WeaveSpeed = 12; // the value for the weave speed, though not sure if it's needed at the current moment
    [SerializeField] private Camera mainCamera; // grabbing the main camera
    [SerializeField] private LayerMask LayerstoHit; //a layermask
    [SerializeField] private float distance = 12f;
    private bool Startfloating; //a bool to detect if the weaveable is interacted and will start floating
    private bool rellocate; // bool for relocate
    public InputAction WeaveMove; //the input action for the right stick (still don't know the method for that)
    private Vector2 weave;
    public Transform PlayerPrefab;
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
            MovingWeaveMouse();
            UninteractDistance();
        }
       
    }


    void UninteractDistance()
    {
        float distanceBetween = Vector3.Distance(PlayerPrefab.transform.position, transform.position);
        if (distanceBetween > distance) 
        {
            Uninteract();
        }
    }

    void MovingWeaveMouse()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100, LayerstoHit))
        {
            transform.position = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z);
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
