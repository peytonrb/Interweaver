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
                    wovenObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                    hoverScript.StartHover(wovenObject);
                }
            }

            if (wovenObject.TryGetComponent<HoverCrystalScript>(out hoverScript))
            {
                if (!hoverScript.hoverBegan)
                {
                    // drops weaveable from weaving control
                    other.GetComponent<WeaveableObject>().weaveController.OnDrop();
                    hoverScript.StartHover(other);
                }
            }
        }
    }
}
