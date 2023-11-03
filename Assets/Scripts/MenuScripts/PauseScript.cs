using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{   
    public Toggle toggle;
    public bool usingController = false;
    //Resumes the game
    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    public void UsingController() {
        if (toggle.isOn == true) {
            usingController = true;
        }
        else {
            usingController = false;
        }
    }
}
