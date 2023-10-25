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
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, textBox);
    }

    // occurs only with Event Triggers
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("TRIGGERED");
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, textBox);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("TRIGGERED");
            FindObjectOfType<DialogueManager>().EndDialogue();
        }
    }
}