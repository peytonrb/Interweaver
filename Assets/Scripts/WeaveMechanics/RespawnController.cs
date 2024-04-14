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

    public void Start()
    {
        rayList = new List<GameObject>();

        //if (AutoAssignMyWeavables)
        //{
        //    myRespawnables = new List<GameObject>();
        //    RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxCastHalfExtent, transform.up, transform.rotation, layersToCheck);

        //    foreach (RaycastHit hit in hits)
        //    {
        //        if (!hit.collider.CompareTag("FloatingIsland") && hit.collider.GetComponent<WeaveableNew>() != null)
        //        {
        //            myRespawnables.Add(hit.collider.gameObject);
        //            startPositions.Add(hit.collider.gameObject.transform.position);
        //            startRotations.Add(hit.collider.gameObject.transform.rotation);
        //        }
        //    }
        //}
        //else
        //{
            foreach(GameObject obj in myRespawnables)
            {
                startPositions.Add(obj.transform.position);
                startRotations.Add(obj.transform.rotation);
            }
        //}
        
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
                Debug.Log("respawned " + obj.name);
                RespawnObject(obj);
            }
        }

    }

    //// can be called in puzzles and other events that require respawning objects
    public void RespawnObject(GameObject objectToRespawn)
    {
        if(objectToRespawn.TryGetComponent<CrystalScript>(out CrystalScript crystal))
        {
            if(crystal.myFloatingIsland != null)
            {
                return;
            }
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

    public void RespawnInShieldPuzzle()
    {

        for (int i = 0; i < shieldPuzzleWeaveables.Count; i++)
        {
            shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().Uncombine();
            shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().Uninteract();
            shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().player.weaveVisualizer.StopAura(shieldPuzzleWeaveables[i]);
            shieldPuzzleWeaveables[i].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            //shieldPuzzleWeaveables[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            shieldPuzzleWeaveables[i].transform.localPosition = shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().startPos;
            shieldPuzzleWeaveables[i].transform.rotation = shieldPuzzleWeaveables[i].GetComponent<WeaveableNew>().startRot;
        }
    }


    void OnDrawGizmos()
    {
        ExtDebug.DrawBox(transform.position, boxCastHalfExtent, transform.rotation, Color.red);
        
    }
}
