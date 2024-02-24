using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveInteraction_TeleportCrystal : WeaveInteraction
{
    public override void OnWeave(GameObject other, GameObject wovenObject)
    {
        if (!other.TryGetComponent<FloatingIslandScript>(out FloatingIslandScript amongus))
        {
            TeleportationCrystalScript teleportationCrystalScript;

            if (other.TryGetComponent<TeleportationCrystalScript>(out teleportationCrystalScript))
            {
                if (teleportationCrystalScript.isTeleportable)
                {
                    teleportationCrystalScript.TeleportToFunction(wovenObject);
                    Debug.Log("teleportation interaction called 1");
                }
            }

            if (wovenObject.TryGetComponent<TeleportationCrystalScript>(out teleportationCrystalScript))
            {
                if (teleportationCrystalScript.isTeleportable)
                {
                    teleportationCrystalScript.TeleportFunction(wovenObject);
                    Debug.Log("teleportation interaction called 2");
                }
            }
        }
          
    }
}
