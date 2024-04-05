using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationCrystalScript : MonoBehaviour
{
   
    public GameObject linkedCrystal;
   [HideInInspector] public bool isTeleportable = true;
    private WeaveController weaveController;

    //if doing it to a combined object it breaks and the code actually kills itself
    // it says something about weaveableManager line 51

    private void Start()
    {
        weaveController = InputManagerScript.instance.player.GetComponent<WeaveController>();
    }
    public void TeleportFunction(GameObject other)
    {
        //this takes the current object (teleport crystal) and then tells the weaveable object to go to the linked crystal
     other.GetComponent<WeaveableObject>().objectToSnapTo.transform.position = new Vector3(linkedCrystal.transform.position.x,
     linkedCrystal.transform.position.y, linkedCrystal.transform.position.z + 3);


        //audio here

        StartCoroutine(DisconnectOnTeleport());
     
    }

    public void TeleportToFunction(GameObject other)
    {
         //this takes the current object (weaveable) and then tells the teleport crystal to go to the linked crystal
        other.GetComponent<WeaveableObject>().transform.position = new Vector3(linkedCrystal.transform.position.x,
                   linkedCrystal.transform.position.y, linkedCrystal.transform.position.z + 3);


        //audio here
        StartCoroutine(DisconnectOnTeleport());

    }

    public IEnumerator DisconnectOnTeleport()
    {
        WeaveableManager.Instance.DestroyJoints(weaveController.currentWeaveable.listIndex);

        weaveController.OnDrop();

        yield break;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawArrow.ForGizmo(transform.position,Vector3.forward * 3);
    }
}
