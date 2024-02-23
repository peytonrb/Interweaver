using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTriggerScript : MonoBehaviour
{
    
    [Tooltip ("0 is for Projectile Cannons, 1 is for Breakeable Spikes, 2 is for stalactites")]
    public int triggerType;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar")) {
            LevelManagerScript.instance.TurnOnOffSection(triggerType);
        }
    }

}
