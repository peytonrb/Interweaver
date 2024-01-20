using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceScript : MonoBehaviour
{
    public static LightSourceScript Instance;

    [Header("Variables")]
    [CannotBeNullObjectField] public Transform playerTransform;
    public LayerMask obstructionView;
    private bool lightsOn;
    private float lightMaxDistance;
    private Light lightStuff;
    [System.Serializable] public struct LightData
    {
        public Light lightSource;
        [Range (0,40)] public float maxDistance;
        public SphereCollider lightCollider;
        public bool isOn;
    }

    public LightData[] lightsArray;

    void Awake()
    {
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
        BatchRaycastInfoForLights(); 

        if(lightsOn) 
        {
            BatchRaycastForLights(lightMaxDistance, lightStuff);
        }
    }

    void BatchRaycastInfoForLights() //will need to make this better so that it doesn't happen every frame, kind of cringe ngl
    {
        
        foreach (LightData lightData in lightsArray)
        {
            Light lightSource = lightData.lightSource; //this is from the public struct

            float maxDistance = lightData.maxDistance; //also from the public struct
            
            lightSource.range = maxDistance;

            lightsOn = lightData.isOn;


            BatchRaycastForLights(maxDistance, lightSource);
        }
    }
    void BatchRaycastForLights(float Distance, Light pointLight)
    {
        lightMaxDistance = Distance;
        lightStuff = pointLight;
        if (lightsOn)
        {
            Vector3 directionToPlayer = playerTransform.position - lightStuff.transform.position;
            //the batch raycast from all light sources in the array will point towards the player and will do somehing if the object between the player and the  light source if it has the layer
            RaycastHit[] hits = Physics.RaycastAll(lightStuff.transform.position, directionToPlayer, lightMaxDistance, obstructionView);

            Debug.DrawRay(lightStuff.transform.position, directionToPlayer * lightMaxDistance / 2f, Color.green);

            foreach (RaycastHit hit in hits)
            {
                //inside this logic this can be used for obstructing the view if there is an object with the obstruction layer
                Debug.Log("Light source: " + lightStuff.name + " - Obstruction by: " + hit.collider.gameObject.name);
            }
        }
    }
}
