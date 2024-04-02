using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernMagicCircle : MonoBehaviour
{
    //public GameObject warning;
    private MagicCircleVFXController vfxScript;
    [SerializeField] private float timer;
    private bool pushUp;
    private bool stayUp;
    [SerializeField] private float height;
    private float newheight;
    [SerializeField] private float riseSpeed;

    //Concerning magic circles inside a predetermined configuration. They are always attached to a configuration parent object.
    private GameObject configurationParent;
    private WyvernBossManager bossManager;
    private GameObject phaseTriggerManager;
    private WyvernPhaseTriggerManager wptm;
    private bool hasParent;
    private int magicCircleID;


    // Start is called before the first frame update
    void Start()
    {
        bossManager = GameObject.FindGameObjectWithTag("Boss").GetComponent<WyvernBossManager>();
        phaseTriggerManager = bossManager.wyvernTriggerManager;
        wptm = phaseTriggerManager.GetComponent<WyvernPhaseTriggerManager>();
        vfxScript = this.GetComponent<MagicCircleVFXController>();

        pushUp = false;
        stayUp = false;
        //warning.SetActive(true);
        newheight = transform.position.y + height + 5;
        magicCircleID = bossManager.magicCircleAmount;

        if (bossManager.useConfigurations) {
            hasParent = true;
            configurationParent = transform.parent.gameObject;
            bossManager.configurationIsActive = true;
        }

        //StartCoroutine(Flashing());
    }

    // Update is called once per frame
    void Update()
    {
        if (pushUp == false) {
            timer -= Time.deltaTime;
            Warning();
        }
        else {
            if (stayUp == false) {
                //PushUp();
            }
            else {
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    StopAllCoroutines();
                    if (hasParent && configurationParent != null) {
                        bossManager.configurationIsActive = false;
                        bossManager.reseting = true;
                        Destroy(configurationParent);
                    }
                    else {
                        Destroy(gameObject);
                    }   
                }
            }
        }
    }

    void Warning() {
        // if (number <= 0) {
        //     pushUp = true;
        // }
        
        if (!vfxScript.activateVFX)
            vfxScript.activateVFX = true;
    }

    // IEnumerator Flashing() {
    //     while (warning.activeSelf) {
    //         yield return new WaitForSeconds(0.2f);
    //         warning.SetActive(false);
    //     }
    //     while (!warning.activeSelf) {
    //         yield return new WaitForSeconds(0.2f);
    //         warning.SetActive(true);
    //     }
    //     if (pushUp == false) {
    //         StartCoroutine(Flashing());
    //     }
    //     else {
    //         warning.SetActive(false);
    //         yield break;
    //     }
    // }

    // void PushUp() {
    //     float newpos = Mathf.MoveTowards(transform.position.y, newheight, riseSpeed * Time.deltaTime);
    //     transform.position = new Vector3(transform.position.x,newpos,transform.position.z);

    //     if (transform.position.y >= newheight) {
    //         stayUp = true;
    //         timer = 2.0f;
    //     }
    // }

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
