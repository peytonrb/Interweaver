using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernMagicCircleGroundLevelReset : MonoBehaviour
{
    private GameObject wyvern;
    private WyvernBossManager bossManager;

    void Start() {
        wyvern = GameObject.FindGameObjectWithTag("Boss");
        bossManager = wyvern.GetComponent<WyvernBossManager>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            bossManager.ResetPhase2GroundLevel(true);
        }
        if (other.gameObject.CompareTag("Familiar")) {
            bossManager.ResetPhase2GroundLevel(false);
        }
    }
}
