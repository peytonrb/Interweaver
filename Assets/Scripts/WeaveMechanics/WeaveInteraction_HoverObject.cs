using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveInteraction_HoverObject : WeaveInteraction
{
    public override void OnWeave(GameObject other, GameObject wovenObject)
    {

        if (!other.TryGetComponent<FloatingIslandScript>(out FloatingIslandScript amongus))
        {
            
            HoverCrystalScript hoverScript;

            if (other.TryGetComponent<HoverCrystalScript>(out hoverScript))
            {
                if (!hoverScript.hoverBegan)
                {
                    Debug.Log("weave interaction called");
                    hoverScript.StartHover(wovenObject);
                }
                
            }

            if (wovenObject.TryGetComponent<HoverCrystalScript>(out hoverScript))
            {
                if (!hoverScript.hoverBegan)
                {
                    Debug.Log("weave interaction called 2");
                    hoverScript.StartHover(other);
                }
            }

        }
    }
}
