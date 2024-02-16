using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossScript : MonoBehaviour
{
    public GameObject arachnophobiaBoss;
    public GameObject defaultBoss;
    [HideInInspector] public bool arachnophobiaSetting;
    private int toggleState;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("ArachnophobiaToggleState")) {
            toggleState = PlayerPrefs.GetInt("ArachnophobiaToggleState");
        }
        if (toggleState == 0) {
            arachnophobiaBoss.SetActive(false);
            defaultBoss.SetActive(true);
            arachnophobiaSetting = false;   
        }
        else {
            arachnophobiaBoss.SetActive(true);
            defaultBoss.SetActive(false);
            arachnophobiaSetting = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.X)) {
            if (arachnophobiaSetting == false) {
                ToggleArachnophobia(true);
            }
            else {
                ToggleArachnophobia(false);
            }
        }
        */
        //Debug.Log("A setting "+ arachnophobiaSetting);
    }

    public void ToggleArachnophobia(bool toggleOn) {
        //If true, changes game object to arachnophobia boss.
        if (toggleOn) {
            arachnophobiaBoss.SetActive(true);
            defaultBoss.SetActive(false);
            arachnophobiaSetting = true;
        }
        else {
            arachnophobiaBoss.SetActive(false);
            defaultBoss.SetActive(true);
            arachnophobiaSetting = false;
        }
    }
}
