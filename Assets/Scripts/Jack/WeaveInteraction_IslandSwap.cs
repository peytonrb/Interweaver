using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveInteraction_IslandSwap : WeaveInteraction
{
    public override void OnWeave(GameObject other, GameObject wovenObject)
    {
        if (other.CompareTag("FloatingIsland"))
        {
            other.GetComponent<FloatingIslandScript>().SwapToRiseCamera();
            other.GetComponent<FloatingIslandScript>().AssignNewCrystal(wovenObject.GetComponent<CrystalScript>());
        }
    }
}
