using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateScript : MonoBehaviour
{
    private bool bottomedOut; //If its bottomed out, stop moving it
    private bool toppedOut; //Same thing as bottomed out except in reverse
    public float weight; //Player and familiar will have the same weight
    [Header("FOR DEBUGGING")]
    public bool standingOnPlate; //If player is standing on plate, this is true
    public bool activated;
    private float bottomtargetposition;
    private float toptargetposition;
    public bool activatedByWeaveable = false;
    public UnityEvent pressEvent;

    [Header("Audio")]
    [SerializeField] private AudioClip pressurePlateSound;

    void Start() {
        bottomtargetposition = transform.position.y - 0.2f;
        toptargetposition = transform.position.y;
        bottomedOut = false;
        toppedOut = true;
        activated = false;
    }

    void Update() {
        if (standingOnPlate && !bottomedOut) {
            PushPlate();
        }
        else if (!standingOnPlate && !toppedOut) {
            PullPlate();
        }
        else if (activated) {
            Activation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar"))
        {
            standingOnPlate = true;
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, pressurePlateSound, 1f);
        }

        if (activatedByWeaveable && other.gameObject.CompareTag("Weaveable"))
        {
            standingOnPlate = true;
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, pressurePlateSound, 1f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar")) 
        {
            standingOnPlate = false;
        }

        if (activatedByWeaveable && other.gameObject.CompareTag("Weaveable"))
        {
            standingOnPlate = false;
        }
    }

    void PushPlate() {
        if (transform.position.y > bottomtargetposition) {
            float newposition = Mathf.MoveTowards(transform.position.y, bottomtargetposition, weight * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newposition, transform.position.z);
        }
        else {
            bottomedOut = true;
            activated = true;
        }
        toppedOut = false;
    }

    void PullPlate() {
        if (transform.position.y < toptargetposition) {
            float newposition = Mathf.MoveTowards(transform.position.y, toptargetposition, weight * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newposition, transform.position.z);
        }
        else {
            toppedOut = true;
        }
        bottomedOut = false;
        activated = false;
    }

    void Activation() {
        pressEvent.Invoke();
    }
}
