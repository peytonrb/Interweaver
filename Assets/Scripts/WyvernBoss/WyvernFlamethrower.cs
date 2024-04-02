using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFlamethrower : MonoBehaviour
{
    private GameObject wyvern;
    private WyvernBossManager wyvernBossManager;
    private GameObject phaseTriggerManager;
    private WyvernPhaseTriggerManager wptm;
    [SerializeField] private bool useDefaultPositionAndScale;
    [SerializeField] private Vector3 flamethrowerLocalPosition;
    [SerializeField] private Vector3 flamethrowerLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        wyvern = GameObject.FindGameObjectWithTag("Boss");
        wyvernBossManager = wyvern.GetComponent<WyvernBossManager>();
        phaseTriggerManager = wyvernBossManager.wyvernTriggerManager;
        wptm = phaseTriggerManager.GetComponent<WyvernPhaseTriggerManager>();

        //DEFAULT LOCAL POSITION AND SCALES
        if (useDefaultPositionAndScale == true) {
            transform.localPosition = new Vector3(0,-0.47f,1.3f);
            transform.localScale = new Vector3(0.05f,0.05f,2f);
        }
        else {
            transform.localPosition = flamethrowerLocalPosition;
            transform.localScale = flamethrowerLocalScale;
        }
        
    }

    public void KillThyself() {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerControllerNew playerController = other.gameObject.GetComponent<PlayerControllerNew>();
            playerController.Death();
            wptm.UpdateTriggersOnDeath(true);
        }
        if (other.gameObject.CompareTag("Familiar")) {
            FamiliarScript familiarScript = other.gameObject.GetComponent<FamiliarScript>();
            familiarScript.Death();
            wptm.UpdateTriggersOnDeath(false);
        }
    }
}
