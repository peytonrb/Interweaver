using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutsceneManagerScript : MonoBehaviour
{
    public CinemachineVirtualCamera[] cutsceneCams;
    public GameObject cutsceneTrigger;
    [SerializeField] private GameObject skipCutsceneText;
    private bool isCutscene;
    private int cutscenePhase;
    private PlayableDirector director;

    [Header("Player Objects")]
    public GameObject player;
    private MovementScript playerMovementScript;
    public bool usingCutsceneWeaver;
    public GameObject cutsceneWeaver;
    private DummyWeaverScript dummyScript;

    [Header ("Transitions")]
    [SerializeField] private bool useTransitions; //For using fade ins and fade outs
    [SerializeField] private float transitionSpeed;
    private bool isTransitioning;
    public GameObject cutsceneCanvas;
    public GameObject blackPanel;
    private CanvasGroup bpCanvasGroup;
    public float blackedoutPauseDuration = 0;
    private bool debugisOn;
    private bool startedPause = false;

    // Start is called before the first frame update
    void Start()
    {
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(false);
            dummyScript = cutsceneWeaver.GetComponent<DummyWeaverScript>();
        }
        if (useTransitions) {
            cutsceneCanvas.SetActive(false);
            bpCanvasGroup = blackPanel.GetComponent<CanvasGroup>();
        }
        
        playerMovementScript = player.GetComponent<MovementScript>();
        director = GetComponent<PlayableDirector>();
        
        debugisOn = DebugManager.instance.GetDebugOn(); //Gets if debug features are on

        isCutscene = false;
        isTransitioning = false;
        cutscenePhase = 0;
        skipCutsceneText.SetActive(false);

        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCutscene) {
            if (useTransitions) {
                switch (cutscenePhase) {
                    case 0:
                        if (!isTransitioning) {
                            isTransitioning = true;
                        }
                        else {
                            bpCanvasGroup.alpha = Mathf.MoveTowards(bpCanvasGroup.alpha, 1f, transitionSpeed * Time.deltaTime);
                            if (bpCanvasGroup.alpha >= 1) {
                                if (blackedoutPauseDuration == 0)
                                {
                                    BeginTimeline();
                                    bpCanvasGroup.alpha = 1;
                                }
                                else if (!startedPause)
                                {
                                    StartCoroutine(CutsceneDelay());
                                    startedPause = true;
                                }
                                
                            }
                        }
                    break;
                    case 1:
                        if (isTransitioning) {
                            bpCanvasGroup.alpha = Mathf.MoveTowards(bpCanvasGroup.alpha, 0f, transitionSpeed * Time.deltaTime);
                            if (bpCanvasGroup.alpha <= 0) {
                                isTransitioning = false;
                                bpCanvasGroup.alpha = 0;
                            }
                        }
                        else {
                            if (director.time >= director.duration - 2) {
                                cutscenePhase = 2;
                            }
                        }
                    break;
                    case 2:
                        if (!isTransitioning) {
                            isTransitioning = true;
                        }
                        else {
                            bpCanvasGroup.alpha = Mathf.MoveTowards(bpCanvasGroup.alpha, 1f, transitionSpeed * Time.deltaTime);
                            
                            if (bpCanvasGroup.alpha >= 1) {

                                if (blackedoutPauseDuration == 0)
                                {
                                    bpCanvasGroup.alpha = 1;
                                    EndCutscene();
                                }
                                else if (!startedPause)
                                {
                                    StartCoroutine(CutsceneDelay());
                                    startedPause = true;
                                }

                            }
                            
                        }
                    break;
                    case 3:
                        if (isTransitioning) {
                            bpCanvasGroup.alpha = Mathf.MoveTowards(bpCanvasGroup.alpha, 0f, transitionSpeed * Time.deltaTime);
                            
                            if (bpCanvasGroup.alpha <= 0) {
                                isTransitioning = false;
                                bpCanvasGroup.alpha = 0;
                                cutscenePhase = 0;
                                isCutscene = false;
                            }
                        }
                    break;
                }
            }

            //For if not using transitions
            else {
                switch (cutscenePhase) {
                    case 0:
                        BeginTimeline();
                    break;
                    case 1:
                        if (director.time >= director.duration - 2) {
                            cutscenePhase = 2;
                        }
                    break;
                    case 2:
                        EndCutscene();
                    break;
                    case 3:
                        cutscenePhase = 0;
                        isCutscene = false;
                    break;
                }
            }

            if (InputManagerScript.instance.stopCutscene == true) {
                EndCutscene();
            }

            if (debugisOn) {
                if (Input.GetKeyDown(KeyCode.F)) {
                    EndCutscene();
                }
            }   
        }
        
    }

    public void StartCutscene() {
        isCutscene = true;
        playerMovementScript.inCutscene = true;
        if (useTransitions) {
            cutsceneCanvas.SetActive(true);
        }
            
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(true);
        }
    }

    private void BeginTimeline() {
        director.Play();
        skipCutsceneText.SetActive(true);
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(true); 
        }
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 10;
        }
        cutscenePhase += 1;
    }

    private void EndCutscene() {
        Destroy(cutsceneTrigger);
        director.Stop();
        skipCutsceneText.SetActive(false);
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 0;  
        }
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(false);
        }
        
        playerMovementScript.inCutscene = false;

        cutscenePhase += 1;
    }

    public IEnumerator CutsceneDelay()
    {
        foreach (CinemachineVirtualCamera vcam in cutsceneCams)
        {
            vcam.Priority = 10;
        }

        yield return new WaitForSeconds(blackedoutPauseDuration);

        bpCanvasGroup.alpha = 1;

        if (cutscenePhase == 0)
        {
            BeginTimeline();    
        }
        else
        {
            EndCutscene();
        }

        startedPause = false;
        yield break;
    }

}
