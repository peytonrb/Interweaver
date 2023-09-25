using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionTestScript : MonoBehaviour
{
    private GameMasterScript GM;

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();
        transform.position = GM.LastCheckPointPos;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //this is purely for testing the checkpoint function if it's working properly
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //this is for testing
        }
    }
}
