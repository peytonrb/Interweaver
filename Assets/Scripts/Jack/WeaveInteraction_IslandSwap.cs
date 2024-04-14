using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveInteraction_IslandSwap : WeaveInteraction
{
    public override void OnWeave(GameObject other, GameObject wovenObject)
    {
        if (other.CompareTag("FloatingIsland"))
        {

            // drops weaveable from weaving control
            WeaveableManager.Instance.DestroyJoints(wovenObject.GetComponent<WeaveableObject>().weaveController.currentWeaveable.listIndex);
            wovenObject.GetComponent<WeaveableObject>().weaveController.OnDrop();

            // set to the children for the new weave prefab structure for rework
            other.transform.GetChild(0).GetComponent<FloatingIslandScript>().StartFloating(wovenObject);
        }
    }
}
