using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteSpawnerController : MonoBehaviour
{
    [Header("Variables")]
    [CannotBeNullObjectField] public Transform playerTransform;
    [CannotBeNullObjectField] public Transform familiarTransform;
    private StalactiteSpawnerScript stalactiteSpawnerScript;
    private GameObject realStalactiteSpawners;
    public Collider colliderDetector;

    [System.Serializable]
    public struct StalactiteData
    {
        public GameObject stalactiteSpawner;
    }
    public List<StalactiteData> stalactiteSpawnerLists = new List<StalactiteData>();

    // Update is called once per frame
    void Update()
    {
        BatchCollision();
    }

    void BatchCollision()
    {
        for (int i = 0; i < stalactiteSpawnerLists.Count; i++)
        {
            realStalactiteSpawners = stalactiteSpawnerLists[i].stalactiteSpawner;

            stalactiteSpawnerScript = realStalactiteSpawners.GetComponent<StalactiteSpawnerScript>();

            if (colliderDetector.bounds.Contains(playerTransform.position) || colliderDetector.bounds.Contains(familiarTransform.position))
            {
                stalactiteSpawnerScript.canFall = true;
            }

            else
            {
                stalactiteSpawnerScript.canFall = false;
            }
        }
    }
}
