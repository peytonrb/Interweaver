using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyCrystalsScript : MonoBehaviour, IDamageable
{
    private bool isRespawning;

    private bool hasInvoked;

    public GameObject vfxPrefab;

    public AudioClip shatterFile;
    // Start is called before the first frame update
    void Start()
    {
        hasInvoked = false;
        isRespawning = false;
    }

    public void Damage()
    {
        GameObject shatterVFX = Instantiate(vfxPrefab, transform.position, transform.rotation);
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, shatterFile, 1f);      

        Destroy(shatterVFX.gameObject, 1f);
        gameObject.SetActive(false);
    }
    
}
