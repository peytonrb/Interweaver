using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutsceneManagerScript : MonoBehaviour
{
    public GameObject cutsceneCanvas;
    public GameObject blackPanel;
    private CanvasGroup bpCanvasGroup;
    public CinemachineVirtualCamera[] cutsceneCams;
    private bool isCutscene;
    private bool isTransitioning;
    private int cutscenePhase;
    private BoxCollider bc;
    [SerializeField] private float transitionSpeed;
    private float lerpval;
    private PlayableDirector director;

    [Header("Player Objects")]
    public GameObject player;
    private MovementScript playerMovementScript;
    public bool usingCutsceneWeaver;
    public GameObject cutsceneWeaver;
    public GameObject cutsceneTrigger;

    // Start is called before the first frame update
    void Start()
    {
        cutsceneCanvas.SetActive(false);
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(false);
        }
        bpCanvasGroup = blackPanel.GetComponent<CanvasGroup>();
        playerMovementScript = player.GetComponent<MovementScript>();
        director = GetComponent<PlayableDirector>();
        bc = GetComponentInChildren<BoxCollider>();

        isCutscene = false;
        isTransitioning = false;
        cutscenePhase = 0;
        lerpval = 0;

        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCutscene) {
            switch (cutscenePhase) {
                case 0:
                    if (!isTransitioning) {
                        isTransitioning = true;
                    }
                    else {
                        bpCanvasGroup.alpha = Mathf.Lerp(bpCanvasGroup.alpha,1,lerpval);
                        lerpval += transitionSpeed * Time.deltaTime;
                        
                        if (lerpval >= 1) {
                            BeginTimeline();
                            bpCanvasGroup.alpha = 1;
                        }
                    }
                break;
                case 1:
                    if (isTransitioning) {
                        bpCanvasGroup.alpha = Mathf.Lerp(bpCanvasGroup.alpha,0,lerpval);
                        lerpval += -transitionSpeed * Time.deltaTime;

                        if (lerpval <= 0) {
                            isTransitioning = false;
                            bpCanvasGroup.alpha = 0;
                            lerpval = 0;
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
                        bpCanvasGroup.alpha = Mathf.Lerp(bpCanvasGroup.alpha,1,lerpval);
                        lerpval += transitionSpeed * Time.deltaTime;
                        
                        if (lerpval >= 1) {
                            bpCanvasGroup.alpha = 1;
                            EndCutscene();
                        }
                    }
                break;
                case 3:
                    if (isTransitioning) {
                        bpCanvasGroup.alpha = Mathf.Lerp(bpCanvasGroup.alpha,0,lerpval);
                        lerpval += -transitionSpeed * Time.deltaTime;
                        
                        if (lerpval <= 0) {
                            isTransitioning = false;
                            bpCanvasGroup.alpha = 0;
                            lerpval = 0;
                            cutscenePhase = 0;
                            isCutscene = false;
                        }
                    }
                break;
            }
            //Debug.Log(cutscenePhase);
            Debug.Log(director.duration);
        }
        
    }

    public void StartCutscene() {
        if (bc.isTrigger) {
            isCutscene = true;
            playerMovementScript.inCutscene = true;
            cutsceneCanvas.SetActive(true);
            
            //player.SetActive(false);
        }
    }

    private void BeginTimeline() {
        director.Play();
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(true); 
        }
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 10;
        }
        cutscenePhase = 1;
        
    }

    private void EndCutscene() {
        Destroy(cutsceneTrigger);
        director.Stop();
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 0;  
        }
        if (usingCutsceneWeaver) {
            cutsceneWeaver.SetActive(false);
        }
        //player.SetActive(true);
        playerMovementScript.inCutscene = false;
        cutscenePhase = 3;
    }

}
