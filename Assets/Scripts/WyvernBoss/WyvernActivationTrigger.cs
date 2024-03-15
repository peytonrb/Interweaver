using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernActivationTrigger : MonoBehaviour
{
    [SerializeField] private GameObject wyvern;
    [SerializeField] private bool destroyThisAfterActivation;

    // Start is called before the first frame update
    void Awake()
    {
       wyvern.SetActive(false); 
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Familiar")) {
            ActivateWyvern();
        }
    }

    public void ActivateWyvern() {
        wyvern.SetActive(true);

        if (destroyThisAfterActivation) {
            Destroy(gameObject);
        }
        else {
            gameObject.SetActive(false);
        }
        
    }

    public void ResetTrigger() {
        gameObject.SetActive(true);
        wyvern.SetActive(false);
    }
}
