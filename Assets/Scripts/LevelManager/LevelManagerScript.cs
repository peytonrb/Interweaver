using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManagerScript : MonoBehaviour
{
    public static LevelManagerScript instance;
    public bool cannonSectionIsActive;
    private GameObject[] cannons;
    private NetCannonScript netCannonScript;
    private LevelTriggerScript levelTriggerScript;

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

        if (cannonSectionIsActive) {
            for (int i = 0; i < cannons.Length; i++) {
                netCannonScript = cannons[i].GetComponent<NetCannonScript>();

                netCannonScript.isOn = true;
            }
            Debug.Log("Cannons enabled");
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
        }
    }
}
