using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedWyvernScript : MonoBehaviour
{
    [SerializeField] private WyvernBossManager wyvernBossManager;

    private void InstantiateFireball()
    {
        wyvernBossManager.InstantiateFireball(); // I don't like this, ngl
    }
}
