using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFlamethrower : MonoBehaviour, ITriggerable
{
    private GameObject weaver;
    [SerializeField] private bool useDefaultPositionAndScale;
    [SerializeField] private Vector3 flamethrowerLocalPosition;
    [SerializeField] private Vector3 flamethrowerLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        //DEFAULT LOCAL POSITION AND SCALES
        if (useDefaultPositionAndScale == true) {
            transform.localPosition = new Vector3(0,-0.47f,1.3f);
            transform.localScale = new Vector3(0.05f,0.05f,2f);
        }
        else {
            transform.localPosition = flamethrowerLocalPosition;
            transform.localScale = flamethrowerLocalScale;
        }
        

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
