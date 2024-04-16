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
    public bool doesAnimate = false;
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

    [Header("Dynamic Dialogue")]
    [SerializeField] private bool hasDynamicDialogue;
    [SerializeField] private List<Dialogue> dialogueList = new List<Dialogue>();

    void Update()
    {
        if (myMoveScript != null && isInteracting)
        {
            myMoveScript.ToggleCanMove(false);
            myMoveScript.ToggleCanLook(false);
        }
    }


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
                if (doesAnimate)
                    ToggleAnim(true);

                RefreshDynamicDialogue();
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

            if (doesAnimate)
                ToggleAnim(true);

            if (!isInteracting)
            {
                RefreshDynamicDialogue();
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
        if (doesAnimate)
        ToggleAnim(false);

        if (isAutoTrigger && triggerOnlyOnce)
        {
            triggerOnlyOnce = false;
            isAutoTrigger = false;
        }
        myMoveScript.ToggleCanMove(true);
        myMoveScript.ToggleCanLook(true);
        DialogueManager.instance.inAutoTriggeredDialogue = false;
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
                if (doesAnimate)
                    ToggleAnim(true);

                myMoveScript = collider.GetComponent<MovementScript>();
                DialogueManager.instance.inAutoTriggeredDialogue = true;
                DialogueManager.instance.currentTrigger = this;
                RefreshDynamicDialogue();
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
                myMoveScript.ToggleCanLook(false);
                triggered = true;
            }
        }

        if (!triggerOnlyOnce)
        {
            if (isAutoTrigger)
            {
                if (doesAnimate)
                    ToggleAnim(true);

                myMoveScript = collider.GetComponent<MovementScript>();
                DialogueManager.instance.inAutoTriggeredDialogue = true;
                DialogueManager.instance.currentTrigger = this;
                RefreshDynamicDialogue();
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
                popupUIInteraction.SetActive(false);
            }
            else
            {
                popupUIInteraction.SetActive(false);
            }
        }
    }

    private void RefreshDynamicDialogue()
    {
        if (!hasDynamicDialogue)
        {
            return;
        }

        if (dialogueList.Count > PlayerData.levelsCompleted && dialogueList[PlayerData.levelsCompleted])
        {
            dialogue = dialogueList[PlayerData.levelsCompleted];
        }
    }

    public void ToggleAnim(bool toggle)
    {
        if (toggle) 
        {
            GetComponent<Animator>().SetBool("Talking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("Talking", false);
        }
    }
}