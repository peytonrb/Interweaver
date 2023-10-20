using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class OwlDiveScript : MonoBehaviour
{
    [Header("References")]
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    [Header("Inputs")]
    public bool divePressed; // defines the initial press of the dive
    public bool diveHeld; // defines the dive as still being held

    [Header("Variables")]
    [SerializeField][Range(-40, -3)]private float diveAcceleration = -20f;
    [SerializeField][Range(-50, -25)]private float terminalVelocity = -30f;
    [SerializeField][Range(0f, 20f)]private float aerialAcceleration = 4f;
    [SerializeField][Range(0f, 30f)]private float aerialDeceleration = 2f;
    private bool isDiving;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        //inputs = movementScript.inputs; // this feels wrong, find better way to reference later
    }

    // Update is called once per frame
    void Update()
    {
        if (movementScript.active && !characterController.isGrounded) // owl should only be able to dive when they are active & in the air
        {
            if (divePressed) // dive starts, assign values
            {
                movementScript.ChangeGravity(diveAcceleration); 
                movementScript.ChangeTerminalVelocity(terminalVelocity);
                movementScript.ChangeAerialAcceleration(aerialAcceleration);
                movementScript.ChangeAerialDeceleration(aerialDeceleration);
                isDiving = true;
            }
            if (isDiving && !diveHeld) // if dive input stops being held, end dive
            {
                EndDive();
            }
        }
        else if (isDiving) // if on the ground and isDiving is true, make sure to end dive
        {
            EndDive();
        }
    }

    private void EndDive() // returns values to their original forms
    {
        isDiving = false;
        movementScript.ResetGravity();
        movementScript.ResetTerminalVelocity();
        movementScript.ResetAerialAcceleration();
        movementScript.ResetAerialDeceleration();
    }
}
