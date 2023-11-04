using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    public FloatingIslandScript myFloatingIsland;

    private bool inRange = false;

    public AudioClip shatterFile;

    public void AssignFloatingIsland(FloatingIslandScript myIsland)
    {
        myFloatingIsland = myIsland;
    }

    public void TriggerBreak()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, shatterFile, 1f);

        if (myFloatingIsland != null)
        {
            myFloatingIsland.StartFalling();
        }
        
        Destroy(gameObject);
    }

}
