using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class MovementScript : MonoBehaviour
{
    private GameMasterScript GM; //This is refrencing the game master script

    [HideInInspector] public bool isInTutorial; //this is for the UI pop up and it only shows when they're in the tutorial

    [Header("Movement Variables")]
    public bool canLook = true;
    [field: SerializeField] public bool canMove {get;  private set;} = true; 
    public float speed; //Base walk speed for player
    [HideInInspector] public float currentSpeed {get;  private set;} // the current speed for the player
    private bool turning;
    [HideInInspector] public bool freeMove; // bases movement off input rather than direction 
    private CharacterController characterController; //references the character controller component  
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
    [HideInInspector] public bool isNearGround;
    private bool debugisOn;

    //private bool bouncing = false;

    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;


    [Header("character's camera")]
    //Character Rotation values
    [SerializeField] private float timeToTurn = 0.1f;
    private float originalTimeToTurn;
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
    private PlayerControllerNew playerController;
    private FamiliarScript familiarScript;

    [Header("Materials")]
    [SerializeField] private Material dissolveMat;
    [SerializeField] private GameObject materialHolder;
    private Material defaultMat;

    [Header("DeathVFX")]
    [SerializeField] private VisualEffect deathVFX;

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
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerNew>();
        familiarScript = GameObject.FindGameObjectWithTag("Familiar").GetComponent<FamiliarScript>();

        isInTutorial = true; //this is the the funny boolean for being in tutorial
       originalGroundAcceleration = groundAcceleration;
        originalGroundDeceleration = groundDeceleration;
        originalTimeToTurn = timeToTurn;
        originalGravity = gravity; // get original gravity of controller
        originalTerminalVelocity = terminalVelocity; // get original terminal velocity of controller
        originalAerialAcceleration = aerialAcceleration;
        originalAerialDeceleration = aerialDeceleration;

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        //transform.position = GM.LastCheckPointPos;
        characterController.enabled = true;
        inCutscene = false;
        debugisOn = DebugManager.instance.GetDebugOn();

        StartCoroutine(DelayBeforeFallAudio());

        defaultMat = materialHolder.GetComponent<SkinnedMeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inCutscene)
        {
            //Move character only if they are on the ground
            if (characterController.isGrounded || aerialControl)
            {
                if (Time.timeScale != 0 && active)
                {
                    //Looks at the inputs coming from arrow keys, WASD, and left stick on gamepad
                    movement = InputManagerScript.instance.movement;
                    LookAndMove();

                    //Skipping checkpoints only allowed when debug mode is on
                    if (debugisOn) {
                        if (Input.GetKeyDown(KeyCode.LeftBracket)) {
                            GM.GoToPreviousCheckpoint();
                        }

                        if (Input.GetKeyDown(KeyCode.RightBracket)) {
                            GM.GoToNextCheckpoint();
                        }

                        //Go forward one scene
                        if (Input.GetKeyDown(KeyCode.Equals)) {
                            switch (SceneHandler.instance.currentSceneName) {
                                case "AlpineCombined":
                                    SceneHandler.instance.LoadLevel("Cavern");
                                break;
                                case "Cavern":
                                    SceneHandler.instance.LoadLevel("Menu");
                                break;
                            }
                        }

                        //Go backwards one scene
                        if (Input.GetKeyDown(KeyCode.Minus)) {
                            switch (SceneHandler.instance.currentSceneName) {
                                case "AlpineCombined":
                                    SceneHandler.instance.LoadLevel("Menu");
                                break;
                                case "Cavern":
                                    SceneHandler.instance.LoadLevel("AlpineCombined");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void NoLongerInTutorial()
    {
        isInTutorial = false;
    }

    public void ToggleCanMove(bool moves)
    {
        if (familiarScript.myTurn)
        {
            if (moves)
            {
                canMove = true;
                characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);
                familiarScript.talkingToNPC = false;
            }
            else
            {
                velocity = Vector3.zero; // stop movements
                Debug.Log(velocity);
                canMove = false;
                characterAnimationHandler.ToggleMoveSpeedBlend(0);
                familiarScript.talkingToNPC = true;

                if (AudioManager.instance.footStepsChannel.isPlaying)
                    AudioManager.instance.StopSoundAfterLoop(AudioManagerChannels.footStepsLoopChannel);
            }
        }
        else
        {
            if (moves)
            {
                canMove = true;
                characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);
                playerController.talkingToNPC = false;
            }
            else
            {
                velocity = Vector3.zero; // stop movements
                canMove = false;
                characterAnimationHandler.ToggleMoveSpeedBlend(0);
                playerController.talkingToNPC = true;

                if (AudioManager.instance.footStepsChannel.isPlaying)
                    AudioManager.instance.StopSoundAfterLoop(AudioManagerChannels.footStepsLoopChannel);
            }
        }

    }

    public void ToggleCanLook(bool look) {
        if (familiarScript.myTurn) {
            if (look) {
                canLook = true;
            }
            else {
                canLook = false;
            }
        }
        else {
           if (look) {
                canLook = true;
            }
            else {
                canLook = false;
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
                if (direction.magnitude >= 0.1f && !turning)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, speed, acceleration * Time.deltaTime);
                    characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);

                    if (characterController.isGrounded)
                    {
                        isNearGround = true;
                        //Play footstep Audio!
                        if (TryGetComponent<PlayerControllerNew>(out PlayerControllerNew playerCon))
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
                    if (currentSpeed > 0.1)
                    {
                        currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);
                    }
                    else
                    {
                        currentSpeed = 0;
                    }
                    characterAnimationHandler.ToggleMoveSpeedBlend(currentSpeed);
                    if (AudioManager.instance.footStepsChannel.isPlaying)
                        AudioManager.instance.StopSoundAfterLoop(AudioManagerChannels.footStepsLoopChannel);
                }

                if (!freeMove)
                {
                    velocity.x = currentSpeed * transform.forward.x;
                    velocity.z = currentSpeed * transform.forward.z;
                }
                else
                {
                    velocity.x = currentSpeed * newDirection.x;
                    velocity.z = currentSpeed * newDirection.z;
                }

                characterController.Move(velocity * Time.deltaTime); // make move

                //Character gravity
                if (!characterController.isGrounded)
                {
                    isNearGround = false;
                    characterAnimationHandler.ToggleFallAnim(true);
                    if (TryGetComponent<PlayerControllerNew>(out PlayerControllerNew playerCon) && !AudioManager.instance.fallChannel.isPlaying && canPlayFallAudio)
                    {
                        AudioManager.instance.PlaySound(AudioManagerChannels.fallLoopChannel, weaverFallClip);
                    }
                    else
                    {
                        if (TryGetComponent<OwlDiveScript>(out OwlDiveScript diveScript) && diveScript.isDiving && canPlayFallAudio)
                        {
                            if (!AudioManager.instance.fallChannel.isPlaying || AudioManager.instance.fallChannel.clip != owlDiveClip && canPlayFallAudio)
                            {
                                AudioManager.instance.PlaySound(AudioManagerChannels.fallLoopChannel, owlDiveClip);
                            }
                        }
                        else if (canPlayFallAudio)
                        {
                            if (!AudioManager.instance.fallChannel.isPlaying || AudioManager.instance.fallChannel.clip != owlGlideClip)
                            {
                                AudioManager.instance.PlaySound(AudioManagerChannels.fallLoopChannel, owlGlideClip);
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
            else if (!active)
            {
                velocity.x = 0f;
                velocity.z = 0f;
                velocity.y += gravity * Time.deltaTime;
                characterController.Move(velocity * Time.deltaTime); // make move
            }

            if (resettingTerminalVelocity)
            {
                ResetTerminalVelocity();
            }

            if (inCutscene)
            {
                AudioManager.instance.StopSound(AudioManagerChannels.fallLoopChannel);
                AudioManager.instance.StopSound(AudioManagerChannels.footStepsLoopChannel);
            }
        }
        else 
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
            if (characterController.isGrounded) {
                characterAnimationHandler.ToggleFallAnim(false);
            }
        }
    }

    public void LookAndMove()
    {
        direction = new Vector3(movement.x, 0, movement.y).normalized; //direction of movement
        
        //Character rotations
        if (direction.magnitude >= 0.2f && canLook)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, timeToTurn);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            newDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            
            if (Vector3.Distance(transform.forward, newDirection) > 0.8 && !freeMove)
            {
                turning = true;
            }
            else
            {
                turning = false;
            }
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

    public void ZeroCurrentSpeed()
    {
        currentSpeed = 0;
        characterAnimationHandler.ToggleMoveSpeedBlend(0);
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

    public void ChangeTimeToTurn(float newTimeToTurn)
    {
        timeToTurn = newTimeToTurn;
    }

    public void ResetTimeToTurn()
    {
        timeToTurn = originalTimeToTurn;
    }

    // -------------------------------------------------------------------

    public void GoToCheckPoint()
    {
        StartCoroutine(ChangeMaterialOnDeath());
        deathVFX.Play();
        FadeToBlack.instance.StartFadeToBlack();


        if (TryGetComponent<PlayerControllerNew>(out PlayerControllerNew playerCon))
        {
            playerCon.Death();
        }

        if (TryGetComponent<FamiliarScript>(out FamiliarScript familiarScript))
        {
            familiarScript.Death();
        }
    }

    public IEnumerator ChangeMaterialOnDeath()
    {
        materialHolder.GetComponent<SkinnedMeshRenderer>().material = dissolveMat;

        float elapsedTime = 2;
        float totalTime = 2;
        float cutoffHeight = 4;

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime * 1.5f;

            cutoffHeight = Mathf.Lerp(-3, 4, elapsedTime / totalTime);
            dissolveMat.SetFloat("_Cutoff_Height", cutoffHeight);

            yield return null;
        }

        yield return new WaitForSeconds(2);
        dissolveMat.SetFloat("_Cutoff_Height", 4);

        materialHolder.GetComponent<SkinnedMeshRenderer>().material = defaultMat;

        yield break;
    }

    public IEnumerator DelayBeforeFallAudio()
    {
        yield return new WaitForSeconds(3);

        canPlayFallAudio = true;
        yield break;
    }
}
