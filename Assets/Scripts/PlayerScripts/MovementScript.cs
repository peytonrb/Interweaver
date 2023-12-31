using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MovementScript : MonoBehaviour 
{
    private GameMasterScript GM; //This is refrencing the game master script

    [Header("Movement Variables")]
    public bool canMove = true;
    public float speed; //Base walk speed for player
    private float currentSpeed = 0; // the current speed for the player
    private CharacterController characterController; //references the character controller component
    //public InputActionAsset inputs; //In inspector, make sure playerInputs is put in this field
    //private InputAction moveInput; //Is the specific input action regarding arrow keys, WASD, and left stick
    private Vector2 movement; //Vector2 regarding movement, which is set to track from moveInput's Vector2
    private Vector3 direction; //A reference to the directional movement of the player in 3D space
    private Vector3 velocity; // velocity of the controller
    [SerializeField] private float gravity = -3f; //Gravity of the controller
    private float originalGravity; // Original gravity of the controller
    public bool aerialControl; //Bool which determines if controller can move in air 
    private float acceleration;
    private float deceleration;
    [SerializeField][Range(1f, 20f)] private float groundAcceleration; // ground acceleration of the controller
    private float originalGroundAcceleration;
    [SerializeField][Range(1f, 30f)] private float groundDeceleration; // ground deceleration of the controller
    private float originalGroundDeceleration;
    [SerializeField][Range(0f, 20f)] private float aerialAcceleration; // aerial horizontal acceleration of the controller
    private float originalAerialAcceleration;
    [SerializeField][Range(0f, 30f)] private float aerialDeceleration; // aerial horizontal deceleration of the controller
    private float originalAerialDeceleration;
    [SerializeField][Range(-50f, -5f)] private float terminalVelocity; // the terminal velocity of the controller
    private bool resettingTerminalVelocity;
    private float originalTerminalVelocity; // original terminal velocity of the controller
    public Vector3 bounceVector; //max velocity for bounce
    public float bounceValue = 3;
    private bool isNearGround;

    //private bool bouncing = false;

    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;


    [Header("character's camera")]
    //Character Rotation values
    private float rotationSpeed;
    private float rotationVelocity;
    [HideInInspector] public Vector3 newDirection;
    public GameObject cam; //Camera object reference
    public CinemachineVirtualCamera virtualCam; //Virtual Camera reference
    public bool active; //Determines if movement controller is active

    [Header("Character's sounds")]
    [SerializeField] private AudioClip footStepsClip;
    [SerializeField] private AudioClip weaverFallClip;
    [SerializeField] private AudioClip owlGlideClip;
    [SerializeField] private AudioClip owlDiveClip;
    private bool canPlayFallAudio = false;

    [Header("Dive VFX")]
    private ParticleSystem speedLinesVFX;

    [HideInInspector] public bool inCutscene;
    private PlayerController playerController;
    private FamiliarScript familiarScript;
    

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        TryGetComponent<ParticleSystem>(out ParticleSystem vfx);

        speedLinesVFX = vfx;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        familiarScript = GameObject.FindGameObjectWithTag("Familiar").GetComponent<FamiliarScript>();

        rotationSpeed = 0.1f;
        originalGroundAcceleration = groundAcceleration;
        originalGroundDeceleration = groundDeceleration;
        originalGravity = gravity; // get original gravity of controller
        originalTerminalVelocity = terminalVelocity; // get original terminal velocity of controller
        originalAerialAcceleration = aerialAcceleration;
        originalAerialDeceleration = aerialDeceleration;

        //moveInput = inputs.FindAction("Player/Move");

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        //transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        inCutscene = false;

        StartCoroutine(DelayBeforeFallAudio());
    }

    // Update is called once per frame
    void Update()
    {
        if (!inCutscene) {
            //Move character only if they are on the ground
            if (characterController.isGrounded || aerialControl)
            {
                if (Time.timeScale != 0 && active)
                {
                    //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad
                    movement = InputManagerScript.instance.movement;
                    LookAndMove();
                    
                }
            }
        }
        
    }

    public void ToggleCanMove(bool moves)
    {
        if (familiarScript.myTurn) {
            if (moves)
            {
                canMove = true;
                characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);
                familiarScript.talkingToNPC = false;
            }
            else
            {
                canMove = false;
                characterAnimationHandler.ToggleMoveSpeedBlend(0);
                familiarScript.talkingToNPC = true;
            }
        }
        else {
            if (moves)
            {
                canMove = true;
                characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);
                playerController.talkingToNPC = false;
            }
            else
            {
                canMove = false;
                characterAnimationHandler.ToggleMoveSpeedBlend(0);
                playerController.talkingToNPC = true;
            }
        }
        
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (Time.timeScale != 0 && active && !inCutscene)
            {
                // changes what acceleration/deceleration type is being used based on if controller is grouunded or not
                acceleration = characterController.isGrounded ? groundAcceleration : aerialAcceleration;
                deceleration = characterController.isGrounded ? groundDeceleration : aerialDeceleration;

                //Character movement
                if (direction.magnitude >= 0.1f)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, speed, acceleration * Time.deltaTime);
                    //Debug.Log(currentSpeed);
                    //currentSpeed += acceleration * Time.deltaTime;
                    //currentSpeed = Mathf.Clamp(currentSpeed, 0f, speed);
                    characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed); // note: speed is static now, but this should work fine when variable speed is added

                    if (characterController.isGrounded)
                    {
                        //Play footstep Audio!
                        if (TryGetComponent<PlayerController>(out PlayerController playerCon))
                        {
                            if (!AudioManager.instance.footStepsChannel.isPlaying)
                                AudioManager.instance.PlaySound(AudioManagerChannels.footStepsLoopChannel, footStepsClip, 1.3f);
                        }
                        else
                        {
                            if (!AudioManager.instance.footStepsChannel.isPlaying)
                                AudioManager.instance.PlaySound(AudioManagerChannels.footStepsLoopChannel, footStepsClip, 1.7f);
                        }
                    }
                    else
                    {
                        if (AudioManager.instance.footStepsChannel.isPlaying)
                            AudioManager.instance.StopSoundAfterLoop(AudioManagerChannels.footStepsLoopChannel);
                    }

                }
                else
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);
                    characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);
                    if (AudioManager.instance.footStepsChannel.isPlaying)
                        AudioManager.instance.StopSoundAfterLoop(AudioManagerChannels.footStepsLoopChannel);
                    //Debug.Log(currentSpeed);
                    //currentSpeed -= deceleration * Time.deltaTime;
                    //currentSpeed = Mathf.Clamp(currentSpeed, 0f, speed);
                }

                velocity.x = currentSpeed * newDirection.x;
                velocity.z = currentSpeed * newDirection.z;

                characterController.Move(velocity * Time.deltaTime); // make move based on gravity

                //Character gravity
                if (!characterController.isGrounded)
                {
                    characterAnimationHandler.ToggleFallAnim(true);
                    if (TryGetComponent<PlayerController>(out PlayerController playerCon) && !AudioManager.instance.fallChannel.isPlaying && canPlayFallAudio)
                    {
                        AudioManager.instance.PlaySound(AudioManagerChannels.fallLoopChannel, weaverFallClip);
                        Debug.Log("weaver fall");
                    }
                    else
                    {
                        if (TryGetComponent<OwlDiveScript>(out OwlDiveScript diveScript) && diveScript.isDiving && canPlayFallAudio)
                        {
                            if (!AudioManager.instance.fallChannel.isPlaying || AudioManager.instance.fallChannel.clip != owlDiveClip && canPlayFallAudio)
                            {
                                Debug.Log("Play Dive audio");
                                AudioManager.instance.PlaySound(AudioManagerChannels.fallLoopChannel, owlDiveClip);
                            }
                        }
                        else if (canPlayFallAudio)
                        {
                            if (!AudioManager.instance.fallChannel.isPlaying || AudioManager.instance.fallChannel.clip != owlGlideClip)
                            {
                                AudioManager.instance.PlaySound(AudioManagerChannels.fallLoopChannel, owlGlideClip);
                                Debug.Log("owl glide");
                            }
                        }
                    }

                    velocity.y += gravity * Time.deltaTime;
                }
                else
                {
                    characterAnimationHandler.ToggleFallAnim(false);
                    AudioManager.instance.StopSound(AudioManagerChannels.fallLoopChannel);
                    velocity.y = -2f;
                }

                velocity.y = Mathf.Clamp(velocity.y, terminalVelocity, 200f);

                if (speedLinesVFX != null)
                {

                    var em = speedLinesVFX.emission;
                    if (velocity.y < -15)
                    {
                        em.rateOverTime = Mathf.Abs(velocity.y) * 1.2f;
                    }
                    else
                    {
                        em.rateOverTime = 0;
                    }

                }

            }

            if (resettingTerminalVelocity)
            {
                ResetTerminalVelocity();
            }

            if (inCutscene) {
                AudioManager.instance.StopSound(AudioManagerChannels.fallLoopChannel);
                AudioManager.instance.StopSound(AudioManagerChannels.footStepsLoopChannel);
            }
        }
        
    }

    public void LookAndMove()
    {
        direction = new Vector3(movement.x, 0, movement.y).normalized; //direction of movement

        //Character rotations
        if (direction.magnitude >= 0.2f)
        {
            float targetangle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref rotationVelocity, rotationSpeed);                     
            transform.rotation = Quaternion.Euler(0, angle, 0);                     
            newDirection = Quaternion.Euler(0, targetangle, 0) * Vector3.forward;
        }
    }

    // CORE VARIABLE MODS--------------------------------------------------------

    public void HardResetMovementStats()
    {
        //velocity
        velocity = Vector3.zero;
        //ground
        groundAcceleration = originalGroundAcceleration;
        groundDeceleration = originalGroundDeceleration;
        // aerial
        terminalVelocity = originalTerminalVelocity;
        ResetAerialAcceleration();
        ResetAerialDeceleration();
        ResetGravity();
    }

    public void ChangeAerialAcceleration(float newAerialAcceleration)
    {
        aerialAcceleration = newAerialAcceleration;
    }

    public void ResetAerialAcceleration()
    {
        aerialAcceleration = originalAerialAcceleration;
    }

    public void ChangeAerialDeceleration(float newAerialDeceleration)
    {
        aerialDeceleration = newAerialDeceleration;
    }
    
    public void ResetAerialDeceleration()
    {
        aerialDeceleration = originalAerialDeceleration;
    }

    public float GetGravity()
    {
        return gravity;
    }

    public void ChangeGravity(float newGravity) // changes gravity to new value
    {
        gravity = newGravity;
    }

    public void ResetGravity() // resets gravity to original value
    {
        gravity = originalGravity;
    }

    public void ResetVelocityY()
    {
        velocity.y = 0;
    }

    public void ChangeTerminalVelocity(float newTerminalVelocity) // changes terminal velocity to new value
    {
        resettingTerminalVelocity = false;
        terminalVelocity = newTerminalVelocity;
    }

    public void ChangeVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void ResetTerminalVelocity() // resets terminal velocity to original value
    {
        resettingTerminalVelocity = true;
        if (terminalVelocity != originalTerminalVelocity)
        {
            terminalVelocity = Mathf.Lerp(terminalVelocity, originalTerminalVelocity, 0.5f * Time.deltaTime);

            if (characterController.isGrounded)
            {
                terminalVelocity = originalTerminalVelocity;
            }
        }
        else
        {
            resettingTerminalVelocity = false;
        }
    }

    public float GetTerminalVelocity()
    {
        return terminalVelocity;
    }

    // -------------------------------------------------------------------

    /*public void Bounce()
    {
        Debug.Log("bounce called");
        ResetGravity();
        ResetTerminalVelocity();
        ResetAerialAcceleration();
        ResetAerialDeceleration();
        ResetVelocityY();
        GetComponent<OwlDiveScript>().StartDiveCooldown(.1f);
        //characterController.Move(bounceVector);
    }*/

    public void GoToCheckPoint()
    {
        if (TryGetComponent<PlayerController>(out PlayerController playerCon))
        {
            playerCon.Death();
        }

        if (TryGetComponent<FamiliarScript>(out FamiliarScript familiarScript))
        {
            familiarScript.Death();
        }
    }

    public IEnumerator DelayBeforeFallAudio()
    {
        yield return new WaitForSeconds(3);

        canPlayFallAudio = true;
        yield break;
    }
}
