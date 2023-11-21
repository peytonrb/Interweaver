using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{   
    public static bool usingController = false;
    private Toggle toggle;
    private EventSystem eventSystem;

    [SerializeField] private GameObject optionGroup;
    [SerializeField] private GameObject defaultGroup;

    [SerializeField] private GameObject ControllerImage;
    [SerializeField] private GameObject KeyboardImage;

    public void Start()
    {
        toggle = GetComponentInChildren<Toggle>();
        eventSystem = FindObjectOfType<EventSystem>();
    }
    //Resumes the game
    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //Switches between using controller and using keyboard
    public void ToggleUsingController(bool isController)
    {
        if (isController)
        {
            if (Gamepad.current == null)
            {
                toggle.isOn = false;
            }
            else
            {
                InputManagerScript.instance.ToggleControlScheme(true);
                ControllerImage.SetActive(true);
                KeyboardImage.SetActive(false);
            }
        }
        else
        {
            if (Keyboard.current == null)
            {
                toggle.isOn = true;
            }
            else
            {
                InputManagerScript.instance.ToggleControlScheme(false);
                ControllerImage.SetActive(false);
                KeyboardImage.SetActive(true);
            }
        }

    }

    public void ToggleOptions()
    {
        if (optionGroup.activeSelf)
        {
            optionGroup.SetActive(false);
            defaultGroup.SetActive(true);

            optionGroup.GetComponent<CanvasGroup>().alpha = 0f;
            defaultGroup.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            defaultGroup.SetActive(false);
            optionGroup.SetActive(true);

            optionGroup.GetComponent<CanvasGroup>().alpha = 1f;
            defaultGroup.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    public void ResetToCheckpoint()
    {
        InputManagerScript.instance.ResetCurrentCharacter();
    }

    public void QuitToMenu()
    {
        SceneHandler.instance.LoadLevel("Menu");
    }
}
