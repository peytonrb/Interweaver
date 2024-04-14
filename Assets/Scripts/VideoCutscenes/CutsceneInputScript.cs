using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneInputScript : MonoBehaviour
{
    [SerializeField] private GameObject skipCutsceneTextKeyboard;
    [SerializeField] private GameObject skipCutsceneTextGamepad;
    [SerializeField] private float popupTextTimer;
    private float startingTextTimer;
    [SerializeField] private GameObject videoCutscene;
    private VideoCutsceneController vcc;
    [SerializeField] private GameObject introController;
    private PlayerInput playerInput;
    private bool gameStarted;
    private bool usingController;
    private bool hasPressedSkip;
    // Start is called before the first frame update
    void Start()
    {
        vcc = videoCutscene.GetComponent<VideoCutsceneController>();
        playerInput = GetComponent<PlayerInput>();
        IntroSequenceScript introSequenceScript = introController.GetComponent<IntroSequenceScript>();
        gameStarted = introSequenceScript.GetGameStarted();
        hasPressedSkip = false;

        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            usingController = true;
            playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
        }
        else 
        {
            CheckForKeyboard();
        } 

        if (gameStarted) 
        {
            startingTextTimer = popupTextTimer;

            if (usingController == false) 
            {
                skipCutsceneTextKeyboard.SetActive(true);
                skipCutsceneTextGamepad.SetActive(false);
            }
            else 
            {
                skipCutsceneTextKeyboard.SetActive(false);
                skipCutsceneTextGamepad.SetActive(true);
            }
        }
        else 
        {
            skipCutsceneTextKeyboard.SetActive(false);
            skipCutsceneTextGamepad.SetActive(false);
        }
    }

    void CheckForKeyboard() {
        if (Keyboard.current != null || Mouse.current != null)
        {
            usingController = false;
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
    }

    void Update() {
        if (gameStarted) {
            if (usingController == false) 
            {
                if (skipCutsceneTextKeyboard.activeSelf) 
                {
                    popupTextTimer -= Time.deltaTime;
                    if (popupTextTimer <= 0f) 
                    {
                        skipCutsceneTextKeyboard.SetActive(false);
                        popupTextTimer = startingTextTimer;
                    }
                }
            }
            else
            {
                if (skipCutsceneTextGamepad.activeSelf) 
                {
                    popupTextTimer -= Time.deltaTime;
                    if (popupTextTimer <= 0f) 
                    {
                        skipCutsceneTextGamepad.SetActive(false);
                        popupTextTimer = startingTextTimer;
                    }
                }
            }
        }

        if (SceneHandler.instance.loadOperation.isDone) 
        {
            hasPressedSkip = false;
        }
    }

    public void OnSkipCutscene(InputValue input) {
        if (input.isPressed) 
        {
            if (usingController == false)
            {
                if (skipCutsceneTextKeyboard.activeSelf && !hasPressedSkip) 
                {
                    vcc.SkipCutscene();
                    hasPressedSkip = true;
                }
                else 
                {
                    skipCutsceneTextKeyboard.SetActive(true);
                }
            }
            else
            {
                if (skipCutsceneTextGamepad.activeSelf) 
                {
                    vcc.SkipCutscene();
                }
                else 
                {
                    skipCutsceneTextGamepad.SetActive(true);
                }
            }    
        }
        
    }
}
