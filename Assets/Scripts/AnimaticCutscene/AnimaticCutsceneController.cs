using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimaticCutsceneController : MonoBehaviour
{
    public static int cutscene = 0;
    [SerializeField] private bool isOnTrigger;
    public GameObject[] animaticCutscenes; //The cutscenes themselves
    private AnimaticInstanceScript animaticInstanceScript; //The scripts in the animatic cutscenes
    private GameObject[] animaticCutscenePanels; //The cutscene panels
    private float[] cutscenePanelTimes; //The cutscene panel times
    private AudioClip[] cutsceneAudio; //The cutscene audio clips
    private CanvasGroup panelsCG;
    private AudioSource audioSource;
    public float transitionSpeed;
    private bool playingPanel;
    private int currentPanel;
    private bool transitioning;
    private bool playedAudio;
    private bool debugisOn;

    // Start is called before the first frame update
    void Start()
    {
        if (!isOnTrigger) {
            playingPanel = false;
            transitioning = false;
            playedAudio = false;
            currentPanel = 1;
            audioSource = GetComponent<AudioSource>();
            debugisOn = DebugManager.instance.GetDebugOn();

            //Sets the current active cutscene and disables the others
            for(int i = 0; i < animaticCutscenes.Length; i++) {
                if (i == cutscene) {
                    animaticCutscenes[i].SetActive(true);
                    animaticInstanceScript = animaticCutscenes[i].GetComponent<AnimaticInstanceScript>();
                    animaticCutscenePanels = animaticInstanceScript.cutscenePanels;
                    cutscenePanelTimes = animaticInstanceScript.cutscenePanelTimes;
                    if (animaticInstanceScript.cutsceneAudio != null) {
                        cutsceneAudio = animaticInstanceScript.cutsceneAudio;
                    }
                    
                    //Set transition speed as well
                }
                else {
                    animaticCutscenes[i].SetActive(false);
                }
            }

            for (int i = 0; i < animaticCutscenePanels.Length; i++) {
                panelsCG = animaticCutscenePanels[i].GetComponent<CanvasGroup>();

                //Only sets visibility of the first panel
                if (i > 0) {
                    panelsCG.alpha = 0;
                }
                else {
                    panelsCG.alpha = 1;
                }
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

            if (debugisOn) {
                if (Input.GetKeyDown(KeyCode.F)) {
                    SceneManager.LoadScene("AlpineCombined");
                }
                if (Input.GetKeyDown(KeyCode.G)) {
                    SceneManager.LoadScene("Cavern");
                }
                if (Input.GetKeyDown(KeyCode.H)) {
                    SceneManager.LoadScene("Sepultus");
                }
                if (Input.GetKeyDown(KeyCode.J)) {
                    SceneManager.LoadScene("Courtyard");
                }
            }
        }   
    }

    void Transition() {
        if (transitioning == false) {
            panelsCG = animaticCutscenePanels[currentPanel].GetComponent<CanvasGroup>();
            transitioning = true;
        }
        else {
            panelsCG.alpha = Mathf.MoveTowards(panelsCG.alpha, 1, transitionSpeed * Time.deltaTime);

            if (panelsCG.alpha >= 1 && currentPanel < animaticCutscenePanels.Length - 1) {
                panelsCG.alpha = 1;
                playingPanel = true;
                transitioning = false;
            } 
            else {
                if (panelsCG.alpha >= 1) {
                    switch (cutscene) {
                        case 0:
                            SceneManager.LoadScene("Courtyard");
                        break;
                        case 1:
                            SceneManager.LoadScene("AlpineCombined");
                        break;
                        case 2:
                            SceneManager.LoadScene("Courtyard");
                        break;
                        case 3:
                            SceneManager.LoadScene("Cavern");
                        break;
                        case 4:
                            SceneManager.LoadScene("Courtyard");
                        break;
                        case 5:
                            SceneManager.LoadScene("Sepultus");
                        break;
                        case 6:
                            SceneManager.LoadScene("Menu");
                        break;
                    }
                }
            }
        }
    }

    void PlayPanel() {
        if (cutscenePanelTimes[currentPanel] > 0) {
            cutscenePanelTimes[currentPanel] -= Time.deltaTime;
            if (!playedAudio) {
                if (cutsceneAudio != null && cutsceneAudio.Length > 0) {
                    if (cutsceneAudio[currentPanel] != null) {
                        PlayAudio(cutsceneAudio[currentPanel]);
                    }
                } 
                
                playedAudio = true;
            }
        }
        else {
            currentPanel += 1;
            playingPanel = false;
            playedAudio = false;
        }
    }

    public void ChangeCutscene(int scene) {
        cutscene = scene;
    }

    void PlayAudio(AudioClip audio) {
        audioSource.PlayOneShot(audio);
    }
}
