using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerScript : MonoBehaviour
{
    [Header("References")]
    private AudioManager audioManager;
    [Header("Modes & Settings")]
    [SerializeField] private bool transitionMode;
    [SerializeField] private bool addLayerMode;
    [SerializeField] [Tooltip("Clears previous musical layers.")] private bool clearPreviousLayers; 
    [Header("Generic Variables")]
    [SerializeField] AudioClip musicToTransitionTo;
    [SerializeField] bool active = true;
    private AudioClip previousMusic; // save the music that was removed if we wish to go back to it at any point
    [Header("Transition Variables")]
    [SerializeField][Range (0.1f, 10f)] float fadeOutTransitionDuration = 2f; 
    [SerializeField][Range (0.1f, 10f)] float fadeInTransitionDuration = 5f;
    [Header("Add Layer Variables")]
    [SerializeField] private List<AudioClip> layersToAdd = new List<AudioClip>();

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }
    
    private void OnTriggerEnter(Collider collider)
    { 
        if ((collider.CompareTag("Player") || collider.CompareTag("Familiar")) && active)
        {
            active = false;
            if (clearPreviousLayers)
            {
                audioManager.ClearMusicLayers();
            }
            if (transitionMode)
            {
                previousMusic = audioManager.musicChannel.clip;
                StartCoroutine(audioManager.StartMusicFadeOut(musicToTransitionTo, fadeOutTransitionDuration, fadeInTransitionDuration));
            }
            if (addLayerMode)
            {
                audioManager.AddMusicLayer(layersToAdd[0], true);
            }
        }
    }
}
