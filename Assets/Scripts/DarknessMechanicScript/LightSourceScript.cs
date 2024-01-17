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

    void BatchRaycastForLights()
    {
        
        foreach (LightData lightData in lightsArray)
        {
            Light lightSource = lightData.lightSource;

            float maxDistance = lightData.maxDistance;
            
            lightSource.range = maxDistance;

            lightData.lightCollider.transform.localScale = new Vector3 (maxDistance,0,0);

            Vector3 directionToPlayer = playerTransform.position - lightSource.transform.position;
          

            RaycastHit[] hits = Physics.RaycastAll(lightSource.transform.position, directionToPlayer, maxDistance, obstructionView);

            Debug.DrawRay(lightSource.transform.position, directionToPlayer * maxDistance /2, Color.green);

            foreach (RaycastHit hit in hits)
            {        
                Debug.Log("Light source: " + lightSource.name + " - Obstruction by: " + hit.collider.gameObject.name);
            }
        }
    }
}
