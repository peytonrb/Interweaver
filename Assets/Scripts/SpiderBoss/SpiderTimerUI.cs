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
    private PlayerControllerNew playercontroller;
    private bool playerdied;
    public AnimationCurve screenShake;
    private float intensitytime;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        weaver = GameObject.FindGameObjectWithTag("Player");
        playercontroller = weaver.GetComponent<PlayerControllerNew>();

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
            if (timer <= 15f) {
                intensitytime += Time.deltaTime;
                float screenshakeintensity = screenShake.Evaluate(intensitytime/15);
                CameraMasterScript.instance.ShakeCurrentCamera(screenshakeintensity,0.2f,0.1f);
                timer -= Time.deltaTime;
                UpdateTime();
            }
            else {
                timer -= Time.deltaTime;
                UpdateTime();
            }
        }
    }

    void KillPlayer() {
        if (!playerdied) {
            Debug.Log("death happened!");
            playercontroller.Death();
            playerdied = true;
        }
        if (playercontroller.isDead == false) {
            intensitytime = 0;
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
        intensitytime = 0;
        timer += time;
        if (timer > timerMaxValue) {
            timer = timerMaxValue;
        }
        UpdateTime();
    }

    public void SubtractTime(float time) {
        timer -= time;
        if (timer < 0) {
            timer = 0;
        }
        UpdateTime();
    }
}
