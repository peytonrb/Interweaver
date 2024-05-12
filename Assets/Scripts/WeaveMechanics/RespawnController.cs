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
    private FamiliarScript familiar;
    private const float delay = 2f;
    private bool hasBeenCheckedOnce;

    public void Start()
    {
        rayList = new List<GameObject>();
        player = GameObject.FindWithTag("Player").GetComponent<WeaveController>();
        familiar = GameObject.FindWithTag("Familiar").GetComponent<FamiliarScript>();

        StartCoroutine(StartDelay());
    }

    public void Update()
    {
        //Weaveable respawns can only be checked on the weaver's turn
        if (familiar.myTurn == false)
        {
            timeBeforeCheck -= Time.deltaTime;

            if (timeBeforeCheck < 0)
            {
                for (int i = 0; i < startPositions.Count; i++)
                {
                    // checks if the respawnable has moved since the start position and also checks if it moved by more than 5f in any direction 
                    //    (in case starting position was slightly above the ground or something)
                    if (myRespawnables[i].transform.position != startPositions[i] && 
                        Vector3.Dot(((startPositions[i] - new Vector3(10f, 10f, 10f)) - (startPositions[i] + new Vector3(10f, 10f, 10f))).normalized, 
                                    (myRespawnables[i].transform.position - (startPositions[i] - new Vector3(10f, 10f, 10f))).normalized) < 0f && 
                                    Vector3.Dot(((startPositions[i] + new Vector3(10f, 10f, 10f)) - (startPositions[i] - new Vector3(10f, 10f, 10f))).normalized, 
                                    (myRespawnables[i].transform.position - (startPositions[i] + new Vector3(10f, 10f, 10f))).normalized) < 0f)
                    {
                    //This makes sure if multiple respawn weaveables have moved, the controller will only check once per timer delay instead of running it multiple times in one frame.
                    if (hasBeenCheckedOnce == false)
                    {
                        //Check here
                        //Debug.Log("here");
                        CheckAndRespawnWeaveables();
                        hasBeenCheckedOnce = true;
                    }
                }
            }
            hasBeenCheckedOnce = false;
            timeBeforeCheck = 3;
        }
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
        //Debug.Log(hit.collider.gameObject);
        foreach (GameObject obj in myRespawnables)
        {
            if (hit.collider.gameObject == obj)
            {
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
        Debug.Log(objectToRespawn);
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

IEnumerator StartDelay()
{
    yield return new WaitForSeconds(delay);

    foreach (GameObject obj in myRespawnables)
    {
        startPositions.Add(obj.transform.position);
        startRotations.Add(obj.transform.rotation);
    }

    yield break;
}


void OnDrawGizmos()
{
    ExtDebug.DrawBox(transform.position, boxCastHalfExtent, transform.rotation, Color.red);

}
}
