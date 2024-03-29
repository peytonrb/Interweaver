using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCannonController : MonoBehaviour
{
    [Header("Variables")]
    [CannotBeNullObjectField] public Transform playerTransform;
    private bool netCannonOn;
    private bool hasInvoked;
    public List <NetCannonData> netCannonsList = new List<NetCannonData>();
    private NetCannonScript netCannonScript;
    private GameObject netCannon;

    [System.Serializable]
    public struct NetCannonData
    {
        public GameObject realNetCannon;
        public bool isNetCannonOn;
    }
    void Awake()
    {
        hasInvoked = false; 
    }
   
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
            netCannonOn = true;
            if (netCannonOn) 
            {
                netCannonScript.FireNet();
            }
           
        }
    }
}
