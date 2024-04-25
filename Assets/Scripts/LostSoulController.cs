using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoulController : MonoBehaviour
{
    public float attractionStrength;
    public GameObject player;
    public int soulID; //First soul in the level should have ID = 0
    public Dialogue lostSoulDialogue;

    void FixedUpdate()
    {
        Vector3 directionToPlayer = transform.position - player.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        if (distanceToPlayer > 0.5 && distanceToPlayer <= 5)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            float attractionForce = attractionStrength / distanceToPlayer;
            gameObject.GetComponent<Rigidbody>().AddForce(directionToPlayer.normalized * -attractionForce);
        }
        else if (distanceToPlayer <= 0.5)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else if (distanceToPlayer > 5)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        }
    }

    public void DestroyMyself() {
        Destroy(gameObject);
    }
}
