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
    public float[] openingCutscenePanelTimes;
    public float[] closingCutscenePanelTimes;
    public AudioClip[] openingCutsceneAudio;
    public AudioClip[] closingCutsceneAudio;
    private AudioSource audioSource;
    public float transitionSpeed;
    private bool playingPanel;
    private int currentPanel;
    private bool transitioning;
    private bool playedAudio;
    public bool debugCutsceneChanger;

    // Start is called before the first frame update
    void Start()
    {
        if (!isOnTrigger) {
            playingPanel = false;
            transitioning = false;
            playedAudio = false;
            currentPanel = 1;
            audioSource = GetComponent<AudioSource>();
            if (debugCutsceneChanger == true) {
                cutscene = 1;
            }
            else {
                cutscene = 0;
            }

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
                            SceneManager.LoadScene("AlpineCombined");
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
                            SceneManager.LoadScene("Menu");
                            Debug.Log("Cutscene Completed");
                        }
                    }
                }
            break;
        }
          
    }

    void PlayPanel() {
        switch (cutscene) {
            case 0:
                if (openingCutscenePanelTimes[currentPanel] > 0) {
                    openingCutscenePanelTimes[currentPanel] -= Time.deltaTime;
                    if (!playedAudio) {
                        PlayAudio(openingCutsceneAudio[currentPanel]);
                        playedAudio = true;
                    }
                }
                else {
                    currentPanel += 1;
                    playingPanel = false;
                    playedAudio = false;
                }
            break;
            case 1:
                if (closingCutscenePanelTimes[currentPanel] > 0) {
                    closingCutscenePanelTimes[currentPanel] -= Time.deltaTime;
                    if (!playedAudio) {
                        PlayAudio(closingCutsceneAudio[currentPanel]);
                        playedAudio = true;
                    }
                }
                else {
                    currentPanel += 1;
                    playingPanel = false;
                    playedAudio = false;
                }
            break;
        }
        
    }

    public void ChangeCutscene(int scene) {
        cutscene = scene;
    }

    void PlayAudio(AudioClip audio) {
        audioSource.PlayOneShot(audio);
    }
}
