using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour, IDamageable
{
    public FloatingIslandScript myFloatingIsland;

    private bool inRange = false;

    public GameObject vfxPrefab;

    public AudioClip shatterFile;

    public void AssignFloatingIsland(FloatingIslandScript myIsland)
    {
        myFloatingIsland = myIsland;
    }

    public void Damage()
    {
        GameObject shatterVFX = Instantiate(vfxPrefab, transform.position, transform.rotation);
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, shatterFile, 1f);

        if (myFloatingIsland != null)
        {
            myFloatingIsland.StartFalling(this);
        }

        Destroy(shatterVFX.gameObject, 1f);
       gameObject.SetActive(false);
    }
    

}
