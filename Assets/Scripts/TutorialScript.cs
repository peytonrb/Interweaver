using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TutorialScript : MonoBehaviour
{
    public UnityEvent triggerEventWeaver; 
    public UnityEvent triggerEventFamiliar;
    // Start is called before the first frame update
    public  void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            triggerEventWeaver.Invoke();
        }
        if (collider.gameObject.tag == "Familiar")
        {
            triggerEventFamiliar.Invoke();
        }
    }
}
