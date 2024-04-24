using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip clipToPlay;
    private bool isPlaying = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlaying)
        {
            isPlaying = true;
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, clipToPlay);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlaying = false;
        }
    }
}
