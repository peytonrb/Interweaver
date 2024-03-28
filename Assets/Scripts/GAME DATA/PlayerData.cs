using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public static int lostSoulCountAlpine = 0;
    public static int lostSoulCountCavern = 0;
    public static int lostSoulCountSepultus = 0;

    //If false, then that lost soul has not been retrieved from that level.
    //The length of the array should be 3 for each level.
    public static List<bool> lostSoulsinAlpine = new List<bool>();
    public static List<bool> lostSoulsinCavern = new List<bool>();
    public static List<bool> lostSoulsinSepultus = new List<bool>();

    public static int levelsCompleted = 0;
    public static bool saveDataExists = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        if (lostSoulsinAlpine.Count < 3) {
            lostSoulsinAlpine.Capacity = 3;
            for (int i = 0; i < lostSoulsinAlpine.Count; i++) {
                lostSoulsinAlpine[i] = true;
            }
        }
        if (lostSoulsinCavern.Count < 3) {
            lostSoulsinCavern.Capacity = 3;
            for (int i = 0; i < lostSoulsinCavern.Count; i++) {
                lostSoulsinCavern[i] = true;
            }
        }
        if (lostSoulsinSepultus.Count < 3) {
            lostSoulsinSepultus.Capacity = 3;
            for (int i = 0; i < lostSoulsinSepultus.Count; i++) {
                lostSoulsinSepultus[i] = true;
            }
        }
        
    }

    public void SaveGame() {
        GameSaveData gameData = new GameSaveData();
        gameData.levelsCompleted = levelsCompleted;
        gameData.lostSoulsFoundInAlpine = lostSoulCountAlpine;
        gameData.lostSoulsFoundInCavern = lostSoulCountCavern;
        gameData.lostSoulsFoundInSepultus = lostSoulCountSepultus;
        gameData.lostSoulsinAlpine = lostSoulsinAlpine;
        gameData.lostSoulsinCavern = lostSoulsinCavern;
        gameData.lostSoulsinSepultus = lostSoulsinSepultus;

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
            lostSoulCountAlpine = gameData.lostSoulsFoundInAlpine;
            lostSoulCountCavern = gameData.lostSoulsFoundInCavern;
            lostSoulCountSepultus = gameData.lostSoulsFoundInSepultus;
            lostSoulsinAlpine = gameData.lostSoulsinAlpine;
            lostSoulsinCavern = gameData.lostSoulsinCavern;
            lostSoulsinSepultus = gameData.lostSoulsinSepultus;

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
        lostSoulCountAlpine = 0;
        lostSoulCountCavern = 0;
        lostSoulCountSepultus = 0;
        
        for (int i = 0; i < lostSoulsinAlpine.Count; i++) {
            lostSoulsinAlpine[i] = true;
        }
        for (int i = 0; i < lostSoulsinCavern.Count; i++) {
            lostSoulsinCavern[i] = true;
        }
        for (int i = 0; i < lostSoulsinSepultus.Count; i++) {
            lostSoulsinSepultus[i] = true;
        }
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
    /// <param name="level">
    /// level 1 = Alpine, level 2 = Cavern, level 3 = Sepultus
    /// </param>
    /// <param name="souls">
    /// Amount of lost souls you want to add/subtract from the total lost soul count.
    /// </param>
    /// <param name="addSouls">
    /// If true, you are adding. If false, you are subtracting.
    /// </param>
    public void AddSubtractLostSoul(int level, int souls, bool addSouls) {
        if (addSouls == true) {
            switch (level) {
                case 1:
                    lostSoulCountAlpine += souls;
                break;
                case 2:
                    lostSoulCountCavern += souls;
                break;
                case 3:
                    lostSoulCountSepultus += souls;
                break;
            }
        }
        else {
            switch (level) {
                case 1:
                    lostSoulCountAlpine -= souls;
                break;
                case 2:
                    lostSoulCountCavern -= souls;
                break;
                case 3:
                    lostSoulCountSepultus -= souls;
                break;
            }
        }
        
    }

    /// <summary>
    /// References the existence of lost souls in a particular level.
    /// </summary>
    /// <returns></returns>
    public List<bool> GetAlpineLostSouls() {
        return lostSoulsinAlpine;
    }

    public List<bool> GetCavernLostSouls() {
        return lostSoulsinCavern;
    }

    public List<bool> GetSepultusLostSouls() {
        return lostSoulsinSepultus;
    }

    /// <summary>
    /// Sets the lost soul's existence in a level
    /// </summary>
    /// <param name="index">
    /// The index of the lostSoulsinAlpine list.
    /// </param>
    /// <param name="isAlive">
    /// Is this Lost Soul alive? (Hint: Set to false if its being captured)
    /// </param>
    public void SetAlpineLostSouls(int index, bool isAlive) {
        lostSoulsinAlpine[index] = isAlive;
    }

    public void SetCavernLostSouls(int index, bool isAlive) {
        lostSoulsinAlpine[index] = isAlive;
    }

    public void SetSepultusLostSouls(int index, bool isAlive) {
        lostSoulsinAlpine[index] = isAlive;
    }

    /// <summary>
    /// Returns the number of lost souls collected in Alpine thus far.
    /// </summary>
    /// <returns></returns>
    public int GetAlpineLostSoulCount() {
        return lostSoulCountAlpine;
    }

    /// <summary>
    /// Returns the number of lost souls collected in Cavern thus far.
    /// </summary>
    /// <returns></returns>
    public int GetCavernLostSoulCount() {
        return lostSoulCountCavern;
    }

    /// <summary>
    /// Returns the number of lost souls collected in Sepultus thus far.
    /// </summary>
    /// <returns></returns>
    public int GetSepultusLostSoulCount() {
        return lostSoulCountSepultus;
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
    public void ResetAlpineSoulCount() {
        lostSoulCountAlpine = 0;
    }

    public void ResetCavernSoulCount() {
        lostSoulCountCavern = 0;
    }

    public void ResetSepultusSoulCount() {
        lostSoulCountSepultus = 0;
    }

}