using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseScript : MonoBehaviour
{   
    //Resumes the game
    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
