using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.IO;

public class OwlDiveScript : MonoBehaviour
{
    [Header("References")]
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    private FamiliarScript familiarScript; // reference for the familiar script component
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    private UpdraftScript updraftScript;
    [Header("Inputs")]
    public bool divePressed; // defines the initial press of the dive

    [Header("Variables")]
    [SerializeField][Range(-40, -3)]private float diveAcceleration = -20f;
    [SerializeField][Range(-50, -25)]private float terminalVelocity = -30f;
    [SerializeField][Range(0f, 20f)]private float aerialAcceleration = 4f;
    [SerializeField][Range(0f, 30f)]private float aerialDeceleration = 2f;
    [HideInInspector] public bool isDiving;
    private bool onCooldown = false;
    public AudioClip islandBreakFile;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        familiarScript = GetComponent<FamiliarScript>();
        updraftScript = GetComponent<UpdraftScript>();
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

            if (!isDiving)
            {
                characterAnimationHandler.ToggleDiveAnim(true);
                movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, movementScript.GetVelocity().y, movementScript.GetVelocity().z));
            }
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
            characterAnimationHandler.ToggleDiveAnim(false);
            EndDive();
        } 
    }

    public void StartDiveCooldown(float duration)
    {
        EndDive();
        StartCoroutine(DiveCooldown(duration));
    }

    public void Bounce()
    {
        Debug.Log("bounce called");
        movementScript.ResetGravity();
        movementScript.ResetTerminalVelocity();
        movementScript.ResetAerialAcceleration();
        movementScript.ResetAerialDeceleration();
        movementScript.ResetVelocityY();
        StartDiveCooldown(.1f);
    }

    public IEnumerator DiveCooldown(float duration)
    {
        characterAnimationHandler.ToggleBounceAnim();

        onCooldown = true;

        yield return new WaitForFixedUpdate();
        
        movementScript.ChangeGravity(200);
        
        yield return new WaitForSecondsRealtime(duration);

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
        characterAnimationHandler.ToggleDiveAnim(false);
        AudioManager.instance.StopSound(AudioManagerChannels.SoundEffectChannel);
        movementScript.ResetGravity();
        movementScript.ResetTerminalVelocity();
        movementScript.ResetAerialAcceleration();
        movementScript.ResetAerialDeceleration();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Breakable")) // if familiar collides with breakable object while using movement ability
        {
            if (isDiving)
            {
                Bounce();
                
                if (collision.gameObject.TryGetComponent<CrystalScript>(out CrystalScript crystal))
                {
                    crystal.TriggerBreak();
                }
                else
                {
                    AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, islandBreakFile, 1f);
                    Destroy(collision.gameObject);
                }
            }
            else
            {
                CameraMasterScript.instance.EndLeapOfFaith();
                familiarScript.Death();
            }
        }
    }
}
