using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernMagicCircle : MonoBehaviour, ITriggerable
{
    public GameObject warning;
    private GameObject weaver;
    [SerializeField] private float timer;
    private bool pushUp;
    private bool stayUp;
    [SerializeField] private float height;
    [SerializeField] private float riseSpeed;

    //Concerning magic circles inside a predetermined configuration. They are always attached to a configuration parent object.
    private GameObject configurationParent;
    private WyvernBossManager bossManager;
    private bool hasParent;


    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
        bossManager = GameObject.FindGameObjectWithTag("Boss").GetComponent<WyvernBossManager>();

        pushUp = false;
        stayUp = false;
        warning.SetActive(true);

        if (bossManager.useConfigurations) {
            hasParent = true;
            configurationParent = transform.parent.gameObject;
            bossManager.configurationIsActive = true;
        }

        StartCoroutine(Flashing());
    }

    // Update is called once per frame
    void Update()
    {
        if (pushUp == false) {
            timer -= Time.deltaTime;
            Warning(timer);
        }
        else {
            if (stayUp == false) {
                PushUp();
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

    void Warning(float number) {
        if (number <= 0) {
            pushUp = true;
        }
    }

    IEnumerator Flashing() {
        while (warning.activeSelf) {
            yield return new WaitForSeconds(0.2f);
            warning.SetActive(false);
        }
        while (!warning.activeSelf) {
            yield return new WaitForSeconds(0.2f);
            warning.SetActive(true);
        }
        if (pushUp == false) {
            StartCoroutine(Flashing());
        }
        else {
            warning.SetActive(false);
            yield break;
        }
    }

    void PushUp() {
        float newpos = Mathf.MoveTowards(transform.position.y, height, riseSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x,newpos,transform.position.z);

        if (transform.position.y >= height) {
            stayUp = true;
            timer = 2.0f;
        }
    }

    public void OnTrigEnter(Collider other) {
        if (other.gameObject.CompareTag("Hazard")) {
            PlayerControllerNew player = weaver.GetComponent<PlayerControllerNew>();
            player.Death();
        }
    }

    public void OnTrigExit(Collider other) {

    }
}
