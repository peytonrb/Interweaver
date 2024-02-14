using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightSourceScript : MonoBehaviour
{
    public static LightSourceScript Instance;

    [Header("Variables")]
    [CannotBeNullObjectField] public Transform playerTransform;
    private bool lightsOn;
    private float lightMaxDistance;
    private Collider lightDataCollider;
    private bool hasInvoked;
    [System.Serializable]
    public struct LightData
    {
        public Light lightSource;
        [Range(0, 40)] public float maxDistance;
        public Collider lightCollider;
        public bool isOn;
    }

    public List <LightData> lightsArray = new List<LightData>();

    public UnityEvent triggerEvent, triggerEvent2;
    void Awake()
    {
        hasInvoked = false;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        BatchCollisionDetection();
    }

    void BatchCollisionDetection() //will need to make this better so that it doesn't happen every frame, kind of cringe ngl
    {
        bool playerIsNowSafe = false;

        for (int i = 0; i < lightsArray.Count; i++)
        {
            Light lightSource = lightsArray[i].lightSource; //this is from the public struct

            float maxDistance = lightsArray[i].maxDistance; //also from the public struct

            lightDataCollider = lightsArray[i].lightCollider;

            //lightSource.range = maxDistance;

            lightsOn = lightsArray[i].isOn;

            if ((lightDataCollider.bounds.Contains(playerTransform.position)) && (lightDataCollider != null) && (lightsOn))
            {
                Debug.Log("this is the collider that yoinked it: " + lightDataCollider.gameObject.name);
                playerIsNowSafe = true;
                hasInvoked = false;
                break;
            }
           
        }

        if ((playerIsNowSafe) && (!hasInvoked))
        {
            MakePlayerSafe();
            hasInvoked = true;
        }

        else if ((!playerIsNowSafe) && (hasInvoked))
        {
            MakePlayerDie();
            hasInvoked = false;
        }
    }


    void MakePlayerSafe()
    {
        triggerEvent.Invoke();
    }

    void MakePlayerDie()
    {
        triggerEvent2.Invoke();
    }
}
