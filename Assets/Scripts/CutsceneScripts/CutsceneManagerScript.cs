using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManagerScript : MonoBehaviour
{
    public GameObject cutsceneCanvas;
    public GameObject blackPanel;
    private CanvasGroup bpCanvasGroup;
    private bool isCutscene;
    private bool isTransitioning;
    private int cutscenePhase;
    private float transitionSpeed;
    private float lerpval;

    [Header("Player Objects")]
    public GameObject player;
    private MovementScript playerMovementScript;

    // Start is called before the first frame update
    void Start()
    {
        cutsceneCanvas.SetActive(false);
        bpCanvasGroup = blackPanel.GetComponent<CanvasGroup>();
        playerMovementScript = player.GetComponent<MovementScript>();

        isCutscene = false;
        isTransitioning = false;
        cutscenePhase = 0;
        lerpval = 0;
        transitionSpeed = 0.01f;
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
                        if (bpCanvasGroup.alpha >= 1) {
                            cutscenePhase = 1;
                        }
                    }
                break;
                case 1:
                    if (isTransitioning) {
                        bpCanvasGroup.alpha = Mathf.Lerp(bpCanvasGroup.alpha,1,lerpval);
                        lerpval -= transitionSpeed * Time.deltaTime;
                        if (bpCanvasGroup.alpha <= 0) {
                            isTransitioning = false;
                        }
                    }
                    else {
                        
                    }
                break;
            }
        }
    }

    public void StartCutscene() {
        isCutscene = true;
        playerMovementScript.active = false;
        cutsceneCanvas.SetActive(true);
    }

}
