using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public int levelsCompleted;
    public int lostSoulsFoundInAlpine;
    public int lostSoulsFoundInCavern;
    public int lostSoulsFoundInSepultus;
    public List<bool> lostSoulsinAlpine;
    public List<bool> lostSoulsinCavern;
    public List<bool> lostSoulsinSepultus;
}