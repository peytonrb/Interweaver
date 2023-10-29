using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    private float transitionSpeed;
    private float lerpval;
    private PlayableDirector director;

    [Header("Player Objects")]
    public GameObject player;
    private MovementScript playerMovementScript;
    public GameObject cutsceneWeaver;

    // Start is called before the first frame update
    void Start()
    {
        cutsceneCanvas.SetActive(false);
        cutsceneWeaver.SetActive(false);
        bpCanvasGroup = blackPanel.GetComponent<CanvasGroup>();
        playerMovementScript = player.GetComponent<MovementScript>();
        director = GetComponent<PlayableDirector>();

        isCutscene = false;
        isTransitioning = false;
        cutscenePhase = 0;
        lerpval = 0;
        transitionSpeed = 0.01f;

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
                        
                        if (bpCanvasGroup.alpha >= 0.95f) {
                            cutscenePhase = 1;
                            bpCanvasGroup.alpha = 1;
                        }
                    }
                break;
                case 1:
                    if (isTransitioning) {
                        bpCanvasGroup.alpha = Mathf.Lerp(bpCanvasGroup.alpha,0,lerpval);
                        lerpval -= transitionSpeed * Time.deltaTime;
                        if (bpCanvasGroup.alpha <= 0.05f) {
                            BeginTimeline();
                            isTransitioning = false;
                            bpCanvasGroup.alpha = 0;
                        }
                    }
                    else {
                        cutscenePhase = 2;
                    }
                break;
                case 2:
                    if (director.time > 25f) {
                        EndCutscene();
                        cutscenePhase = 3;
                    }
                break;
            }
        }
    }

    public void StartCutscene() {
        isCutscene = true;
        playerMovementScript.active = false;
        cutsceneCanvas.SetActive(true);
        player.SetActive(false);
    }

    private void BeginTimeline() {
        director.Play();
        foreach (CinemachineVirtualCamera vcam in cutsceneCams) {
            vcam.Priority = 10;
        }
        cutsceneWeaver.SetActive(true);    
        
    }

    private void EndCutscene() {
        
    }

}
