using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationCrystalScript : MonoBehaviour
{
   
    public GameObject linkedCrystal;
   [HideInInspector] public bool isTeleportable = true;
    public WeaveController weaveController;

    public void TeleportFunction(GameObject other)
    {        
     other.GetComponent<WeaveableObject>().objectToSnapTo.transform.position = new Vector3(linkedCrystal.transform.position.x,
     linkedCrystal.transform.position.y, linkedCrystal.transform.position.z + 3);

     weaveController.currentWeaveable.ResetWeaveable();
        //need to somehow stop the vfx
     WeaveableManager.Instance.DestroyJoints(weaveController.currentWeaveable.listIndex);
    }

    public void TeleportToFunction(GameObject other)
    {
        other.GetComponent<WeaveableObject>().transform.position = new Vector3(linkedCrystal.transform.position.x,
                   linkedCrystal.transform.position.y, linkedCrystal.transform.position.z + 3);

        weaveController.currentWeaveable.ResetWeaveable();
        //need to somehow stop the vfx
        WeaveableManager.Instance.DestroyJoints(weaveController.currentWeaveable.listIndex);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawArrow.ForGizmo(transform.position,Vector3.forward * 3);
    }
}
