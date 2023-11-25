using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggers : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject textBox;
    public bool isInteracting = false;
    public MovementScript myMoveScript;

    // is called if near an NPC 
    public void triggerDialogue(MovementScript movementScript)
    {
        myMoveScript = movementScript;

        if (!isInteracting)
        {
            DialogueManager.instance.StartDialogue(dialogue, textBox);
            DialogueManager.instance.currentTrigger = this;
            isInteracting = true;
            myMoveScript.ToggleCanMove(false);
        }
        else
        {
            DialogueManager.instance.DisplayNextSentence();
            Debug.Log("advancing");
        }
    }

    public void disableNPCDialogue()
    {
        myMoveScript.ToggleCanMove(true);
    }

    // occurs only with Event Triggers
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("TRIGGERED");
            DialogueManager.instance.StartDialogue(dialogue, textBox);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("TRIGGERED");
            DialogueManager.instance.EndDialogue();
        }
    }
}