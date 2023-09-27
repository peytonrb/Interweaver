using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseScript : MonoBehaviour
{   
    public GameObject player;
    private PlayerScript playerScript;

    void Start() {
        playerScript = player.GetComponent<PlayerScript>();
    }

    //Resumes the game
    public void Resume() {
        gameObject.SetActive(false);
        playerScript.isPaused = false;
    }
}
