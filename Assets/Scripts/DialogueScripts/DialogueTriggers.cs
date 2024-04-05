using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueTriggers : MonoBehaviour
{
    [Header("Required")]
    public Dialogue dialogue;
    public GameObject textBox;
    public GameObject popupUIInteraction;

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
        switch (type)
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

        popupUIInteraction.SetActive(false);
    }

    // occurs only with Event Triggers
    public void OnTriggerEnter(Collider collider)
    {
        var weaverNPCInteraction = InputManagerScript.instance.playerInput.actions["NPCInteraction"].GetBindingDisplayString();
        var familiarNPCInteraction = InputManagerScript.instance.playerInput.actions["NPCInteraction"].GetBindingDisplayString();
        switch (type)
        {
            case CharacterTriggerType.bothChars:
                {
                    if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Familiar")
                        AutoTrigger(collider);
                    popupUIInteraction.SetActive(true);
                    popupUIInteraction.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().SetText("<sprite name=" + weaverNPCInteraction + ">"
                         + " ...");
                    break;
                }
            case CharacterTriggerType.Weaver:
                {

                    if (collider.gameObject.tag == "Player")
                        AutoTrigger(collider);
                    popupUIInteraction.SetActive(true);
                    popupUIInteraction.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().SetText("<sprite name="+weaverNPCInteraction+">"
                         + " ...");
                    //this is where I would put the ui and the text element here
                    break;
                }
            case CharacterTriggerType.Familiar:
                {
                    if (collider.gameObject.tag == "Familiar")
                        AutoTrigger(collider);
                    popupUIInteraction.SetActive(true);
                    popupUIInteraction.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().SetText("<sprite name="+familiarNPCInteraction+">"
                         + " ...");
                    //this is where I would put the ui and the text element here
                    break;
                }
        }
    }

    private void AutoTrigger(Collider collider)
    {
        if (triggerOnlyOnce && !triggered)
        {
            if (isAutoTrigger)
            {
                myMoveScript = collider.GetComponent<MovementScript>();
                DialogueManager.instance.currentTrigger = this;
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
                //Tells the jump and dash script that the weaver has just dashed into an auto trigger.
                JumpAndDashScript jads = collider.GetComponent<JumpAndDashScript>();
                if (jads != null) 
                {
                    jads.hitAutoTrigger = true;
                }
                triggered = true;
            }
        }

        if (!triggerOnlyOnce)
        {

            if (isAutoTrigger)
            {
                myMoveScript = collider.GetComponent<MovementScript>();
                DialogueManager.instance.currentTrigger = this;
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
                JumpAndDashScript jads = collider.GetComponent<JumpAndDashScript>();
                if (jads != null) 
                {
                    jads.hitAutoTrigger = true;
                }
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
                popupUIInteraction.SetActive(false);
            }
            else
            {
                popupUIInteraction.SetActive(false);
            }

        }
    }

  
}