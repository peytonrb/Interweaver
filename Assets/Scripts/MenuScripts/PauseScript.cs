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
            }
        }

    }
}
