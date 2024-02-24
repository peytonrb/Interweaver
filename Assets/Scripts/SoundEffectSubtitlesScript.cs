using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundEffectSubtitlesScript : MonoBehaviour
{
    [SerializeField] private GameObject[] audiosources;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject asgo in audiosources) {
            TextMeshProUGUI audiosourcetext = asgo.GetComponent<TextMeshProUGUI>();
            audiosourcetext.text = "";
            asgo.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (AudioManager.instance.musicChannel.isPlaying) {
            UpdateText(audiosources[0],AudioManagerChannels.MusicChannel);
        }
        if (AudioManager.instance.soundeffectChannel.isPlaying) {
            UpdateText(audiosources[1],AudioManagerChannels.SoundEffectChannel);
        }
        if (AudioManager.instance.weaveChannel.isPlaying) {
            UpdateText(audiosources[2],AudioManagerChannels.weaveLoopingChannel);
        }
        if (AudioManager.instance.footStepsChannel.isPlaying) {
            UpdateText(audiosources[3],AudioManagerChannels.footStepsLoopChannel);
        }
        if (AudioManager.instance.fallChannel.isPlaying) {
            UpdateText(audiosources[4],AudioManagerChannels.fallLoopChannel);
        }

        if (!AudioManager.instance.musicChannel.isPlaying) {
            TurnTextOff(audiosources[0]);
        }
        if (!AudioManager.instance.soundeffectChannel.isPlaying) {
            TurnTextOff(audiosources[1]);
        }
        if (!AudioManager.instance.weaveChannel.isPlaying) {
            TurnTextOff(audiosources[2]);
        }
        if (!AudioManager.instance.footStepsChannel.isPlaying) {
            TurnTextOff(audiosources[3]);
        }
        if (!AudioManager.instance.fallChannel.isPlaying) {
            TurnTextOff(audiosources[4]);
        }
        
    }

    //Updates the text with the name of the sound clip
    void UpdateText(GameObject thisobject, AudioManagerChannels audiochannel) {

        thisobject.SetActive(true);
        TextMeshProUGUI audiosourcetext = thisobject.GetComponent<TextMeshProUGUI>();

        switch (audiochannel) {
            case AudioManagerChannels.MusicChannel:
                if (AudioManager.instance.musicChannel.clip != null) {
                    string audioname = AudioManager.instance.musicChannel.clip.name;
                    audiosourcetext.text = audioname;
                }
            break;
            case AudioManagerChannels.SoundEffectChannel:
                if (AudioManager.instance.soundeffectChannel.clip != null) {
                    string audioname = AudioManager.instance.soundeffectChannel.clip.name;
                    audiosourcetext.text = audioname;
                }
            break;
            case AudioManagerChannels.weaveLoopingChannel:
                if (AudioManager.instance.weaveChannel.clip != null) {
                    string audioname = AudioManager.instance.weaveChannel.clip.name;
                    audiosourcetext.text = audioname;
                }
            break;
            case AudioManagerChannels.footStepsLoopChannel:
                if (AudioManager.instance.footStepsChannel.clip != null) {
                    string audioname = AudioManager.instance.footStepsChannel.clip.name;
                    if (audioname == "Env_GrassStep") {
                        audiosourcetext.text = "Footsteps";
                    }
                    else {
                        audiosourcetext.text = audioname;
                    }
                    
                }
            break;
            case AudioManagerChannels.fallLoopChannel:
                if (AudioManager.instance.fallChannel.clip != null) {
                    string audioname = AudioManager.instance.fallChannel.clip.name;
                    audiosourcetext.text = audioname;
                }
            break;
        }
        
    }

    //Turns off the text if clip is not playing
    void TurnTextOff(GameObject thisobject) {
        thisobject.SetActive(false);
    }

}
