using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private GameObject respawnObject;

    private void OnTriggerEnter(Collider collider)
    {
        startPosition = collider.gameObject.GetComponent<RespawnableObject>().spawnPos;
        startRotation = collider.gameObject.GetComponent<RespawnableObject>().spawnRotation;
        respawnObject = collider.gameObject;

        RespawnObject();
    }

    // can be called in puzzles and other events that require respawning objects
    public void RespawnObject()
    {
        // if respawn isn't caused by a collision
        if (startPosition == null || startRotation == null || respawnObject == null)
        {
            InitializeObjects();
        }

        respawnObject.transform.position = startPosition;
        respawnObject.transform.rotation = startRotation;
    }

    // will commonnly be called on puzzles that require a full reset if failed
    private void InitializeObjects()
    {
        // populate once this case occurs, unsure how these will get initialized rn
    }
}
