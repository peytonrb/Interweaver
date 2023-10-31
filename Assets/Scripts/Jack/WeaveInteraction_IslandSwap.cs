using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveInteraction_IslandSwap : WeaveInteraction
{
    public override void OnWeave(GameObject other)
    {
        if (other.CompareTag("FloatingIsland"))
        {
            other.GetComponent<FloatingIslandScript>().SwapToRiseCamera();
        }
    }
}
