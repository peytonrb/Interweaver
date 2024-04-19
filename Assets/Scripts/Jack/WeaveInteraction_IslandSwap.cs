using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveInteraction_IslandSwap : WeaveInteraction
{
    public override void OnWeave(GameObject other, GameObject wovenObject)
    {
        if (other.CompareTag("FloatingIsland"))
        {
          
            // set to the children for the new weave prefab structure for rework
            other.transform.GetChild(0).GetComponent<FloatingIslandScript>().StartFloating(wovenObject);
        }
    }
}
