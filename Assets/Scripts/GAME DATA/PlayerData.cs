using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public static int levelsCompleted = 0;
    public static int lostSoulCount = 0;
    public static bool saveDataExists = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void SaveGame() {
        GameSaveData gameData = new GameSaveData();
        gameData.levelsCompleted = levelsCompleted;
        gameData.lostSoulCount = lostSoulCount;

        string json = JsonUtility.ToJson(gameData,false);
        File.WriteAllText(Application.dataPath + "/GameData.json",json);
        Debug.Log("Game Saved!");
    }

    public void LoadGame() {
        InitialFileCheck();

        if (saveDataExists) {
            string json = File.ReadAllText(Application.dataPath + "/GameData.json");
            GameSaveData gameData = JsonUtility.FromJson<GameSaveData>(json);

            levelsCompleted = gameData.levelsCompleted;
            lostSoulCount = gameData.lostSoulCount;

            Debug.Log("Game found and loaded!");
        }
        else {
            Debug.LogError("FATAL ERROR: No save data detected. Use SaveGame() to create a new save file.");
        }
    }

    public void NewGame() {
        ResetAllData();
    }

    public void ResetAllData() {
        levelsCompleted = 0;
        lostSoulCount = 0;
        saveDataExists = false;
    }

    public void InitialFileCheck() {
        bool fileExists = File.Exists(Application.dataPath + "/GameData.json");

        if (fileExists == true) {
            saveDataExists = true;
        }
        else {
            saveDataExists = false;
        }
    }

    public int GetLevelsCompleted() {
        return levelsCompleted;
    }

    public int GetLostSoulCount() {
        return lostSoulCount;
    }

    public bool GetSaveData() {
        return saveDataExists;
    }

    public void SetLevelsCompleted(int amount) {
        levelsCompleted = amount;
    }

    public void ResetLevelsCompleted() {
        levelsCompleted = 0;
    }

}
