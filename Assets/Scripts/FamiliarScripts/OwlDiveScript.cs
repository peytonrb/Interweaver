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

    public AudioClip diveFile;

    [Header("Variables")]
    [SerializeField][Range(-40, -3)]private float diveAcceleration = -20f;
    [SerializeField][Range(-50, -25)]private float terminalVelocity = -30f;
    [SerializeField][Range(0f, 20f)]private float aerialAcceleration = 4f;
    [SerializeField][Range(0f, 30f)]private float aerialDeceleration = 2f;
    [HideInInspector] public bool isDiving;
    private bool onCooldown = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        //inputs = movementScript.inputs; // this feels wrong, find better way to reference later
    }

    // Update is called once per frame
    void Update()
    {
        if (isDiving && characterController.isGrounded) // if on the ground and isDiving is true, make sure to end dive
        {
            EndDive();
        }
    }

    public void DivePressed()
    {
        if (!characterController.isGrounded) {
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, diveFile, 1f);
            movementScript.ChangeGravity(diveAcceleration);
            movementScript.ChangeTerminalVelocity(terminalVelocity);
            movementScript.ChangeAerialAcceleration(aerialAcceleration);
            movementScript.ChangeAerialDeceleration(aerialDeceleration);
            isDiving = true;
        }
        
    }

    public void DiveRelease()
    {
        if (isDiving)
        {
            EndDive();
        } 
    }

    public void startDiveCooldown(float duration)
    {
        EndDive();
        StartCoroutine(diveCooldown(duration));
    }

    public IEnumerator diveCooldown(float duration)
    {
        
        onCooldown = true;
        
        movementScript.ChangeGravity(200);
        
        yield return new WaitForSeconds(duration);

        movementScript.ChangeGravity(-50);

        yield return new WaitForSeconds(.3f);

        movementScript.ResetGravity();

        yield return new WaitForSeconds(2f);
        onCooldown = false;
        yield break;
    }

    private void EndDive() // returns values to their original forms
    {
        isDiving = false;
        AudioManager.instance.StopSound(AudioManagerChannels.SoundEffectChannel);
        movementScript.ResetGravity();
        movementScript.ResetTerminalVelocity();
        movementScript.ResetAerialAcceleration();
        movementScript.ResetAerialDeceleration();
    }
}
