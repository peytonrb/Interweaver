using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoulController : MonoBehaviour
{
    public float attractionStrength;
    public GameObject player;

    void FixedUpdate()
    {
        Vector3 directionToPlayer = transform.position - player.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        if (distanceToPlayer > 0.5 && distanceToPlayer < 5)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            float attractionForce = attractionStrength / distanceToPlayer;
            gameObject.GetComponent<Rigidbody>().AddForce(directionToPlayer.normalized * -attractionForce);
        }
        else if (distanceToPlayer <= 0.5)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
