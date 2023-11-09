using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    [SerializeField] private List<Vector3> startPositions;
    [SerializeField] private List<Quaternion> startRotations;
    [SerializeField] private List<GameObject> respawnObjects;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<WeaveableNew>() != null)
        {
            WeaveableNew weaveableObject = collider.gameObject.GetComponent<WeaveableNew>();

            if (weaveableObject.isCombined)
            {
                Debug.Log("object: " + weaveableObject + " count: " + weaveableObject.wovenObjects.Count);
                for (int i = 0; i < weaveableObject.wovenObjects.Count; i++)
                {
                    startPositions.Add(weaveableObject.wovenObjects[i].spawnPos);
                    Debug.Log("start pos: " + startPositions[i]);
                    startRotations.Add(weaveableObject.wovenObjects[i].spawnRotation);
                    Debug.Log("start rot: " + startRotations[i]);
                    respawnObjects.Add(weaveableObject.wovenObjects[i].gameObject);
                    Debug.Log("respawn objects: " + respawnObjects[i]);
                }
            }
            else
            {
                // list will have max 1 element at this point
                startPositions.Add(weaveableObject.spawnPos);
                startRotations.Add(weaveableObject.spawnRotation);
                respawnObjects.Add(weaveableObject.gameObject);
            }
        }
        else
        {
            // other objects not set up to respawn yet
        }

        RespawnObject();
    }

    // can be called in puzzles and other events that require respawning objects
    public void RespawnObject()
    {
        // if respawn isn't caused by a collision
        if (startPositions == null || startRotations == null || respawnObjects == null)
        {
            InitializeObjects();
        }

        for (int i = 0; i < respawnObjects.Count; i++)
        {
            if (respawnObjects[i].GetComponent<WeaveableNew>() != null)
            {
                respawnObjects[i].GetComponent<WeaveableNew>().Uncombine();
            }

            respawnObjects[i].transform.position = startPositions[i];
            respawnObjects[i].transform.rotation = startRotations[i];
        }

        // clear all references once objects have been respawned
        startPositions.Clear();
        startRotations.Clear();
        respawnObjects.Clear();
    }

    // will commonnly be called on puzzles that require a full reset if failed
    private void InitializeObjects()
    {
        // populate once this case occurs, unsure how these will get initialized rn
    }
}
