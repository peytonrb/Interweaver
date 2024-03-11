using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WyvernPhaseTriggers : MonoBehaviour
{
    public enum TriggerType {
        FireballAndMagicCircle,
        MagicCircleAndFlameThrower,
        FlamethrowerAndFireball
    }
    [HideInInspector] public int currentPhase;
    [HideInInspector] public int newPhase;
    public TriggerType triggerType;
    [SerializeField] private GameObject wyvern;
    private WyvernBossManager bossManager;
    private WyvernPhaseTriggerManager triggerManager;
    private BoxCollider bc;
    private bool weaverInsideTrigger, familiarInsideTrigger; //Note to self: used for determining if a trigger has been triggered if swapping to the other character
    public bool triggered;
    private bool isZTrigger;
    private bool enteredFromNorth;
    private Vector3 currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        bossManager = wyvern.GetComponent<WyvernBossManager>();
        triggerManager = GetComponentInParent<WyvernPhaseTriggerManager>();
        bc = GetComponent<BoxCollider>();

        triggered = false;

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
                currentPosition = other.gameObject.transform.position;
                if (isZTrigger) {
                    if (bc.center.z > currentPosition.z) {
                        enteredFromNorth = false;
                    }
                    else {
                        enteredFromNorth = true;
                    }
                }
                else {
                    if (bc.center.x > currentPosition.x) {
                        enteredFromNorth = false;
                    }
                    else {
                        enteredFromNorth = true;
                    }
                }
                FlipFlop(currentPhase,newPhase,true);
                weaverInsideTrigger = true;
            }
            
        }
        if (other.gameObject.CompareTag("Familiar")) {
            if (weaverInsideTrigger == false) {
                currentPosition = other.gameObject.transform.position;
                if (isZTrigger) {
                    if (bc.center.z > currentPosition.z) {
                        enteredFromNorth = false;
                    }
                    else {
                        enteredFromNorth = true;
                    }
                }
                else {
                    if (bc.center.x > currentPosition.x) {
                        enteredFromNorth = false;
                    }
                    else {
                        enteredFromNorth = true;
                    }
                }
                FlipFlop(currentPhase,newPhase,false);
                familiarInsideTrigger = true;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        /*
        if (other.gameObject.CompareTag("Player")) {
            if (familiarInsideTrigger == false) {
                currentPosition = other.gameObject.transform.position;
                if (isZTrigger && !enteredFromNorth) {
                    if (bc.center.z < currentPosition.z) {
                        FlipFlop(currentPhase,newPhase,true);
                    }
                }
                else if (isZTrigger && enteredFromNorth) {
                    if (bc.center.z > currentPosition.z) {
                        FlipFlop(currentPhase,newPhase,true);
                    }
                }
                else if (!isZTrigger && !enteredFromNorth) {
                    if (bc.center.x < currentPosition.x) {
                        FlipFlop(currentPhase,newPhase,true);
                    }
                }
                else if (!isZTrigger && enteredFromNorth) {
                    if (bc.center.x > currentPosition.x) {
                        FlipFlop(currentPhase,newPhase,true);
                    }
                }
                else {
                    Debug.Log("NO FLIP FLOP");
                }
                weaverInsideTrigger = false;
            }
        }
        if (other.gameObject.CompareTag("Familiar")) {
            if (weaverInsideTrigger == false) {
                currentPosition = other.gameObject.transform.position;
                if (isZTrigger && !enteredFromNorth) {
                    if (bc.center.z < currentPosition.z) {
                        FlipFlop(currentPhase,newPhase,false);
                    }
                }
                else if (isZTrigger && enteredFromNorth) {
                    if (bc.center.z > currentPosition.z) {
                        FlipFlop(currentPhase,newPhase,false);
                    }
                }
                else if (!isZTrigger && !enteredFromNorth) {
                    if (bc.center.x < currentPosition.x) {
                        FlipFlop(currentPhase,newPhase,false);
                    }
                }
                else if (!isZTrigger && enteredFromNorth) {
                    if (bc.center.x > currentPosition.x) {
                        FlipFlop(currentPhase,newPhase,false);
                    }
                }
                familiarInsideTrigger = false;
            }
        }
        */
    }

    //Flips between one phase and the other.
    void FlipFlop(int phase1, int phase2, bool isWeaversTurn) {
        if (isWeaversTurn) {
            //Phase 1 to Phase 2
            bossManager.SwitchToPhase(phase2,phase1,isWeaversTurn);
            triggered = true;
            triggerManager.UpdateOtherTriggers();
        }
        else {
            //Phase 1 to Phase 2
            bossManager.SwitchToPhase(phase2,phase1,isWeaversTurn);
            triggered = true;
            triggerManager.UpdateOtherTriggers();
        }
        
    }
    
    //Update other triggers with the change.

    void Update() {
        if (bossManager.updatePhaseOnTrigger) {
            UpdatePhase();
        }
    }

    //Updates the triggers with the current phase after possession/depossession happens.
    public void UpdatePhase() {
        currentPhase = bossManager.phases;
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
        bossManager.updatePhaseOnTrigger = false;
    }

}
