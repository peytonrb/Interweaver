using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCannonControlScript : MonoBehaviour
{
    [Header("Variables")]
    [CannotBeNullObjectField] public Transform playerTransform;
    [CannotBeNullObjectField] public Transform familiarTransform;
    private bool netCannonOn;
    private NetCannonScript netCannonScript;
    private GameObject netCannon;
    public Collider colliderDetector;
    private Vector3 boxCastHalfExtent;

    [System.Serializable]
    public struct NetCannonData
    {
        public GameObject realNetCannon;
        public bool isNetCannonOn;
    }
    public List<NetCannonData> netCannonsList = new List<NetCannonData>();

    
    void Update()
    {
        BatchColliderForCannon();
    }

    void BatchColliderForCannon()
    {
        for (int i = 0; i < netCannonsList.Count; i++)
        {
            netCannon = netCannonsList[i].realNetCannon;
            netCannonOn = netCannonsList[i].isNetCannonOn;           
            netCannonScript = netCannon.GetComponent<NetCannonScript>();

            if (colliderDetector.bounds.Contains(playerTransform.position) || colliderDetector.bounds.Contains(familiarTransform.position))
            {
                netCannonScript.FireNet();
            }
            //this is here if you want to indavidually mess with the net cannons
            //*******************************
            if(netCannonOn) 
            {
                netCannonScript.FireNet();
            }
            //*******************************                 
        }
    }
}
