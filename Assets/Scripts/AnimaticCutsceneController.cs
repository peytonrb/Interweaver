using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimaticCutsceneController : MonoBehaviour
{
    public GameObject[] panels;
    private CanvasGroup panelsCG;
    public float[] panelTimes;
    public float transitionSpeed;
    private bool playingPanel;
    private int currentPanel;
    private bool transitioning;

    // Start is called before the first frame update
    void Start()
    {
        playingPanel = false;
        transitioning = false;
        currentPanel = 1;

        for (int i = 0; i < panels.Length; i++) {
            panelsCG = panels[i].GetComponent<CanvasGroup>();

            //Only sets visibility of the first panel
            if (i > 0) {
                panelsCG.alpha = 0;
            }
            else {
                panelsCG.alpha = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playingPanel) {
            PlayPanel();
        }
        else {
            Transition();
        }
    }

    void Transition() {
        if (transitioning == false) {
            panelsCG = panels[currentPanel].GetComponent<CanvasGroup>();
            transitioning = true;
        }
        else {
            panelsCG.alpha = Mathf.MoveTowards(panelsCG.alpha, 1, transitionSpeed * Time.deltaTime);

            if (panelsCG.alpha >= 1 && currentPanel < panels.Length - 1) {
                panelsCG.alpha = 1;
                playingPanel = true;
                transitioning = false;
            } 
            else {
                if (panelsCG.alpha >= 1) {
                    //SceneManager.LoadScene("AlpineCombined");
                    Debug.Log("Cutscene Completed");
                }
            }
        }
          
    }

    void PlayPanel() {
        if (panelTimes[currentPanel] > 0) {
            panelTimes[currentPanel] -= Time.deltaTime;
        }
        else {
            currentPanel += 1;
            playingPanel = false;
        }
    }
}
