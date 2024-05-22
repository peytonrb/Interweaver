using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXonDestroy : MonoBehaviour
{
    public GameObject vfxSpawn;

    private void OnDestroy()
    {
        Instantiate(vfxSpawn, transform.position, transform.rotation);
    }
}
