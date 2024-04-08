using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControllerNew : MonoBehaviour
{
    [Header("Movement/Possession")]
    private CharacterController characterController;
    private MovementScript movementScript;
    [HideInInspector] public bool possessing;

    [Header("Camera")]
    [CannotBeNullObjectField][SerializeField] private Camera mainCamera;
    private int vCamRotationState; //State 0 is default

    [Header("Death")]
    public bool isDead = false;
    [SerializeField] private float deathTimer = 3.0f;

    [Header("Cutscenes/UI")]
    [CannotBeNullObjectField] public GameObject pauseMenu;
    private PauseScript pauseScript;
    [HideInInspector] public bool inCutscene;
    [HideInInspector] public bool talkingToNPC;

    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;

    [Header("Audio")]
    [SerializeField] private AudioClip possessionClip;
    [SerializeField] private AudioClip deathClip;

    [Header("References")]
    [CannotBeNullObjectField] public GameObject familiar;
    [CannotBeNullObjectField] public RespawnController respawnController;
    private FamiliarScript familiarScript;
    private GameMasterScript GM;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        characterController.enabled = false;
    }

    void Start()
    {
        familiarScript = familiar.GetComponent<FamiliarScript>();
        pauseScript = pauseMenu.GetComponent<PauseScript>();
        possessing = false;
        vCamRotationState = 0;
        talkingToNPC = false;

        pauseMenu.SetActive(false);

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        characterController.enabled = true;

        inCutscene = movementScript.inCutscene;

        if (SceneManager.GetActiveScene().name != "Sepultus")
        {
            this.transform.Find("WyvernCamera").gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // If game is not paused, return to normal movement functions
        if (Time.timeScale != 0 && !talkingToNPC)
        {
            if (familiarScript.depossessing)
            {
                CameraMasterScript.instance.SwitchToWeaverCamera();
                familiarScript.myTurn = false;
                possessing = false;
                familiarScript.depossessing = false;
                movementScript.active = true;
            }

            inCutscene = movementScript.inCutscene;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //See CameraIndexScript for more information about this function
        ITriggerable trigger = other.GetComponent<ITriggerable>();
        if (trigger != null)
        {
            trigger.OnTrigExit(other);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //See CameraIndexScript for more information about this function
        ITriggerable trigger = other.GetComponent<ITriggerable>();
        if (trigger != null)
        {
            //Debug.Log("Hit?");
            trigger.OnTrigEnter(other);
        }
    }

    public void Possession()
    {
        //Move character only if they are on the ground
        if (characterController.isGrounded && Time.timeScale != 0 && !inCutscene && !talkingToNPC)
        {
            if (possessing == false)
            {
                //Switches to Familiar
                CameraMasterScript.instance.SwitchToFamiliarCamera();
                possessing = true;
                movementScript.ZeroCurrentSpeed();
                movementScript.active = false;
                StartCoroutine(familiarScript.ForcedDelay());

                AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, possessionClip);
            }
        }
    }

    public void Death()
    {
        if (!isDead)
        {
            isDead = true;
            movementScript.active = false;
            characterAnimationHandler.ToggleDeathAnim();
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, deathClip);
            StartCoroutine(DeathTimer(deathTimer));
        }
    }

    IEnumerator DeathTimer(float deathTimer)
    {
        yield return new WaitForSeconds(deathTimer);

        transform.position = GM.WeaverCheckPointPos;
        CameraMasterScript.instance.WeaverCameraReturnOnDeath(CameraMasterScript.instance.lastWeaverCameraTriggered);
        movementScript.HardResetMovementStats();

        if (GM.WeaverCheckPointNum == 0 && respawnController != null) // first checkpoint in shield puzzle - should also specify scene
        {
            respawnController.RespawnInShieldPuzzle();
        }

        characterAnimationHandler.ToggleRespawnAnim();

        isDead = false;
        movementScript.active = true;

        yield break;
    }

    public void SetNewPosition(Vector3 newposition, Quaternion newrotation)
    {
        transform.position = newposition;
        transform.rotation = newrotation;
    }
}