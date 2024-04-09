using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDialoguePlayer : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject textBox;

    public void NextDialogue()
    {
        DialogueManager.instance.DisplayNextSentence();
    }

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue, textBox);
    }
}
