using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManagerScript : MonoBehaviour
{
    public CinemachineVirtualCamera[] cutsceneCams;
    public GameObject cutsceneTrigger;
    [SerializeField] private GameObject skipCutsceneTextKeyboard;
    //[SerializeField] private GameObject skipCutsceneTextGamepad;
    private bool skipped;
    private bool isCutscene;
    private int cutscenePhase;
    private PlayableDirector director;
    public bool playOnStart;

    [Header("Player Objects")]
    public GameObject player;
    private MovementScript playerMovementScript;
    public bool usingCutsceneWeaver;
    public GameObject cutsceneWeaver;
    private DummyWeaverScript dummyScript;
    public bool isSkippable = true;

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

    void Awake() {
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(false);
            dummyScript = cutsceneWeaver.GetComponent<DummyWeaverScript>();
        }
        if (useTransitions) {
            cutsceneCanvas.SetActive(false);
            bpCanvasGroup = blackPanel.GetComponent<CanvasGroup>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        playerMovementScript = player.GetComponent<MovementScript>();
        director = GetComponent<PlayableDirector>();
        
        debugisOn = DebugManager.instance.GetDebugOn(); //Gets if debug features are on

        isCutscene = false;
        isTransitioning = false;
        skipped = false;
        cutscenePhase = 0;
        if (skipCutsceneTextKeyboard != null) {
            skipCutsceneTextKeyboard.SetActive(false);
            //skipCutsceneTextGamepad.SetActive(false);
        }
        

        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 0;
        }

        if (playOnStart == true) {
            StartCutscene();
        }

        if (cutsceneCanvas != null && !isSkippable)
            cutsceneCanvas.transform.GetChild(1).gameObject.SetActive(false);
        else
            cutsceneCanvas.transform.GetChild(1).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCutscene) {
            if (useTransitions) {
                switch (cutscenePhase) {
                    case 0:
                        //If play on start is true, cutscene will fade in from black instead of out and then back in.
                        if (playOnStart) {
                            BeginTimeline();
                            if (useTransitions) {
                                bpCanvasGroup.alpha = 1;
                                isTransitioning = true;
                            }
                        }
                        else {
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
                                InputManagerScript.instance.insideCutscene = false;                                
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

            if (InputManagerScript.instance.stopCutscene == true && isSkippable) {
                skipped = true;
                EndCutscene();
            }

            /*
            if (debugisOn) {
                if (Input.GetKeyDown(KeyCode.F)) {
                    Debug.Log("Debug is on");
                    EndCutscene();
                }
            } 
            */  
        }
        
    }

    public void StartCutscene() {
        isCutscene = true;
        playerMovementScript.inCutscene = true;
        InputManagerScript.instance.insideCutscene = true;
        if (useTransitions) {
            cutsceneCanvas.SetActive(true);
        }
            
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(true);
        }
    }

    private void BeginTimeline() {
        director.Play();

        if (skipCutsceneTextKeyboard != null) {
            skipCutsceneTextKeyboard.SetActive(true);
        }
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(true); 
        }
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 10;
        }
        cutscenePhase += 1;
    }

    private void EndCutscene() {
        InputManagerScript.instance.insideCutscene = false;
        Destroy(cutsceneTrigger);
        director.Stop();

        if (playOnStart)
        playerMovementScript.UnfreezePlayer();

        if (skipCutsceneTextKeyboard != null) {
            skipCutsceneTextKeyboard.SetActive(false);
        }
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 0;  
        }
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(false);
        }
        
        playerMovementScript.inCutscene = false;

        if (skipped == true) {
            isTransitioning = false;
            bpCanvasGroup.alpha = 0;
            cutscenePhase = 0;
            isCutscene = false;
            InputManagerScript.instance.insideCutscene = false;
            skipped = false;
        }
        else {
            cutscenePhase += 1;
        }
        
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
