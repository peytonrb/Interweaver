using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalEventTrigger : MonoBehaviour
{
    private GameObject weaver;
    private MovementScript moveScript;
    private GameObject rival;
    private bool hasPlayed; // only will play once. if player runs through trigger again, it would fail

    [Header("Rival Dialogue")]
    public Dialogue dialogue;
    public GameObject textBox;
    [Range(0, 10)] public int secondsUntilDialogueAppears = 2;
    private bool isSpeaking = false;

    void Start()
    {
        hasPlayed = false;
        rival = this.gameObject.transform.GetChild(0).gameObject;
        rival.SetActive(false);
        weaver = GameObject.FindWithTag("Player");
        moveScript = weaver.GetComponent<MovementScript>();
    }

    void Update()
    {
        if (isSpeaking)
        {
            if (Input.GetKeyDown(KeyCode.E)) // will refactor, fastest way to fix this for rn
            {
                DialogueManager.instance.DisplayNextSentence();
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!hasPlayed)
        {
            rival.SetActive(true); // make more interesting w animation
            // add vfx
            StartCoroutine(DialogueStart());
            hasPlayed = true;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        rival.SetActive(false);
    }

    IEnumerator DialogueStart()
    {
        yield return new WaitForSeconds(timeUntilDialogueAppears);
        DialogueManager.instance.StartDialogue(dialogue, textBox);
        moveScript.ToggleCanMove(false);
        isSpeaking = true;
    }
}
