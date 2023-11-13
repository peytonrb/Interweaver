using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterScript : MonoBehaviour
{
    private static GameMasterScript instance;
    public int totalLostSouls;
    public Vector3 WeaverCheckPointPos;
    public int WeaverCheckPointNum;
    public Vector3 FamiliarCheckPointPos;
    public int FamiliarCheckPointNum;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
