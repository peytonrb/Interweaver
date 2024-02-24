using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFlamethrower : MonoBehaviour, ITriggerable
{
    private GameObject weaver;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(0,-0.47f,1.3f);
        transform.localScale = new Vector3(0.05f,0.05f,2f);

        weaver = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnTrigEnter(Collider other) {
        if (other.gameObject.CompareTag("Hazard")) {
            PlayerController player = weaver.GetComponent<PlayerController>();
            player.Death();
        }
    }

    public void OnTrigExit(Collider other) {

    }

    public void KillThyself() {
        Destroy(gameObject);
    }
}
