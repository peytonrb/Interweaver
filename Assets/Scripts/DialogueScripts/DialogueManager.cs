using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private GameObject textBoxUI;
    public bool inAutoTriggeredDialogue;
    private AudioClip audioClip;

    public AudioClip speechFile;
    public DialogueTriggers currentTrigger;
    
    public static DialogueManager instance;
    private MovementScript moveScript;
    public bool isActive;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sentences = new Queue<string>();
        isActive = false;
        moveScript = GameObject.FindWithTag("Player").GetComponent<MovementScript>();
    }

    // begins the dialogue
    public void StartDialogue(Dialogue dialogue, GameObject textBox)
    {
        initialize(textBox);
        nameText.text = dialogue.name;
        sentences.Clear();
        textBoxUI.SetActive(true);
        isActive = true;

        //int i = 0;
        foreach (string sentence in dialogue.sentences)
        {
            //i++;
            //Debug.Log("Sentence #" + i + ": " + sentence); //-- okay here
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    // when enter is hit, next sentence is displayed
    public void DisplayNextSentence()
    {
        if (textBoxUI == null)
        {
            Debug.Log("ee");
            return; 
        }
            

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(SentenceScroll(sentence));
    }

    // word scroll functionality
    IEnumerator SentenceScroll(string sentence)
    {
        //Debug.Log(sentence); - prints 2 lines at a time
        dialogueText.text = "";

        
            foreach (char letter in sentence.ToCharArray()) // add array of clips w pitches to be randomly called from here
        {
            if(!isActive)
            {
                break;
            }
            //Debug.Log("Ahh");
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, speechFile, 1f);
            yield return new WaitForSeconds(.02f);
            dialogueText.text += letter;
            yield return null;
            
        }
    }

    public void EndDialogue()
    {
        textBoxUI.SetActive(false);

        if (currentTrigger != null)
        {
            currentTrigger.isInteracting = false;
            currentTrigger.disableNPCDialogue();
        }

        moveScript.ToggleCanMove(true);
        isActive = false;
    }

    // initializes the text objects for intended dialogue
    private void initialize(GameObject textBox)
    {
        textBoxUI = textBox;
        nameText = textBox.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        dialogueText = textBox.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }
}