using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{   
    public Toggle toggle;
    public static bool usingController = false;
    public GameObject inputManager;

    //Resumes the game
    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //Switches between using controller and using keyboard
    public void UsingController() {
        if (toggle.isOn == true) {
            InputManagerScript ips = inputManager.GetComponent<InputManagerScript>();
            ips.playerInput.SwitchCurrentControlScheme(Gamepad.current);
            usingController = true;
        }
        else {
            InputManagerScript ips = inputManager.GetComponent<InputManagerScript>();
            ips.playerInput.SwitchCurrentControlScheme(Keyboard.current);
            usingController = false;
        }
    }

    public void TurnOnUsingController() {
        InputManagerScript ips = inputManager.GetComponent<InputManagerScript>();
        ips.playerInput.SwitchCurrentControlScheme(Gamepad.current);
        usingController = true;
    }

    public void TurnOffUsingController() {
        InputManagerScript ips = inputManager.GetComponent<InputManagerScript>();
        ips.playerInput.SwitchCurrentControlScheme(Keyboard.current);
        usingController = false;
    }

    public bool GetUsingController() {
        return usingController;
    }
}
