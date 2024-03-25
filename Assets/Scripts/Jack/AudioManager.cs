using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
    [Header("References")]
    private List<AudioSource> musicLayers = new List<AudioSource>();
    [SerializeField] private AudioSource introMusicSource;
    [Header("Variables")]
    public static AudioManager instance;
    public static float musicChannelVol = 1f;
    public static float soundeffectChannelVol = 1f;
    public AudioSource musicChannel;
    public AudioSource soundeffectChannel;
    public AudioSource weaveChannel;
    public AudioSource footStepsChannel;
    public AudioSource fallChannel;

    public bool transitioning; 
    public bool playingSequencedMusic;
    //AudioSource for the digging sound

    [Header("Music Audioclip List")]
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip alpineMusic;
    [SerializeField] private AudioClip alpineMusicIntro;
    [SerializeField] private AudioClip cavernMusic;
    [SerializeField] private AudioClip cavernIntroMusic;
    [SerializeField] private AudioClip sepultusMusic;
    [SerializeField] private AudioClip hubMusic;

  //  [Header("SFX")]
   // [SerializeField] private List<AudioClip> soundEffects = new List<AudioClip>();  
   

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

        instance.PlaySound(AudioManagerChannels.MusicChannel, titleMusic, 1f);
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
                    PlaySound(AudioManagerChannels.MusicChannel, titleMusic);
                    break;
                }
            case "AlpineCombined":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, alpineMusic);
                    introMusicSource.clip = alpineMusicIntro;
                    introMusicSource.outputAudioMixerGroup = musicChannel.outputAudioMixerGroup;
                    introMusicSource.Play();
                    PlaySequencedMusic(alpineMusicIntro);
                    break;
                }
            case "Cavern":
                {
                    PlaySound(AudioManagerChannels.MusicChannel, cavernMusic);
                    introMusicSource.clip = cavernIntroMusic;
                    introMusicSource.outputAudioMixerGroup = musicChannel.outputAudioMixerGroup;
                    introMusicSource.Play();
                    PlaySequencedMusic(cavernIntroMusic);
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

    public void StopSoundAfterLoop(AudioManagerChannels target)
    {
        StartCoroutine(WaitTillAfterLoop(target));
    }

    public IEnumerator WaitTillAfterLoop(AudioManagerChannels target)
    {
        switch (target)
        {
            case AudioManagerChannels.MusicChannel:
                musicChannel.loop = false;
                yield return new WaitUntil(() => !musicChannel.isPlaying);
                musicChannel.Stop();
                musicChannel.loop = true;
                break;
            case AudioManagerChannels.SoundEffectChannel:
                soundeffectChannel.loop = false;
                yield return new WaitUntil(() => !soundeffectChannel.isPlaying);
                soundeffectChannel.Stop();
                soundeffectChannel.loop = true;
                break;
            case AudioManagerChannels.weaveLoopingChannel:
                weaveChannel.loop = false;
                yield return new WaitUntil(() => !weaveChannel.isPlaying);
                weaveChannel.Stop();
                weaveChannel.loop = true;
                break;
            case AudioManagerChannels.footStepsLoopChannel:
                footStepsChannel.loop = false;
                yield return new WaitUntil(() => !footStepsChannel.isPlaying);
                footStepsChannel.Stop();
                footStepsChannel.loop = true;
                break;
            case AudioManagerChannels.fallLoopChannel:
                fallChannel.loop = false;
                yield return new WaitUntil(() => !fallChannel.isPlaying);
                fallChannel.Stop();
                fallChannel.loop = true;
                break;
        }

        yield break;
    }

    public void PlaySequencedMusic(AudioClip audioClip)
    {
        musicChannel.Stop();
        double introTime = (double)audioClip.samples / audioClip.frequency; // most accurate way to get to get the length of an audioclip as a double
        double startTime = AudioSettings.dspTime + 0.2; 
        musicChannel.PlayScheduled(startTime + introTime); // best way to play without a delay, using the usually Play() will add a delay! Fucked!
        playingSequencedMusic = true;
        StartCoroutine(WaitForSongIntroToEnd(audioClip.length));
    }

    private IEnumerator WaitForSongIntroToEnd(float audioClipLength)
    {
        double startTime = Time.time;
        while (Time.time < startTime + audioClipLength) // less accurate measure of time tracking, but not 'visible', so no worries
        {
            yield return null;
        }
        playingSequencedMusic = false;
    }

    public IEnumerator StartMusicFadeOut(AudioClip musicToTransitionTo, float fadeOutTransitionDuration, float fadeInTransitionDuration, float musicStartTime)
    {
        if (musicToTransitionTo.name == musicChannel.clip.name || playingSequencedMusic)
        {
            yield break;
        }

        transitioning = true;
        float time = 0;
        float start = musicChannel.volume;
        while (time < fadeOutTransitionDuration)
        {
            time += Time.deltaTime;
            musicChannel.volume = Mathf.Lerp(start, 0, time / fadeOutTransitionDuration);
            yield return null;
        }
        musicChannel.clip = musicToTransitionTo;
        musicChannel.time = musicStartTime;
        StartCoroutine(StartMusicFadeIn(fadeInTransitionDuration, start));
        musicChannel.Play();
        yield break;
    }

    public IEnumerator StartMusicFadeIn(float transitionDuration, float originalMusicVolume)
    {
        float time = 0;
        float start = musicChannel.volume;
        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            musicChannel.volume = Mathf.Lerp(start, originalMusicVolume, time / transitionDuration);
            yield return null;
        }
        transitioning = false;
        yield break;
    }

    public void ClearMusicLayers()
    {
        foreach (AudioSource musicLayer in musicLayers)
        {
            Destroy(musicLayer);
        }
        musicLayers.Clear();
    }

    public void AddMusicLayer(AudioClip newLayer, bool loop)
    {
        if (newLayer != null)
        {
            AudioSource musicLayer = gameObject.AddComponent(typeof (AudioSource)) as AudioSource;
            musicLayer.clip = newLayer;
            musicLayer.Play();
            musicLayer.outputAudioMixerGroup = musicChannel.outputAudioMixerGroup;

            if (loop)
            {
                musicLayer.loop = loop;
            }
            musicLayers.Add(musicLayer);
        }
    }

    public AudioSource AddSFX(AudioClip newSFX, bool loop, AudioSource lastInstanceToKill)
    {
        if (lastInstanceToKill)
        {
            KillAudioSource(lastInstanceToKill);
        }
        else
        {
            //Debug.LogWarning("No audio source instance to kill");
        }
        if (newSFX != null)
        {
            AudioSource sfx = gameObject.AddComponent(typeof (AudioSource)) as AudioSource;
            sfx.clip = newSFX;
            sfx.Play();
            sfx.outputAudioMixerGroup = soundeffectChannel.outputAudioMixerGroup;
            if (loop)
            {
                sfx.loop = true;
            }
            return sfx;
        }
        else
        {
            //Debug.LogWarning("No new sfx");
            return null;
        }
    }

    public AudioSource KillAudioSource(AudioSource audioSource)
    {
        if (audioSource)
        {
            //Debug.Log("Killed " + audioSource + " that was playing " + audioSource.clip.name);
            Destroy(audioSource);
        }
        return null;
    }
}
