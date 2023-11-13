using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    [CannotBeNullObjectField] public List<GameObject> shieldPuzzleWeaveables;
    [SerializeField] private List<Vector3> startPositions;
    [SerializeField] private List<Quaternion> startRotations;
    [SerializeField] private List<GameObject> respawnObjects;

    private void OnTriggerEnter(Collider collider)
    {
        // clear past storage
        startPositions.Clear();
        startRotations.Clear();
        respawnObjects.Clear();

        if (collider.gameObject.GetComponent<WeaveableNew>() != null)
        {
            WeaveableNew weaveableObject = collider.gameObject.GetComponent<WeaveableNew>();

            if (weaveableObject.isCombined)
            {
                Debug.Log("object: " + weaveableObject + " count: " + weaveableObject.wovenObjects.Count);
                for (int i = 0; i < weaveableObject.wovenObjects.Count; i++)
                {
                    startPositions.Add(weaveableObject.wovenObjects[i].startPos);
                    startRotations.Add(weaveableObject.wovenObjects[i].startRot);
                    respawnObjects.Add(weaveableObject.wovenObjects[i].gameObject);
                }
            }
            else
            {
                // list will have max 1 element at this point
                startPositions.Add(weaveableObject.startPos);
                startRotations.Add(weaveableObject.startRot);
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

            respawnObjects[i].transform.localPosition = startPositions[i];
            respawnObjects[i].transform.rotation = startRotations[i];

            respawnObjects[i].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            respawnObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    // will commonnly be called on puzzles that require a full reset if failed
    private void InitializeObjects()
    {
        // populate once this case occurs, unsure how these will get initialized rn
    }

    public void RespawnInShieldPuzzle()
    {
        for (int i = 0; i < shieldPuzzleWeaveables.Count; i++)
        {
            shieldPuzzleWeaveables[i].transform.localPosition = shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().startPos;
            shieldPuzzleWeaveables[i].transform.rotation = shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().startRot;
        }
    }
}
