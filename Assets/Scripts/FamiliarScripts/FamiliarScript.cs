using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting; //this is for testing 

public class FamiliarScript : MonoBehaviour
{

    [Header("Movement Variables")]
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    public bool depossessing; //True if the familiar is being deposessed
    public bool myTurn; //Responsible for determining if the familiar can move
    public bool leapOfFaith; //Determines if owl familiar is in a leap of faith 
    public bool familiarMovementAbility;//Only used for reading if familiar is using movemeny ability
    private bool insideTrigger;
    [HideInInspector] public bool talkingToNPC;
    private GameMasterScript GM; //This is refrencing the game master script
    [SerializeField] private AudioClip possessionClip;
    //public InputAction NPCInteraction;

    [HideInInspector] public bool isDead;
    public bool islandisfalling;

    void Awake()
    {
        //references to character components
        isDead = false;
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
        characterController.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        myTurn = false;
        islandisfalling = false;
        depossessing = false;
        leapOfFaith = false;
        familiarMovementAbility = false;
        insideTrigger = false;
        talkingToNPC = false;

        //these two lines are grabing the game master's last checkpoint position
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>(); 
        characterController.enabled = true;
        // Debug.Log("Active Current Position: " + transform.position);
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Crystal"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.GetComponent<CrystalScript>().TriggerBreak();
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        ITriggerable trigger = collision.GetComponent<ITriggerable>();
        if (trigger != null)
        {
            trigger.OnTrigEnter(collision);
        }

        if (collision.gameObject.CompareTag("Leap of Faith Trigger"))
        {
            CameraMasterScript.instance.StartLeapOfFaith();
        }

        else if (collision.gameObject.CompareTag("Kill Area"))
        {
            Debug.Log("Death");
            CameraMasterScript.instance.EndLeapOfFaith();
            Death();
        }

        else if (collision.gameObject.CompareTag("Hazard"))
        {
            Death();
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

    public void Depossess() {
        if (myTurn && Time.timeScale != 0 && !islandisfalling && !talkingToNPC && movementScript.active) {
            //Move character only if they are on the ground or in leapOfFaith
            if (characterController.isGrounded || leapOfFaith) {     
                if (!leapOfFaith) {
                    depossessing = true;
                    movementScript.ZeroCurrentSpeed();
                    movementScript.active = false;
                    AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, possessionClip);
                }
            }
        }   
    }

    //This coroutine is for an intentional delay that lasts exactly 1 frame, which starts on the frame the possession button is pressed.
    //This is so the the control for depossessing does not get activated on the same frame.
    public IEnumerator ForcedDelay() {
        yield return new WaitForNextFrameUnit();
        movementScript.active = true;
        myTurn = true;
        StopCoroutine(ForcedDelay());
        //movementScript.ToggleCanMove(false);
        //movementScript.ToggleCanLook(false);
    }

    public void Death() // this should be an interface EVENTUALLY WAHOO // I AGREE OMG OMG SAME BESTIE
    {
        movementScript.active = false;
        isDead = true;
        Invoke("ResetToCheckpoint", 3);
    }

    public void ResetToCheckpoint()
    {
        characterController.enabled = false;
        transform.position = GM.FamiliarCheckPointPos;
        movementScript.HardResetMovementStats();

        CameraMasterScript.instance.FamiliarCameraReturnOnDeath(CameraMasterScript.instance.lastFamiliarCameraTriggered);
        characterController.enabled = true;
        movementScript.active = true;
        isDead = false;
    }

}
