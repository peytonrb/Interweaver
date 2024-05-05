using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossScript : MonoBehaviour
{
    public GameObject arachnophobiaBoss;
    public GameObject defaultBoss;
    [HideInInspector] public bool arachnophobiaSetting;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneHandler.instance.arachnophobiaState) {
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

    // Update is called once per frame
    void Update()
    {
        
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
