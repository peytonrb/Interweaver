using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustPushScript : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        // here is where you'd put something that'd control the wind's speed/direction
        // but that's for smarter people than me
        // yippeeeeeeee
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            collider.GetComponent<CharacterController>().Move(gameObject.transform.localRotation * direction.normalized * (speed/100));
        }
    }

    void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(gameObject.transform.position, gameObject.transform.localRotation * direction.normalized); 
    }
}
