using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimaticCutsceneController : MonoBehaviour
{
    public static int cutscene = 0;
    [SerializeField] private bool isOnTrigger;
    public GameObject[] openingCutscenePanels;
    public GameObject[] closingCutscenePanels;
    private CanvasGroup panelsCG;
    public float[] panelTimes;
    public float transitionSpeed;
    private bool playingPanel;
    private int currentPanel;
    private bool transitioning;

    // Start is called before the first frame update
    void Start()
    {
        if (!isOnTrigger) {
            playingPanel = false;
            transitioning = false;
            currentPanel = 1;

            switch (cutscene) {
                case 0:
                    //Opening cutscene
                    for (int i = 0; i < openingCutscenePanels.Length; i++) {
                        panelsCG = openingCutscenePanels[i].GetComponent<CanvasGroup>();

                        //Only sets visibility of the first panel
                        if (i > 0) {
                            panelsCG.alpha = 0;
                        }
                        else {
                            panelsCG.alpha = 1;
                        }
                    }

                    foreach (GameObject objects in closingCutscenePanels) {
                        objects.SetActive(false);
                    }
                break;
                case 1:
                    //Closing cutscene
                    for (int i = 0; i < closingCutscenePanels.Length; i++) {
                        panelsCG = closingCutscenePanels[i].GetComponent<CanvasGroup>();

                        //Only sets visibility of the first panel
                        if (i > 0) {
                            panelsCG.alpha = 0;
                        }
                        else {
                            panelsCG.alpha = 1;
                        }
                    }

                    foreach (GameObject objects in openingCutscenePanels) {
                        objects.SetActive(false);
                    }
                break;
            } 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOnTrigger) {
            if (playingPanel) {
                PlayPanel();
            }
            else {
                Transition();
            }
        }
        
    }

    void Transition() {
        switch (cutscene) {
            case 0:
                if (transitioning == false) {
                    panelsCG = openingCutscenePanels[currentPanel].GetComponent<CanvasGroup>();
                    transitioning = true;
                }
                else {
                    panelsCG.alpha = Mathf.MoveTowards(panelsCG.alpha, 1, transitionSpeed * Time.deltaTime);

                    if (panelsCG.alpha >= 1 && currentPanel < openingCutscenePanels.Length - 1) {
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
            break;
            case 1:
                if (transitioning == false) {
                    panelsCG = closingCutscenePanels[currentPanel].GetComponent<CanvasGroup>();
                    transitioning = true;
                }
                else {
                    panelsCG.alpha = Mathf.MoveTowards(panelsCG.alpha, 1, transitionSpeed * Time.deltaTime);

                    if (panelsCG.alpha >= 1 && currentPanel < closingCutscenePanels.Length - 1) {
                        panelsCG.alpha = 1;
                        playingPanel = true;
                        transitioning = false;
                    } 
                    else {
                        if (panelsCG.alpha >= 1) {
                            //SceneManager.LoadScene("MainMenu");
                            Debug.Log("Cutscene Completed");
                        }
                    }
                }
            break;
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

    public void ChangeCutscene(int scene) {
        cutscene = scene;
    }
}
