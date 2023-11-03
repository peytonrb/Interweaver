using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    public FloatingIslandScript myFloatingIsland;

    private bool inRange = false;

    public void AssignFloatingIsland(FloatingIslandScript myIsland)
    {
        myFloatingIsland = myIsland;
    }

    public void TriggerBreak()
    {
        if (myFloatingIsland != null)
        {
            myFloatingIsland.StartFalling();
        }
        
        Destroy(gameObject);
    }

}
