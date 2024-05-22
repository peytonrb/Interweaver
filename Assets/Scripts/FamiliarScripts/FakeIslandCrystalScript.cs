using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeIslandCrystalScript : MonoBehaviour, IDamageable
{

    public GameObject vfxPrefab;

    [SerializeField] private AudioClip shatterFile;
    private AudioSource shatterSource;

    public FloatingIslandScript myFloatingIsland;

    public GameObject gustsToSpawn;

    void  Start()
    {
        shatterSource = null;
    }
    public void Damage()
    {
        GameObject shatterVFX = Instantiate(vfxPrefab, transform.position, transform.rotation);
        shatterSource = AudioManager.instance.AddSFX(shatterFile, false, shatterSource);

        if (myFloatingIsland != null)
        {
            myFloatingIsland.StartFalling();
        }

        if (gustsToSpawn != null)
        {
            gustsToSpawn.SetActive(true);
        }

        Destroy(shatterVFX.gameObject, 1f);
        gameObject.SetActive(false);

    }
}
