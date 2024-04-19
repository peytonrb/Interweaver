using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using System;

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
    private bool isScrolling; // simply a bool that flags as true when a sentence is still scrolling
    private bool skipSentence; // when true, finishes sentence

    [Header("Screen Shake")]
    [SerializeField] private float screenShakeAmplitude;
    [SerializeField] private float screenShakeFrequency;
    private bool shakeInvoked;

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
        textBoxUI = textBox;
        textBoxUI.SetActive(true);
        isActive = true;
        shakeInvoked = false; // avoid shaking at first sight of a number

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
        if (isScrolling)
        {
            skipSentence = true;
            return;
        }

        if (textBoxUI == null)
        {
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

        if (sentence.Contains("[SHAKE]"))
        {
            shakeInvoked = true;
            DisplayNextSentence();
        }
        if (shakeInvoked)
        {
            //Debug.Log(sentence.Any(char.IsDigit));
            double screenShakeLength = Convert.ToDouble(sentence);
            CameraMasterScript.instance.ShakeCurrentCamera(screenShakeAmplitude, screenShakeFrequency, (float)screenShakeLength);
            shakeInvoked = false;
            DisplayNextSentence();
        }
    }

    // word scroll functionality
    IEnumerator SentenceScroll(string sentence)
    {
        //Debug.Log(sentence); - prints 2 lines at a time
        dialogueText.text = "";
        isScrolling = true;

        int currentPos = 0;

        foreach (char letter in sentence.ToCharArray()) // add array of clips w pitches to be randomly called from here
        {
            if (!isActive)
            {
                break;
            }
            if (!skipSentence)
            {
                AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, speechFile, 1f);
                if (letter.Equals('<'))
                {
                    InsertIcon(sentence, currentPos, sentence.IndexOf('>'));
                    break;
                }
                else
                {
                    dialogueText.text += letter;
                }

                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                if (!sentence.Contains('<'))
                dialogueText.text = sentence;
                else
                {
                    InsertIcon(sentence, sentence.IndexOf('<'), sentence.IndexOf('>'));
                    break;
                }
                break;
            }

            yield return null;

            currentPos++;
        }

        isScrolling = false;
        skipSentence = false;
    }

    private string GetPhrase(string sentence, int startPos)
    {
        string phrase = "";
        char[] charArray = sentence.ToCharArray();

        for (int i = startPos; i < charArray.Length; i++)
        {
            if (charArray[i].ToString() == ">")
            {
                break;
            }
            phrase += charArray[i];
        }

        return phrase;
    }

    private void InsertIcon(string sentence, int currentPos, int lastPos)
    {
        bool isController = InputManagerScript.instance.isGamepad;

        GetPhrase(sentence, currentPos + 1);


        dialogueText.text = sentence;


        //check the phrase
        switch (GetPhrase(sentence, currentPos + 1))
        {
            case "move":
                {
                    if (isController) // checks current control type. Using the current input for that control type adds the entire <sprite name=whatever> phrase
                    {
                        for(int i = currentPos; i < currentPos; i++)
                        {
                            sentence.Remove(currentPos);
                        }
                        
                        sentence.Insert(currentPos, "sprite name=LS");
                    }
                    else
                    {
                        dialogueText.text = dialogueText.text.Remove(currentPos);

                        dialogueText.text += "<sprite name=WASD>";               

                    }
                    break;
                }
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