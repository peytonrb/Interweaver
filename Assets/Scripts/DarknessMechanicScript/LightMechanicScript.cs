using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceScript : MonoBehaviour
{
    [Header("variables")]
    [CannotBeNullObjectField] public Transform playerTransform;

    public LayerMask obstructionView;

    [Range(0, 40)] public float maxDistance;

    [Header("light collider")]

    [CannotBeNullObjectField] public GameObject lightCollider;

    Light lt;

    private Vector3 lightScale;
    // Start is called before the first frame update
    void Start()
    {
       Light lt = GetComponent<Light>();

        lt.range = maxDistance;

        lightScale = new Vector3 (maxDistance, 0, 0);

        lightCollider.transform.localScale = lightScale * 2;
    }

    // Update is called once per frame
    void Update()
    {
        CheckObstruction();
    }

    void CheckObstruction()
    {
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light lightSource in lights)
        {

            Vector3 directionToPlayer = playerTransform.position - lightSource.transform.position;

            RaycastHit[] hits = Physics.RaycastAll(lightSource.transform.position, directionToPlayer, maxDistance, obstructionView);

            Debug.DrawRay(lightSource.transform.position, directionToPlayer, Color.green);

            foreach (RaycastHit hit in hits)
            {
                Debug.Log("Light source: " + lightSource.name + " - Obstruction by: " + hit.collider.gameObject.name);
            }
        }
    }
}
