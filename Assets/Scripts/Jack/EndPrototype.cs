using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPrototype : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneHandler.instance.LoadLevel("ArtTestScene");
        }
    }
}
