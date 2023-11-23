using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCutsceneTrigger : MonoBehaviour
{
    private AnimaticCutsceneController amc;

    void Start() {
        amc = GetComponent<AnimaticCutsceneController>();
    }

    public void StartCutscene() {
        amc.ChangeCutscene(1);
        //Debug.Log("Hit");
        //Changes to end cutscene
        SceneManager.LoadScene("AnimaticCutscenes");
    }
    
}
