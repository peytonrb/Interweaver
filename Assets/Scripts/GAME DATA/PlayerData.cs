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

    /// <summary>
    /// Resets all game data.
    /// </summary>
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

    /// <summary>
    /// Returns the amount of levels completed thus far.
    /// </summary>
    /// <returns></returns>
    public int GetLevelsCompleted() {
        return levelsCompleted;
    }
    
    /// <summary>
    /// Adds 1 to the amount of levels completed.
    /// </summary>
    public void AddLevelCompleted() {
        levelsCompleted++;
    }

    /// <summary>
    /// Adds to or subtracts from the amount of lost souls collected so far. 
    /// </summary>
    /// <param name="souls">
    /// Amount of lost souls you want to add/subtract from the total lost soul count.
    /// </param>
    /// <param name="addSouls">
    /// If true, you are adding. If false, you are subtracting.
    /// </param>
    public void AddSubtractLostSoul(int souls, bool addSouls) {
        if (addSouls == true) {
            lostSoulCount += souls;
        }
        else {
            lostSoulCount -= souls;
        }
        
    }

    /// <summary>
    /// Returns the number of lost souls collected thus far.
    /// </summary>
    /// <returns></returns>
    public int GetLostSoulCount() {
        return lostSoulCount;
    }

    /// <summary>
    /// Returns true if there is a save file found in the data folder.
    /// </summary>
    /// <returns></returns>
    public bool GetSaveData() {
        return saveDataExists;
    }

    /// <summary>
    /// Sets levels completed by amount.
    /// </summary>
    /// <param name="amount"></param>
    public void SetLevelsCompleted(int amount) {
        levelsCompleted = amount;
    }

    /// <summary>
    /// Resets the amount of levels completed to 0.
    /// </summary>
    public void ResetLevelsCompleted() {
        levelsCompleted = 0;
    }

    /// <summary>
    /// Resets the amount of lost souls collected to 0.
    /// </summary>
    public void ResetLostSoulCount() {
        lostSoulCount = 0;
    }

}