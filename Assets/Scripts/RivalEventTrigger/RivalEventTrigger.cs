using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Cinemachine;

public class RivalEventTrigger : MonoBehaviour
{
    private GameObject weaver;
    private MovementScript moveScript;
    private GameObject rival;
    private bool hasPlayed; // only will play once. if player runs through trigger again, it would fail
    public GameObject smokePrefab;
    [CannotBeNullObjectField] public CinemachineVirtualCamera myVirtualCam;

    [Header("Rival Dialogue")]
    public Dialogue dialogue;
    public GameObject textBox;
    [Range(0, 10)] public int secondsUntilDialogueAppears = 2;
    private bool isSpeaking = false;
    private VisualEffect smoke;

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

        if (!DialogueManager.instance.isActive && hasPlayed)
        {
            myVirtualCam.Priority = 0;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!hasPlayed)
        {
            myVirtualCam.Priority = 2;
            rival.SetActive(true); // make more interesting w animation
            moveScript.ToggleCanMove(false);
            smoke = Instantiate(smokePrefab, rival.transform.position - new Vector3(0, 1.5f, 0), Quaternion.identity).GetComponent<VisualEffect>();
            smoke.Play();
            StartCoroutine(DialogueStart());
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        smoke.Play();
        rival.SetActive(false);
    }

    IEnumerator DialogueStart()
    {
        yield return new WaitForSeconds(secondsUntilDialogueAppears);
        DialogueManager.instance.StartDialogue(dialogue, textBox);
        moveScript.ToggleCanMove(false);
        isSpeaking = true;
        hasPlayed = true;
    }
}
