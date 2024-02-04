using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StalactiteScript : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFalling;
    private Vector3 pos;
    private Quaternion rot;
    [SerializeField] private float cooldown;
    public GameObject stalactite;
    public BoxCollider childtrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isFalling = false;

        pos = transform.position;
        rot = transform.rotation;
        childtrigger.center = new Vector3(0,-25,0);
        childtrigger.enabled = false;

        StartCoroutine(RegrowthCooldown());
    }

    public void Fall() {
        rb.constraints = RigidbodyConstraints.None;
        isFalling = true;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Familiar")) {
            Debug.Log("Death");
            Instantiate(stalactite,pos,Quaternion.identity);
            Destroy(gameObject);
        }
        else {
            Debug.Log("Miss");
            Instantiate(stalactite,pos,Quaternion.identity);
            Destroy(gameObject);
        }
    }

    IEnumerator RegrowthCooldown() {
        yield return new WaitForSeconds(cooldown);
        childtrigger.enabled = true;
    }
}
