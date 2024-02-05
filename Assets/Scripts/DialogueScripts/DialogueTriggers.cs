using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggers : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject textBox;
    public bool isInteracting = false;
    public MovementScript myMoveScript;

    public bool isAutoTrigger = false;

    public GameObject ControllerIndicator;
    public GameObject KeyboardIndicator;

    // is called if near an NPC 
    public void triggerDialogue(MovementScript movementScript)
    {
        myMoveScript = movementScript;

        if (!isInteracting)
        {
            Debug.Log("interacting");
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
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Familiar")
        {
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
                Debug.Log("TRIGGERED");
                DialogueManager.instance.currentTrigger = this;
                DialogueManager.instance.StartDialogue(dialogue, textBox);
                isInteracting = true;
                myMoveScript.ToggleCanMove(false);
            }

        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Familiar")
        {

            if (isAutoTrigger)
            {
                Debug.Log("TRIGGERED");
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.instance.DisplayNextSentence();
            }
        }
    }
}