using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerScript : MonoBehaviour
{
    [Header("References")]
    private AudioManager audioManager;
    private Bounds bounds;
    [Header("Modes & Settings")]
    [SerializeField] private bool transitionMode;
    [SerializeField] private bool addLayerMode;
    [SerializeField] private bool trackSongPosition;
    [SerializeField] [Tooltip("Clears previous musical layers.")] private bool clearPreviousLayers; 
    [Header("Generic Variables")]
    [SerializeField] AudioClip musicToTransitionTo;
    private bool tripped; 
    public List<GameObject> currentCharacters = new List<GameObject>();
    private float songTime;
    [Header("Transition Variables")]
    [SerializeField][Range (0.1f, 10f)] float fadeOutTransitionDuration = 2f; 
    [SerializeField][Range (0.1f, 10f)] float fadeInTransitionDuration = 5f;
    [Header("Add Layer Variables")]
    [SerializeField] private List<AudioClip> layersToAdd = new List<AudioClip>();


    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
        bounds = gameObject.GetComponent<Collider>().bounds;
    }

    void Update()
    {
        if (tripped)
        {
            if (trackSongPosition)
            {
                songTime = audioManager.musicChannel.time;
            }
            
            foreach (GameObject character in currentCharacters)
            {
                if (!bounds.Contains(character.transform.position))
                {
                    tripped = false;
                    currentCharacters.Remove(character);
                    break; // quite frankly I don't really fully understand why break is needed here, but it snuffs an outta bounds error so yay?
                }
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!tripped) // if a trigger is newly entered
        {
            if (gameObject.GetComponent<Collider>().bounds.Contains(collider.transform.position))
            {
                MovementScript colliderMovementScript = collider.gameObject.GetComponent<MovementScript>();

                if (colliderMovementScript.active && !currentCharacters.Contains(collider.gameObject) && !audioManager.transitioning)
                {
                    ChangeTrack();
                    tripped = true;
                    currentCharacters.Add(collider.gameObject);
                }
            }
        }
        else
        {
            MovementScript colliderMovementScript = collider.gameObject.GetComponent<MovementScript>();

            if (!colliderMovementScript.active && currentCharacters.Contains(colliderMovementScript.gameObject))
            {
                tripped = false;
                currentCharacters.Remove(collider.gameObject);
            }
        }
    }

    private void ChangeTrack()
    {
        if (clearPreviousLayers)
        {
            audioManager.ClearMusicLayers();
        }
        if (transitionMode)
        {
            StartCoroutine(audioManager.StartMusicFadeOut(musicToTransitionTo, fadeOutTransitionDuration, fadeInTransitionDuration, songTime));
        }
        if (addLayerMode)
        {
            foreach (AudioClip layerToAdd in layersToAdd)
            {
                audioManager.AddMusicLayer(layerToAdd, true);
            }
        }
    }
}
