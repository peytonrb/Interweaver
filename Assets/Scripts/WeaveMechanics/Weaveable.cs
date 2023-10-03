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
    [SerializeField] private Vector3 raycastPosition;
    [SerializeField] private float WeaveDistance = 12f;
    private Vector3 WeaveablePos;
    private bool Startfloating; //a bool to detect if the weaveable is interacted and will start floating
    private bool relocate; // bool for relocate
    private bool Weave; //bool for weaving the weaveables
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
        MovingWeaveMouse();


        if (Startfloating) 
        {
           transform.position = transform.position + new Vector3 (0, HoveringValue*Time.deltaTime, 0);
            Startfloating = false;          
        } 
        

        if (relocate) 
        {
          UninteractDistance();
         rigidbody.freezeRotation = true;           
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
            if (relocate)
            {
                rigidbody.velocity = new Vector3(raycastHit.point.x - rigidbody.position.x, transform.position.y - rigidbody.position.y, raycastHit.point.z - rigidbody.position.z);
            }
            if (Weave)
            {
                transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y , raycastHit.point.z));
                WeaveWeaveables();
            }
        }
    }



    void MovingWeave() //method for moving the weaveable object
    {
        weave = WeaveMove.ReadValue<Vector2>();
        //direction = new Vector3( transform.position.x + weave.x, transform.position.y, transform.position.y + weave.y);
        //transform.position = direction * WeaveSpeed * Time.deltaTime;
        Debug.Log("this is the X" + weave.x);
        Debug.Log("this is theY" + weave.y);
    }

    void WeaveWeaveables() //method for weaving the weaveables
    {
        WeaveablePos = new Vector3(transform.position.x, transform.position.y + raycastPosition.y, transform.position.z); //this is the raycast origin 
        Vector3 rayDirection = transform.forward;
        Ray ray = new Ray(WeaveablePos, rayDirection); //the actual  raycast
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * WeaveDistance, Color.red); //debug  for  when the game  starts and the line can be  seen on  scene
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
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        relocate = false;
        Weave = false;
    }

    public void Relocate()
    {
        relocate = true;
        Weave = false;
        rigidbody.constraints = RigidbodyConstraints.None;
        Debug.Log("Relocate Mode");
    }

    public void WeaveMode()
    {
        relocate = false;
        Weave = true;
        rigidbody.constraints = RigidbodyConstraints.FreezePosition; 
        Debug.Log("weave Mode");
    }
}
