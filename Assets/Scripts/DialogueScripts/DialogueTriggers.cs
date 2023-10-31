using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggers : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject textBox;

    // is called if near an NPC 
    public void triggerDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue, textBox);
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