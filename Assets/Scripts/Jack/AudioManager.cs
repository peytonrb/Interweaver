using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AudioManagerChannels 
{
    MusicChannel = 0,
    SoundEffectChannel,
}


public class AudioManager : MonoBehaviour
{
    [Header("Variables")]
    public static AudioManager instance;

    public static float musicChannelVol = 1f;
    public static float soundeffectChannelVol = 1f;

    public AudioSource musicChannel;
    public AudioSource soundeffectChannel;

    [Header("Music Audioclip List")]
    public AudioClip TitleMusic;
    public AudioClip PactMusic;
    public AudioClip MerchantMusic;
    public AudioClip JoinMusic;
    public AudioClip DuelMusic;
    public AudioClip WinMusic;

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
            case "MenuScene":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, TitleMusic);
                    break;
                }
            case "JoinScene":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, JoinMusic);
                    break;
                }
            case "DuelScene":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, DuelMusic);
                    break;
                }
            case "PactScene":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, PactMusic);
                    break;
                }
            case "MerchantScene":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, MerchantMusic);
                    break;
                }
            case "WinScene":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, WinMusic);
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
                //soundeffectChannel.Stop();
                //soundeffectChannel.clip = clip;
                soundeffectChannel.pitch = pitch;
                soundeffectChannel.PlayOneShot(clip);
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
        }
    }

}
