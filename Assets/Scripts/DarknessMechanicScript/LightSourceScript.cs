using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceScript : MonoBehaviour
{
    [Header("variables")]
    [CannotBeNullObjectField] public Transform playerTransform;

    public LayerMask obstructionView;
    
    [System.Serializable]
    public struct LightData
    {
        public Light lightSource;
        [Range (0,40)] public float maxDistance;
        public GameObject lightCollider;
    }

    public LightData[] lightsArray;

    void Start()
    {
      
    }
    void Update()
    {
        BatchRaycastForLights(); 
    }

    void BatchRaycastForLights() //will need to make this better so that it doesn't happen every frame, kind of cringe ngl
    {
        
        foreach (LightData lightData in lightsArray)
        {
            Light lightSource = lightData.lightSource; //this is from the public struct

            float maxDistance = lightData.maxDistance; //also from the public struct
            
            lightSource.range = maxDistance;

            lightData.lightCollider.transform.localScale = new Vector3 (maxDistance,0,0); //clickty clackty, this is from the public structy

            Vector3 directionToPlayer = playerTransform.position - lightSource.transform.position;

            //the batch raycast from all light sources in the array will point towards the player and will do somehing if the object between the player and the  light source if it has the layer
            RaycastHit[] hits = Physics.RaycastAll(lightSource.transform.position, directionToPlayer, maxDistance, obstructionView); 

            Debug.DrawRay(lightSource.transform.position, directionToPlayer * maxDistance /2f, Color.green);

            foreach (RaycastHit hit in hits)
            {   
                //inside this logic this can be used for obstructing the view if there is an object with the obstruction layer
                Debug.Log("Light source: " + lightSource.name + " - Obstruction by: " + hit.collider.gameObject.name);
            }
        }
    }
}
