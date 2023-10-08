using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseScript : MonoBehaviour
{   
    private PlayerScript playerScript;

    void Start() {

    }

    //Resumes the game
    public void Resume() {
        gameObject.SetActive(false);
        
    }
}
