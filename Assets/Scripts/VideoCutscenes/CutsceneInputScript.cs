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

    // Start is called before the first frame update
    void Start()
    {
        vcc = videoCutscene.GetComponent<VideoCutsceneController>();

        startingTextTimer = popupTextTimer;
        skipCutsceneText.SetActive(true);
    }

    void Update() {
        if (skipCutsceneText.activeSelf) {
            popupTextTimer -= Time.deltaTime;
            if (popupTextTimer <= 0f) {
                skipCutsceneText.SetActive(false);
                popupTextTimer = startingTextTimer;
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
