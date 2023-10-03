using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoulController : MonoBehaviour
{
    public float attractionStrength;
    public GameObject player;

    void Update()
    {
        Vector3 directionToPlayer = transform.position - player.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        if (distanceToPlayer > 0 && distanceToPlayer < 5)
        {
            float attractionForce = attractionStrength / distanceToPlayer;
            gameObject.GetComponent<Rigidbody>().AddForce(directionToPlayer.normalized * -attractionForce);
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;
        }
    }
}
