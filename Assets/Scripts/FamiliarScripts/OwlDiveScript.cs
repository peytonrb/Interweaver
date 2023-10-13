using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class OwlDiveScript : MonoBehaviour
{
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component

    private InputAction familiarMovementAbilityInput;// input for movement ability
    private bool familiarMovementAbility;// only used for reading if familiar is using movement ability
    private bool divePressed; // defines the initial press of the dive
    private bool diveHeld; // defines the dive as still being held

    private InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        inputs = movementScript.inputs; // this feels wrong, find better way to reference later
    }

    void OnEnable() 
    {
        inputs.Enable(); 
    }

    void OnDisable() 
    {
        inputs.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        familiarMovementAbilityInput = inputs.FindAction("Player/Familiar Movement Ability");
    }

    // Update is called once per frame
    void Update()
    {
        if (movementScript.active && characterController.isGrounded) // owl should only be able to dive when they are active & in the air
        {

            diveHeld = familiarMovementAbilityInput.IsPressed();

            if (divePressed)
            {
                
            }
        }
    }
}
