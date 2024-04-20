using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Cinemachine;
using Unity.VisualScripting;
using System.Linq;

public class RivalEventTrigger : MonoBehaviour
{
    private GameObject weaver;
    private MovementScript moveScript;
    [HideInInspector] public GameObject rival;
    private bool hasPlayed; // only will play once. if player runs through trigger again, it would fail
    [CannotBeNullObjectField] public GameObject smokePrefab;
    private CinemachineVirtualCamera myVirtualCam;

    [Header("Rival Dialogue")]
    [CannotBeNullObjectField] public Dialogue dialogue;
    [CannotBeNullObjectField] public GameObject textBox;
    [Range(0, 10)] public int secondsUntilDialogueAppears = 2;
    [HideInInspector] public bool isSpeaking = false;
    [HideInInspector] public VisualEffect smoke;

    [CannotBeNullObjectField] public Animator animator;

    void Start()
    {
        hasPlayed = false;
        rival = this.gameObject.transform.GetChild(0).gameObject;
        rival.SetActive(false);
        weaver = GameObject.FindWithTag("Player");
        moveScript = weaver.GetComponent<MovementScript>();
        myVirtualCam = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (isSpeaking)
        {          
           if(!DialogueManager.instance.isActive)
           {
              animator.SetTrigger("Laugh");
           }
            
        }

        if (!DialogueManager.instance.isActive && hasPlayed)
        {
            myVirtualCam.Priority = 0;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!hasPlayed && collider.CompareTag("Player"))
        {
            myVirtualCam.Priority = 2;
            rival.SetActive(true); // make more interesting w animation
            moveScript.ToggleCanMove(false);
            smoke = Instantiate(smokePrefab, rival.transform.position - new Vector3(0, 1.5f, 0), Quaternion.identity).GetComponent<VisualEffect>();
            smoke.Play();
            InputManagerScript.instance.isRivalTrigger = true;            
            StartCoroutine(DialogueStart());
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player") && !hasPlayed)
        {
            animator.SetTrigger("Leave");
            hasPlayed = true;
            isSpeaking = false;
            InputManagerScript.instance.isRivalTrigger = false;
        }
        
    }

    public void NextSentenceForRival()
    {
        DialogueManager.instance.DisplayNextSentence();
    }
    IEnumerator DialogueStart()
    {
        yield return new WaitForSeconds(secondsUntilDialogueAppears);
        DialogueManager.instance.StartDialogue(dialogue, textBox);
        moveScript.ToggleCanMove(false);
        isSpeaking = true;
    }
}
