using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : MonoBehaviour
{
    public static LevelManagerScript instance;
    public bool cannonSectionIsActive;
    public bool stalactiteSectionIsActive;
    private GameObject[] cannons;
    private GameObject[] stalactites;
    private NetCannonScript netCannonScript;
    private LevelTriggerScript levelTriggerScript;
    public GameObject spikeArea;

    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cannons = GameObject.FindGameObjectsWithTag("Cannons");
        stalactites = GameObject.FindGameObjectsWithTag("StalactiteSpawner");

        if (cannonSectionIsActive) {
            for (int i = 0; i < cannons.Length; i++) {
                netCannonScript = cannons[i].GetComponent<NetCannonScript>();

                netCannonScript.isOn = true;
            }
        }

        if (stalactiteSectionIsActive) {
            for (int i = 0; i < stalactites.Length; i++) {
                StalactiteSpawnerScript stalactiteScript = stalactites[i].GetComponent<StalactiteSpawnerScript>();

                stalactiteScript.canFall = true;
            }
        }
        
    }

    public void TurnOnOffSection(int section) {
        levelTriggerScript = GetComponentInChildren<LevelTriggerScript>();

        switch (section) {
            //Turn on/off PROJECTILE CANNONS
            case 0:
                if (levelTriggerScript.triggered) {
                    for (int i = 0; i < cannons.Length; i++) {
                        netCannonScript = cannons[i].GetComponent<NetCannonScript>();

                        netCannonScript.isOn = true;
                        Debug.Log("Cannons enabled");
                    }

                    levelTriggerScript.triggered = false;
                }
                else {
                    for (int i = 0; i < cannons.Length; i++) {
                        netCannonScript = cannons[i].GetComponent<NetCannonScript>();

                        netCannonScript.isOn = false;
                        Debug.Log("Cannons disabled");
                    }

                    levelTriggerScript.triggered = true;
                }
            break;
            case 1:
                if (spikeArea != null) {
                    spikeArea.SetActive(false);
                }
            break;
            case 2:
                if (levelTriggerScript.triggered) {
                    for (int i = 0; i < stalactites.Length; i++) {
                        StalactiteSpawnerScript sss = stalactites[i].GetComponent<StalactiteSpawnerScript>();

                        if (sss.canFall) {
                            sss.canFall = false;
                        }
                        else {
                            sss.canFall = true;
                        }
                    }

                    levelTriggerScript.triggered = false;

                }
                else {
                    for (int i = 0; i < stalactites.Length; i++) {
                        StalactiteSpawnerScript sss = stalactites[i].GetComponent<StalactiteSpawnerScript>();
                        
                        if (sss.canFall) {
                            sss.canFall = false;
                        }
                        else {
                            sss.canFall = true;
                        }
                    }

                    levelTriggerScript.triggered = true;
                }
            break;
        }
    }
}
