using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StalactiteTriggerScript : MonoBehaviour, ITriggerable
{
    [SerializeField] private float timer;
    private bool timerOn;

    //WE WANT THE THING TO FALL REGARDLESS IF THE PLAYER WALKS OUTSIDE OF THE TRIGGER

    // Start is called before the first frame update
    void Start()
    {
        timerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                StalactiteScript stalscript = GetComponentInParent<StalactiteScript>();
                stalscript.Fall();
                Destroy(gameObject);
            }
        }
    }

    public void OnTrigEnter(Collider other) {
        if (other.gameObject.CompareTag("Stalactite")) {
            timerOn = true;
        }
    }
    
    public void OnTrigExit(Collider other) {
        
    }

}
