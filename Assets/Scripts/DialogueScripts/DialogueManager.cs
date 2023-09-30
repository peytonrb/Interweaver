using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private GameObject textBoxUI;
    private AudioClip audioClip;
    
    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }
    }

    // begins the dialogue
    public void StartDialogue(Dialogue dialogue, GameObject textBox)
    {
        initialize(textBox);
        nameText.text = dialogue.name;
        sentences.Clear();
        textBoxUI.SetActive(true);

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    // when enter is hit, next sentence is displayed
    public void DisplayNextSentence()
    {
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
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray()) // add array of clips w pitches to be randomly called from here
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        textBoxUI.SetActive(false);
    }

    // initializes the text objects for intended dialogue
    private void initialize(GameObject textBox)
    {
        textBoxUI = textBox;
        nameText = textBox.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        dialogueText = textBox.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }
}