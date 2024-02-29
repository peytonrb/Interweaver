using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static int levelsCompleted = 0;

    public int GetLevelsCompleted() {
        return levelsCompleted;
    }

    public void SetLevelsCompleted(int amount) {
        levelsCompleted = amount;
    }

    public void ResetLevelsCompleted() {
        levelsCompleted = 0;
    }

}
