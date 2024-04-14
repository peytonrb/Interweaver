using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip breakOpenSound;
    public void BreakMyObject()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, breakOpenSound, 1f);
        Destroy(gameObject);
    }
}
