using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeIslandCrystalScript : MonoBehaviour, IDamageable
{

    public GameObject vfxPrefab;

    public AudioClip shatterFile;

    public FloatingIslandScript myFloatingIsland;
    public void Damage()
    {
        GameObject shatterVFX = Instantiate(vfxPrefab, transform.position, transform.rotation);
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, shatterFile, 1f);

        if (myFloatingIsland != null)
        {
            myFloatingIsland.StartFalling();
        }

        Destroy(shatterVFX.gameObject, 1f);
        gameObject.SetActive(false);

    }
}
