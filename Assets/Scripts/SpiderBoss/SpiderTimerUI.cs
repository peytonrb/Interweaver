using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderTimerUI : MonoBehaviour
{
    private bool spiderTimerStarted;
    private float timer;
    public float timerMaxValue; //Timermaxvalue should be 180
    private Slider healthSlider;
    private GameObject weaver;
    private PlayerController playercontroller;
    private bool playerdied;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        weaver = GameObject.FindGameObjectWithTag("Player");
        playercontroller = weaver.GetComponent<PlayerController>();

        timer = timerMaxValue;
        healthSlider.maxValue = timerMaxValue;
        healthSlider.value = healthSlider.maxValue;
        spiderTimerStarted = true;
        playerdied = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (spiderTimerStarted) {
            if (timer <= 0f) {
                KillPlayer();
            }
            else {
                timer -= Time.deltaTime;
                UpdateTime();
            }
        }
    }

    void KillPlayer() {
        if (!playerdied) {
            playercontroller.Death();
            playerdied = true;
        }
        if (playercontroller.isDead == false) {
            timer = timerMaxValue;
            playerdied = false;
        }
    }

    void UpdateTime() {
        healthSlider.value = timer;
    }

    public void StartSpiderTimer() {
        spiderTimerStarted = true;
    }

    public void AddTime(float time) {
        timer += time;
        UpdateTime();
    }

    public void SubtractTime(float time) {
        timer -= time;
        UpdateTime();
    }
}
