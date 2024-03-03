using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggers : MonoBehaviour
{
    [Header("Required")]
    public Dialogue dialogue;
    public GameObject textBox;
    public GameObject ControllerIndicator;
    public GameObject KeyboardIndicator;

    [Header("Optional")]
    public bool isAutoTrigger = false;
    public bool triggerOnlyOnce = false;
    enum CharacterTriggerType
    {
        bothChars,
        Weaver,
        Familiar
    };

    [SerializeField]
    CharacterTriggerType type = new CharacterTriggerType();

    [HideInInspector]
    public bool isInteracting = false;
    [HideInInspector]
    public MovementScript myMoveScript;

    private bool triggered = false;

    // is called if near an NPC 
    public void TriggerDialogue(MovementScript movementScript)
    {
        switch(type)
        {
            case CharacterTriggerType.bothChars:
                {
                    StartDialogueFromInteraction(movementScript);
                    break;
                }
            case CharacterTriggerType.Weaver:
                {
                    if (movementScript.gameObject.CompareTag("Player"))
                    {
                        StartDialogueFromInteraction(movementScript);
                    }
                        break;
                }
            case CharacterTriggerType.Familiar:
                {
                    if (movementScript.gameObject.CompareTag("Familiar"))
                    {
                        StartDialogueFromInteraction(movementScript);
                    }
                    break;
                }
        }

        
        
    }

    private void StartDialogueFromInteraction(MovementScript movementScript)
    {
        if (triggerOnlyOnce && !triggered)
        {

            myMoveScript = movementScript;

            if (!isInteracting)
            {
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                DialogueManager.instance.currentTrigger = this;
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
            }
            else
            {
                DialogueManager.instance.DisplayNextSentence();
            }

            triggered = true;

        }

        if (!triggerOnlyOnce)
        {
            myMoveScript = movementScript;

            if (!isInteracting)
            {
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                DialogueManager.instance.currentTrigger = this;
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
            }
            else
            {
                DialogueManager.instance.DisplayNextSentence();
            }
        }
    }

    public void disableNPCDialogue()
    {
        myMoveScript.ToggleCanMove(true);
        myMoveScript.ToggleCanLook(true);
    }

    // occurs only with Event Triggers
    public void OnTriggerEnter(Collider collider)
    {
        switch(type)
        {
            case CharacterTriggerType.bothChars:
                {
                    if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Familiar")
                        AutoTrigger(collider);
                    break;
                }
            case CharacterTriggerType.Weaver:
                {
  
                    if (collider.gameObject.tag == "Player")
                        AutoTrigger(collider);
                    //this is where I would put the ui and the text element here
                    break;
                }
            case CharacterTriggerType.Familiar:
                {
                    if (collider.gameObject.tag == "Familiar")
                        AutoTrigger(collider);
                    //this is where I would put the ui and the text element here
                    break;
                }
        } 
    }

    private void AutoTrigger(Collider collider)
    {
        if (triggerOnlyOnce && !triggered)
        {
            //potentially get rid of if and else statement since inputmanager can auto detect
            if (InputManagerScript.instance.isGamepad)
            {
                ControllerIndicator.SetActive(true);
            }
            else
            {
                KeyboardIndicator.SetActive(true);
            }

            if (isAutoTrigger)
            {
                myMoveScript = collider.GetComponent<MovementScript>();
                DialogueManager.instance.currentTrigger = this;
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
                triggered = true;
            }
        }

        if (!triggerOnlyOnce)
        {
            //potentially get rid of this if and else statement since the input manager can autodetect
            if (InputManagerScript.instance.isGamepad)
            {
                ControllerIndicator.SetActive(true);
            }
            else
            {
                KeyboardIndicator.SetActive(true);
            }

            if (isAutoTrigger)
            {
                myMoveScript = collider.GetComponent<MovementScript>();
                DialogueManager.instance.currentTrigger = this;
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
                triggered = true;
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Familiar")
        {

            if (isAutoTrigger)
            {
                DialogueManager.instance.EndDialogue();
            }
            else
            {
                ControllerIndicator.SetActive(false);
                KeyboardIndicator.SetActive(false);
            }

        }
    }

    void Update()
    {
        if (isAutoTrigger && isInteracting) // if actively within an event trigger
        {
            if (Input.GetKeyDown(KeyCode.E)) //will need to completely change this to get the input from the input manager
            {
                DialogueManager.instance.DisplayNextSentence();
            }
        }
    }
}