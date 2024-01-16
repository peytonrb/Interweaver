using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour, ITriggerable
{
    [CannotBeNullObjectField] public GameObject cutsceneManager;

    public void OnTrigEnter(Collider other) {
        if (other.gameObject.tag == "CutsceneTrigger") {
            CutsceneManagerScript cms = cutsceneManager.GetComponent<CutsceneManagerScript>();
            cms.StartCutscene();    
        }
    }

    public void OnTrigExit(Collider other) {

    }

}
