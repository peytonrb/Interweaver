using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggers : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject textBox;
    public bool isInteracting = false;

    // is called if near an NPC 
    public void triggerDialogue()
    {
        if (!isInteracting)
        {
            DialogueManager.instance.StartDialogue(dialogue, textBox);
            DialogueManager.instance.currentTrigger = this;
            isInteracting = true;
        }
        else
        {
            DialogueManager.instance.DisplayNextSentence();
            Debug.Log("advancing");
        }
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