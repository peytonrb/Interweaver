using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject spiderBossCanvas;
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            spiderBossCanvas.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
