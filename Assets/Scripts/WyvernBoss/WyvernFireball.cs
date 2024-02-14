using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFireball : MonoBehaviour
{
    private GameObject weaver;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        weaver = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        HomingMissile();
        if (transform.position.magnitude > 1000) {
            Destroy(gameObject);
        }
    }

    void HomingMissile() {

        transform.position = Vector3.MoveTowards(transform.position,weaver.transform.position, speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other) {
        if (!other.gameObject.CompareTag("Boss")) {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player")) {
            PlayerController playercontroller = other.gameObject.GetComponent<PlayerController>();
            playercontroller.Death();
        }
        
    }
}
