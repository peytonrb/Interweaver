using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustPushScript : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;
    private List<GameObject> onBelt = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        // here is where you'd put something that'd control the wind's speed/direction
        // but that's for smarter people than me
        // yippeeeeeeee
    }

    void OnTriggerEnter(Collider collider)
    {
        onBelt.Add(collider.gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        onBelt.Remove(collider.gameObject);
    }

    void OnTriggerStay(Collider collider)
    {
        for (int i = 0; i <= onBelt.Count - 1; i++)
        {
            onBelt[i].GetComponent<CharacterController>().Move(gameObject.transform.localRotation * direction.normalized * (speed/100));
        }
    }

    void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(gameObject.transform.position, gameObject.transform.localRotation * direction.normalized); 
    }
}
