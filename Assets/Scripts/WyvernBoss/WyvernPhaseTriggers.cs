using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernPhaseTriggers : MonoBehaviour
{
    public enum TriggerType {
        FireballAndMagicCircle,
        MagicCircleAndFlameThrower,
        FlamethrowerAndFireball
    }
    private int currentPhase;
    private int newPhase;
    public TriggerType triggerType;
    [SerializeField] private GameObject wyvern;
    private WyvernBossManager bossManager;
    private BoxCollider bc;
    private int bossPhase;
    private bool weaverInsideTrigger, familiarInsideTrigger; //Note to self: used for determining if a trigger has been triggered if swapping to the other character
    private bool triggered;
    private bool isZTrigger;
    private Vector3 currentPosition;
    private bool toggle;

    // Start is called before the first frame update
    void Start()
    {
        bossManager = wyvern.GetComponent<WyvernBossManager>();
        bc = GetComponent<BoxCollider>();

        bossPhase = bossManager.phases;
        currentPhase = bossPhase;
        triggered = false;
        weaverInsideTrigger = false;
        familiarInsideTrigger = false;

        //Sets the phase this trigger will trigger if tripped.
        switch (currentPhase) {
            //Fireball
            case 1:
                switch (triggerType) {
                    case TriggerType.FireballAndMagicCircle:
                        newPhase = 2;
                    break;
                    case TriggerType.FlamethrowerAndFireball:
                        newPhase = 3;
                    break;
                }
            break;
            //Magic Circle
            case 2:
                switch (triggerType) {
                    case TriggerType.FireballAndMagicCircle:
                        newPhase = 1;
                    break;
                    case TriggerType.MagicCircleAndFlameThrower:
                        newPhase = 3;
                    break;
                }
            break;
            //Flamethrower
            case 3:
                switch (triggerType) {
                    case TriggerType.MagicCircleAndFlameThrower:
                        newPhase = 2;
                    break;
                    case TriggerType.FlamethrowerAndFireball:
                        newPhase = 1;
                    break;
                }
            break;
        }

        if (bc.size.x < bc.size.z) {
            isZTrigger = false;
        }
        else {
            isZTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (familiarInsideTrigger == false) {
                weaverInsideTrigger = true;
                FlipFlop(currentPhase,newPhase);
            }
            
        }
        if (other.gameObject.CompareTag("Familiar")) {
            if (weaverInsideTrigger == false) {
                familiarInsideTrigger = true;
                FlipFlop(currentPhase,newPhase);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (familiarInsideTrigger == false) {
                weaverInsideTrigger = false;
                currentPosition = other.gameObject.transform.position;
                if (isZTrigger) {

                }
            }
        }
        if (other.gameObject.CompareTag("Familiar")) {
            if (weaverInsideTrigger == false) {
                familiarInsideTrigger = false;
                currentPosition = other.gameObject.transform.position;
                if (isZTrigger) {
                    
                }
            }
        }
    }

    //Flips between one phase and the other.
    void FlipFlop(int phase1, int phase2) {
        if (toggle == false) {
            //Phase 1 to Phase 2
            bossManager.phases = phase2;
            toggle = true;
        }
        else {
            //Phase 2 to Phase 1
            bossManager.phases = phase1;
            toggle = false;
        }
    }

}
