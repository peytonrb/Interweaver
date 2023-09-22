using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    public float speed;
    private CharacterController characterController;
    public InputActionAsset inputs;
    private InputAction moveInput;
    private Vector2 movement;
    private Vector3 direction;
    private Vector3 velocity;
    private Transform playerTransform;
    private float gravity;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerTransform = GetComponent<Transform>();

        gravity = -3f;

        //Section reserved for initiating inputs 
        moveInput = inputs.FindAction("Player/Move");
    }

    void OnEnable() {
        inputs.Enable();
    }

    void OnDisable() {
        inputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
       movement = moveInput.ReadValue<Vector2>();
       //Debug.Log(movement);
       LookAndMove();

    }

    void FixedUpdate() {
        if (direction.magnitude >= 0.1f) {
            characterController.Move(direction.normalized * speed * Time.deltaTime);
        }

        if (!characterController.isGrounded) {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity * Time.deltaTime);
            
    }

    private void LookAndMove() {
        direction = new Vector3(-movement.x,0,-movement.y).normalized;
        
        if (direction.magnitude >= 0.1f) {
            playerTransform.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        
    }

}
