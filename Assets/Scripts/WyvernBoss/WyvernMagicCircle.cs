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


    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");

        pushUp = false;
        stayUp = false;
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
                    Destroy(gameObject);
                }
            }
        }
    }

    void Warning(float number) {
        warning.SetActive(true);
        if (number <= 0) {
            Destroy(warning);
            pushUp = true;
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
            PlayerController player = weaver.GetComponent<PlayerController>();
            player.Death();
        }
    }

    public void OnTrigExit(Collider other) {

    }
}
