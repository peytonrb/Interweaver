using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; //this is for testing 

public class PlayerScript : MonoBehaviour
{

    public float speed; //Base walk speed for player
    private CharacterController characterController; //references the character controller component
    public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    private InputAction moveInput; //Is the specific input action regarding arrow keys, WASD, and left stick
    private Vector2 movement; //Vector2 regarding movement, which is set to track from moveInput's Vector2
    private Vector3 direction; //A reference to the directional movement of the player in 3D space
    private Vector3 velocity; //Velocity in relation to gravity
    private float gravity; //Gravity of player
    private GameMasterScript GM; //This is refrencing the game master script


    // Start is called before the first frame update
    void Start()
    {
        //references to character components
        characterController = GetComponent<CharacterController>();

        gravity = -3f;

        //Section reserved for initiating inputs 
        moveInput = inputs.FindAction("Player/Move");

        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>(); //these two lines are grabing the game master's last checkpoint position
        transform.position = GM.LastCheckPointPos;
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
        //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad.
        movement = moveInput.ReadValue<Vector2>();
        //Debug.Log(movement);
        LookAndMove();


        if (Input.GetKeyDown(KeyCode.Space)) //this is purely for testing the checkpoint function if it's working properly
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //this is for testing
        }
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
            //playerTransform.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.forward = direction;
        }
        
    }

}
