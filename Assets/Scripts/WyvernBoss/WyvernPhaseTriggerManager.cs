using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WyvernPhaseTriggerManager : MonoBehaviour
{
    public GameObject[] triggers;
    public GameObject wyvern;
    private WyvernBossManager bossManager;
    private int currentPhase;
    private int weaverCurrentPhase;
    private int familiarCurrentPhase;
    private bool triggeredTriggerFound;

    // Start is called before the first frame update
    void Start()
    {
        bossManager = wyvern.GetComponent<WyvernBossManager>();

        currentPhase = bossManager.phases;
        weaverCurrentPhase = currentPhase;
        familiarCurrentPhase = currentPhase;

        foreach (GameObject go in triggers) {
            WyvernPhaseTriggers triggerscript = go.GetComponent<WyvernPhaseTriggers>();

            switch (currentPhase) {
                //Fireball
                case 1:
                    switch (triggerscript.triggerType) {
                        case WyvernPhaseTriggers.TriggerType.FireballAndMagicCircle:
                            triggerscript.currentPhase = currentPhase;
                            triggerscript.newPhase = 2;
                        break;
                        case WyvernPhaseTriggers.TriggerType.FlamethrowerAndFireball:
                            triggerscript.currentPhase = currentPhase;
                            triggerscript.newPhase = 3;
                        break;
                    }
                break;
                //Magic Circle
                case 2:
                    switch (triggerscript.triggerType) {
                        case WyvernPhaseTriggers.TriggerType.FireballAndMagicCircle:
                            triggerscript.currentPhase = currentPhase;
                            triggerscript.newPhase = 1;
                        break;
                        case WyvernPhaseTriggers.TriggerType.MagicCircleAndFlameThrower:
                            triggerscript.currentPhase = currentPhase;
                            triggerscript.newPhase = 3;
                        break;
                    }
                break;
                //Flamethrower
                case 3:
                    switch (triggerscript.triggerType) {
                        case WyvernPhaseTriggers.TriggerType.MagicCircleAndFlameThrower:
                            triggerscript.currentPhase = currentPhase;
                            triggerscript.newPhase = 2;
                        break;
                        case WyvernPhaseTriggers.TriggerType.FlamethrowerAndFireball:
                            triggerscript.currentPhase = currentPhase;
                            triggerscript.newPhase = 1;
                        break;
                    }
                break;
            }
        }
    }

    public void UpdateOtherTriggers() {
        //Finds the trigger that was triggered.
        if (triggeredTriggerFound == false) {
            for (int i = 0; i < triggers.Length; i++) {
                WyvernPhaseTriggers triggerscript = triggers[i].GetComponent<WyvernPhaseTriggers>();
                if (triggerscript.triggered) {
                    //Sets the current phase from the triggered trigger.
                    currentPhase = triggerscript.newPhase;
                    triggeredTriggerFound = true;
                    triggerscript.triggered = false;
                    UpdateOtherTriggers();
                }
            }
        }
        else {
            for (int i = 0; i < triggers.Length; i++) {
                WyvernPhaseTriggers triggerscript = triggers[i].GetComponent<WyvernPhaseTriggers>();
                switch (currentPhase) {
                    //Fireball
                    case 1:
                        switch (triggerscript.triggerType) {
                            case WyvernPhaseTriggers.TriggerType.FireballAndMagicCircle:
                                triggerscript.currentPhase = currentPhase;
                                triggerscript.newPhase = 2;
                            break;
                            case WyvernPhaseTriggers.TriggerType.FlamethrowerAndFireball:
                                triggerscript.currentPhase = currentPhase;
                                triggerscript.newPhase = 3;
                            break;
                        }
                    break;
                    //Magic Circle
                    case 2:
                        switch (triggerscript.triggerType) {
                            case WyvernPhaseTriggers.TriggerType.FireballAndMagicCircle:
                                triggerscript.currentPhase = currentPhase;
                                triggerscript.newPhase = 1;
                            break;
                            case WyvernPhaseTriggers.TriggerType.MagicCircleAndFlameThrower:
                                triggerscript.currentPhase = currentPhase;
                                triggerscript.newPhase = 3;
                            break;
                        }
                    break;
                    //Flamethrower
                    case 3:
                        switch (triggerscript.triggerType) {
                            case WyvernPhaseTriggers.TriggerType.MagicCircleAndFlameThrower:
                                triggerscript.currentPhase = currentPhase;
                                triggerscript.newPhase = 2;
                            break;
                            case WyvernPhaseTriggers.TriggerType.FlamethrowerAndFireball:
                                triggerscript.currentPhase = currentPhase;
                                triggerscript.newPhase = 1;
                            break;
                        }
                    break;
                }
                    
            }
            triggeredTriggerFound = false;
        }
            
    }

    //Updates the triggers with the current phase after possession/depossession happens.
    public void UpdatePhase() {
        currentPhase = bossManager.phases;

        foreach (GameObject go in triggers) {
            WyvernPhaseTriggers triggerScript = go.GetComponent<WyvernPhaseTriggers>();

            switch (currentPhase) {
            //Fireball
            case 1:
                switch (triggerScript.triggerType) {
                    case WyvernPhaseTriggers.TriggerType.FireballAndMagicCircle:
                        triggerScript.newPhase = 2;
                    break;
                    case WyvernPhaseTriggers.TriggerType.FlamethrowerAndFireball:
                        triggerScript.newPhase = 3;
                    break;
                }
            break;
            //Magic Circle
            case 2:
                switch (triggerScript.triggerType) {
                    case WyvernPhaseTriggers.TriggerType.FireballAndMagicCircle:
                        triggerScript.newPhase = 1;
                    break;
                    case WyvernPhaseTriggers.TriggerType.MagicCircleAndFlameThrower:
                        triggerScript.newPhase = 3;
                    break;
                }
            break;
            //Flamethrower
            case 3:
                switch (triggerScript.triggerType) {
                    case WyvernPhaseTriggers.TriggerType.MagicCircleAndFlameThrower:
                        triggerScript.newPhase = 2;
                    break;
                    case WyvernPhaseTriggers.TriggerType.FlamethrowerAndFireball:
                        triggerScript.newPhase = 1;
                    break;
                }
            break;
        }
        }
        
    }
}
