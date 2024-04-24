using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    [CannotBeNullObjectField] public List<GameObject> shieldPuzzleWeaveables;
    public List<GameObject> myRespawnables;
    [HideInInspector] public List<Vector3> startPositions;
    [HideInInspector] public List<Quaternion> startRotations;
    private bool AutoAssignMyWeavables = true;
    public LayerMask layersToCheck;
    public Vector3 boxCastHalfExtent;
    public List<GameObject> rayList;
    private float timeBeforeCheck = 3;
    private WeaveController player;

    public void Start()
    {
        rayList = new List<GameObject>();
        player = GameObject.FindWithTag("Player").GetComponent<WeaveController>();

        foreach (GameObject obj in myRespawnables)
        {
            startPositions.Add(obj.transform.position);
            startRotations.Add(obj.transform.rotation);
        }
    }

    public void Update()
    {
        timeBeforeCheck -= Time.deltaTime;

        if (timeBeforeCheck < 0)
        {
            //Check here
            CheckAndRespawnWeaveables();
            timeBeforeCheck = 3;
        }
    }

    public void CheckAndRespawnWeaveables()
    {
        rayList.Clear();
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxCastHalfExtent, transform.up, transform.rotation, layersToCheck);
        rayList = new List<GameObject>(myRespawnables);

        //remove elements that are still found in the box
        foreach (RaycastHit hit in hits)
        {
            foreach (GameObject obj in myRespawnables)
            {
                if (hit.collider.gameObject == obj)
                {
                    //Debug.Log(hit.collider.gameObject);
                    rayList.Remove(hit.collider.gameObject);
                }
            }
        }

        //respawn other elements
        foreach (GameObject obj in rayList)
        {
            //Call respawn
            if (obj != null)
            {
                RespawnObject(obj);
            }
        }

    }

    //// can be called in puzzles and other events that require respawning objects
    public void RespawnObject(GameObject objectToRespawn)
    {
        if (!objectToRespawn.activeSelf)
        {
            Debug.Log("object wasn't active");
            return;

        }

        if (objectToRespawn.TryGetComponent<CrystalScript>(out CrystalScript crystal))
        {
            if (crystal.myFloatingIsland != null)
            {
                objectToRespawn.transform.position = startPositions[myRespawnables.IndexOf(objectToRespawn)];
                objectToRespawn.transform.rotation = startRotations[myRespawnables.IndexOf(objectToRespawn)];
                objectToRespawn.GetComponent<Rigidbody>().velocity = Vector3.zero;
                return;
            }
        }

        if (objectToRespawn.TryGetComponent<WeaveableObject>(out WeaveableObject weaveable))
        {
            player.OnDrop();
        }

        objectToRespawn.transform.position = startPositions[myRespawnables.IndexOf(objectToRespawn)];
        objectToRespawn.transform.rotation = startRotations[myRespawnables.IndexOf(objectToRespawn)];
        objectToRespawn.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    //// will commonnly be called on puzzles that require a full reset if failed
    //private void InitializeObjects()
    //{
    //    // populate once this case occurs, unsure how these will get initialized rn
    //}

    public void CheckAndRespawnShieldPuzzleWeaveables()
    {
        foreach (GameObject obj in shieldPuzzleWeaveables)
        {
            //Call respawn
            if (obj != null)
            {
                RespawnObject(obj);
            }
        }
    }


    void OnDrawGizmos()
    {
        ExtDebug.DrawBox(transform.position, boxCastHalfExtent, transform.rotation, Color.red);

    }
}
