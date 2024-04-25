using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCutsceneTrigger : MonoBehaviour
{
    private AnimaticCutsceneController amc;
    private VideoCutsceneController vcc;
    private GameMasterScript gameMaster;
    public int cutsceneToPlay;
    private int currentLevel;

    void Start() {
        amc = GetComponent<AnimaticCutsceneController>();
        vcc = GetComponent<VideoCutsceneController>();
        gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();

    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar")) {
            StartCutscene();
        }
    }

    public void StartCutscene() {
        int lostSoulsCollected = gameMaster.totalLostSouls;
        string currentScene = SceneHandler.instance.currentSceneName;
        switch (currentScene) {
            case "AlpineCombined":
                currentLevel = 1;
            break;
            case "Cavern":
                currentLevel = 2;
            break;
            case "Sepultus":
                currentLevel = 3;
            break;
        }
        //Updates the game's data and saves it.
        PlayerData.instance.AddSubtractLostSoul(currentLevel,lostSoulsCollected,true);
        PlayerData.instance.SetLevelsCompleted(currentLevel);
        PlayerData.instance.SaveGame();
        amc.ChangeCutscene(cutsceneToPlay);
        vcc.ChangeCutscene(cutsceneToPlay);
        SceneHandler.instance.LoadLevel("AnimaticCutscenes");
    }
    
}
