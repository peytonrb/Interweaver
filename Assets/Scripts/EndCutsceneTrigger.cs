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
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Familiar") {
            amc.ChangeCutscene(1);
            //Changes to end cutscene
            SceneManager.LoadScene("AnimaticCutscenes");
        }
    }
    
}
