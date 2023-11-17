using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AudioManagerChannels 
{
    MusicChannel = 0,
    SoundEffectChannel,
    weaveLoopingChannel,
    footStepsLoopChannel,
    fallLoopChannel
}


public class AudioManager : MonoBehaviour
{
    [Header("Variables")]
    public static AudioManager instance;

    public static float musicChannelVol = 1f;
    public static float soundeffectChannelVol = 1f;

    public AudioSource musicChannel;
    public AudioSource soundeffectChannel;
    public AudioSource weaveChannel;
    public AudioSource footStepsChannel;
    public AudioSource fallChannel;

    [Header("Music Audioclip List")]
    public AudioClip TitleMusic;
    public AudioClip alpineMusic;
   

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicChannel = GetComponents<AudioSource>()[0];
        soundeffectChannel = GetComponents<AudioSource>()[1];
        weaveChannel = GetComponents<AudioSource>()[2];
        footStepsChannel = GetComponents<AudioSource>()[3];
        fallChannel = GetComponents<AudioSource>()[4];

        instance.PlaySound(AudioManagerChannels.MusicChannel, TitleMusic, 1f);
    }
    /// <summary>
    /// Plays the set music based on the scene passed
    /// </summary>
    /// <param name="sceneName">The name of the scene</param>
    public void PlayMusicOnSceneChange(string sceneName)
    {
        StopSound(AudioManagerChannels.MusicChannel);

        switch (sceneName) 
        {
            case "Menu":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, TitleMusic);
                    break;
                }
            case "AlpineCombined":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, alpineMusic);
                    break;
                }
        }
    }
    /// <summary>
    /// Sets the volume of the channel
    /// </summary>
    /// <param name="target">Target channel 0 = music, 1 = SFX</param>
    /// <param name="value">Value from 0 - 1</param>
    public static void SetChannelVolume(int target, float value)
    {
        switch (target)
        {
            case 0:
                musicChannelVol = value;
                instance.musicChannel.volume = musicChannelVol;
                break;
            case 1:
                soundeffectChannelVol = value;
                instance.soundeffectChannel.volume = soundeffectChannelVol;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Toggles looping music
    /// </summary>
    public void SetMusicLoop()
    {
        musicChannel.loop = !musicChannel.loop;
    }

    /// <summary>
    /// Plays an audio on a specified channel.
    /// </summary>
    /// <param name="target">Targetted Channel</param>
    /// <param name="clip">Audioclip to play</param>
    public void PlaySound(AudioManagerChannels target, AudioClip clip)
    {
        switch (target)
        {
            case AudioManagerChannels.MusicChannel:
                musicChannel.Stop();
                musicChannel.clip = clip;
                musicChannel.Play();
                break;
            case AudioManagerChannels.SoundEffectChannel:
                soundeffectChannel.PlayOneShot(clip);
                break;
            case AudioManagerChannels.weaveLoopingChannel:
                weaveChannel.clip = clip;
                weaveChannel.Play();
                break;
            case AudioManagerChannels.footStepsLoopChannel:
                footStepsChannel.clip = clip;
                footStepsChannel.Play();
                break;
            case AudioManagerChannels.fallLoopChannel:
                fallChannel.clip = clip;
                fallChannel.Play();
                break;
        }
    }

    /// <summary>
    /// Plays an audio on a specified channel.
    /// </summary>
    /// <param name="target">Targetted Channel</param>
    /// <param name="clip">Audioclip to play</param>
    /// <param name="pitch">Pitch to adjust the audio to (-3 to 3)</param>
    public void PlaySound(AudioManagerChannels target, AudioClip clip, float pitch)
    {
        switch (target)
        {
            case AudioManagerChannels.MusicChannel:
                musicChannel.Stop();
                musicChannel.clip = clip;
                musicChannel.pitch = pitch;
                musicChannel.Play();
                break;
            case AudioManagerChannels.SoundEffectChannel:
                soundeffectChannel.pitch = pitch;
                soundeffectChannel.PlayOneShot(clip);
                break;
            case AudioManagerChannels.weaveLoopingChannel:
                weaveChannel.pitch = pitch;
                weaveChannel.clip = clip;
                weaveChannel.Play();
                break;
            case AudioManagerChannels.footStepsLoopChannel:
                footStepsChannel.pitch = pitch;
                footStepsChannel.clip = clip;
                footStepsChannel.Play();
                break;
            case AudioManagerChannels.fallLoopChannel:
                fallChannel.pitch = pitch;
                fallChannel.clip = clip;
                fallChannel.Play();
                break;
        }
    }
    /// <summary>
    /// Stops audio on specified channel
    /// </summary>
    /// <param name="target">Channel to stop audio on</param>
    public void StopSound(AudioManagerChannels target)
    {
        switch (target)
        {
            case AudioManagerChannels.MusicChannel:
                musicChannel.Stop();
                break;
            case AudioManagerChannels.SoundEffectChannel:
                soundeffectChannel.Stop();
                break;
            case AudioManagerChannels.weaveLoopingChannel:
                weaveChannel.Stop();
                break;
            case AudioManagerChannels.footStepsLoopChannel:
                footStepsChannel.Stop();
                break;
            case AudioManagerChannels.fallLoopChannel:
                fallChannel.Stop();
                break;
        }
    }

}
