using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderlingScript : MonoBehaviour
{
    [SerializeField] private GameObject arachnophobiaSpider;
    [SerializeField] private GameObject defaultSpider;
    [SerializeField] private GameObject pauseScreen;
    private PauseScript pauseScript;
    private int toggleState;

    // Start is called before the first frame update
    void Start()
    {
        pauseScript = pauseScreen.GetComponent<PauseScript>();

        if (PlayerPrefs.HasKey("ArachnophobiaToggleState")) {
            toggleState = PlayerPrefs.GetInt("ArachnophobiaToggleState");
        }
        if (toggleState == 0) {
            arachnophobiaSpider.SetActive(false);
            defaultSpider.SetActive(true); 
        }
        else {
            arachnophobiaSpider.SetActive(true);
            defaultSpider.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseScript.changeSpiderlings == true) {
            if (pauseScript.changeSpiderlingsTo == true) {
                ToggleArachnophobia(true);
                pauseScript.changeSpiderlings = false;
            }
            else {
                ToggleArachnophobia(false);
                pauseScript.changeSpiderlings = false;
            }
        }
    }

    public void ToggleArachnophobia(bool toggleOn) {
        //If true, changes game object to arachnophobia boss.
        if (toggleOn) {
            arachnophobiaSpider.SetActive(true);
            defaultSpider.SetActive(false);
        }
        else {
            arachnophobiaSpider.SetActive(false);
            defaultSpider.SetActive(true);
        }
    }
}
