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
            arachnophobiaSetting = false;   
        }
        else {
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
        Debug.Log("A setting "+ arachnophobiaSetting);
    }

    public void ToggleArachnophobia(bool toggleOn) {
        //If true, changes game object to arachnophobia boss.
        if (toggleOn) {
            Instantiate(arachnophobiaBoss,transform.position,Quaternion.identity);
            //PlayerPrefs.SetInt("ArachnophobiaToggleState",1);
            Debug.Log("Changed " + arachnophobiaSetting);
            Destroy(gameObject);
        }
        else {
            Instantiate(defaultBoss,transform.position,Quaternion.identity);
            //PlayerPrefs.SetInt("ArachnophobiaToggleState",0);
            Debug.Log("Changed " + arachnophobiaSetting);
            Destroy(gameObject);
        }
    }
}
