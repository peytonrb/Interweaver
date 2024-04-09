using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneInputScript : MonoBehaviour
{
    [SerializeField] private GameObject skipCutsceneText;
    [SerializeField] private float popupTextTimer;
    private float startingTextTimer;
    [SerializeField] private GameObject videoCutscene;
    private VideoCutsceneController vcc;
    [SerializeField] private GameObject introController;
    private bool gameStarted;

    // Start is called before the first frame update
    void Start()
    {
        vcc = videoCutscene.GetComponent<VideoCutsceneController>();
        IntroSequenceScript introSequenceScript = introController.GetComponent<IntroSequenceScript>();
        gameStarted = introSequenceScript.GetGameStarted();

        if (gameStarted) {
            startingTextTimer = popupTextTimer;
            skipCutsceneText.SetActive(true);
        }
        else {
            skipCutsceneText.SetActive(false);
        }
    }

    void Update() {
        if (gameStarted) {
            if (skipCutsceneText.activeSelf) {
                popupTextTimer -= Time.deltaTime;
                if (popupTextTimer <= 0f) {
                    skipCutsceneText.SetActive(false);
                    popupTextTimer = startingTextTimer;
                }
            }
        }
    }

    public void OnSkipCutscene(InputValue input) {
        if (input.isPressed) {
            if (skipCutsceneText.activeSelf) {
                vcc.SkipCutscene();
            }
            else {
                skipCutsceneText.SetActive(true);
            }
        }
    }

}
