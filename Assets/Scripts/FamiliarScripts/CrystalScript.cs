using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    public FloatingIslandScript myFloatingIsland;

    private bool inRange = false;

    public AudioClip ambientFile;

    public void AssignFloatingIsland(FloatingIslandScript myIsland)
    {
        myFloatingIsland = myIsland;
    }

    public void TriggerBreak()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, ambientFile, 1f);

        if (myFloatingIsland != null)
        {
            myFloatingIsland.StartFalling();
        }
        
        Destroy(gameObject);
    }

}
