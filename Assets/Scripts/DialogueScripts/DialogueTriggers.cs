using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggers : MonoBehaviour
{
    public float maxDistance = 18.0f;
    private GameObject player;
    public Dialogue dialogue;
    public GameObject textBox;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, FindObjectOfType<DialogueManager>().textBoxUI.transform.position);

        if (distance > maxDistance)
        {
            FindObjectOfType<DialogueManager>().EndDialogue();
        }
    }

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
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, textBox);
        }
    }
}