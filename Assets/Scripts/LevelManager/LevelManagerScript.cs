using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : MonoBehaviour
{
    public static LevelManagerScript instance;
    public bool cannonSectionIsActive;
    public bool stalactiteSectionIsActive;
    public bool mushroomSectionIsActive;
    private GameObject[] cannons;
    private GameObject[] stalactites;
    private GameObject[] glowMushrooms;
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
        glowMushrooms = GameObject.FindGameObjectsWithTag("GlowMushrooms");

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

        if (mushroomSectionIsActive) {
            for (int i = 0; i < glowMushrooms.Length; i++) {
                TimedGlowMushroomsScript glowMushroomsScript = glowMushrooms[i].GetComponent<TimedGlowMushroomsScript>();

                glowMushroomsScript.isActive = true;
            }
        }
        
    }

    public void TurnOnOffSection(int section) {

        switch (section) {
            //Turn on/off PROJECTILE CANNONS
            case 0:
                for (int i = 0; i < cannons.Length; i++) {
                    netCannonScript = cannons[i].GetComponent<NetCannonScript>();

                    if (netCannonScript.isOn) {
                        netCannonScript.isOn = false;
                    }
                    else {
                        netCannonScript.isOn = true;
                    }
                    
                }    
            break;

            case 1:
                if (spikeArea != null) {
                    spikeArea.SetActive(false);
                }
            break;

            case 2:
                for (int i = 0; i < stalactites.Length; i++) {
                    StalactiteSpawnerScript sss = stalactites[i].GetComponent<StalactiteSpawnerScript>();

                    if (sss.canFall) {
                        sss.canFall = false;
                    }
                    else {
                        sss.canFall = true;
                    }
                }
            break;

            case 3:
            for (int i = 0; i < glowMushrooms.Length; i++) {
                TimedGlowMushroomsScript mushroomsScript = glowMushrooms[i].GetComponent<TimedGlowMushroomsScript>();

                if (mushroomsScript.isActive) {
                    mushroomsScript.isActive = false;
                }
                else {
                    mushroomsScript.isActive = true;
                }

                }
            break;
        }
    }
}
