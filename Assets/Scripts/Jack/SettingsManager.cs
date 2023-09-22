using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Variables")]
    public AudioMixer theMixer;
    public Slider masterSlider, musicSlider, sfxSlider;
    public TMP_Text masterValueText, musicValueText, sfxValueText;

    [Header("FullScreen Variables")]
    public Toggle fullscreenToggle;
    [HideInInspector] public int fullscreenInt;
    public TMP_Dropdown resDropdown;
    [HideInInspector] public int resolutionType;

    [Header("Vsync Variables")]
    public Toggle vSyncToggle;
    [HideInInspector] public int vsyncInt;

    [Header("Tutorial Variables")]
    public Toggle tutorialToggle;
    [HideInInspector] public int tutorialInt;

    void Awake()
    {
        
        //Check if there is a key for the playerprefs for the fullscreen and set the int depending on it
        if (PlayerPrefs.HasKey("FullscreenToggleState"))
            fullscreenInt = PlayerPrefs.GetInt("FullscreenToggleState");
        else
            fullscreenInt = 1;

        if (fullscreenInt == 1)
        {
            fullscreenToggle.isOn = true;
            Screen.fullScreen = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
            Screen.fullScreen = false;
        }

        //Check if there is a key for the playerprefs for the vsync and set the int depending on it
        if (PlayerPrefs.HasKey("VsyncToggleState"))
            vsyncInt = PlayerPrefs.GetInt("VsyncToggleState");
        else
            vsyncInt = 1;

        if (vsyncInt == 1)
        {
            vSyncToggle.isOn = true;
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            vSyncToggle.isOn = false;
            QualitySettings.vSyncCount = 0;
        }

#if !UNITY_EDITOR
        //Check if there is a key for the playerprefs for the resolution and set the int depending on it
        if (PlayerPrefs.HasKey("resType"))
            resolutionType = PlayerPrefs.GetInt("resType");
        else
            resolutionType = 1;

        resDropdown.value = resolutionType;

        switch (resDropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(1920, 1080, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 1);
                    resolutionType = 1;
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(1600, 900, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 2);
                    resolutionType = 2;
                    break;
                }
            case 2:
                {
                    Screen.SetResolution(1366, 768, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 3);
                    resolutionType = 3;
                    break;
                }
            case 3:
                {
                    Screen.SetResolution(1280, 720, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 4);
                    resolutionType = 3;
                    break;
                }
        }
#endif

    }
    void Start()
    {
        //Check if the master volume has a key via playerprefs and adjusts it value according to value stored in key
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            theMixer.SetFloat("MasterVol", ConvertToLog(PlayerPrefs.GetFloat("MasterVolume")));
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        //Check if the music volume has a key via playerprefs and adjusts it value according to value stored in key
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            theMixer.SetFloat("MusicVol", ConvertToLog(PlayerPrefs.GetFloat("MusicVolume")));
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicSlider.value = -40f;
        }

        //Check if the sfx volume has a key via playerprefs and adjusts it value according to value stored in key
        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            theMixer.SetFloat("SFXVol", ConvertToLog(PlayerPrefs.GetFloat("SfxVolume")));
            sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        }
        else
        {
            sfxSlider.value = -40f;
        }

        //Adjust the value of the value texts to the right of the slider using the value stored in playerprefs
        masterValueText.text = (masterSlider.value + 80).ToString() + "%";
        musicValueText.text = (musicSlider.value + 80).ToString() + "%";
        sfxValueText.text = (sfxSlider.value + 80).ToString() + "%";
    }

    /// <summary>
    /// Adjusts fullscreen based on the parameter
    /// </summary>
    public void AdjustFullscreen(bool isFullscreenOn)
    {
        Screen.fullScreen = isFullscreenOn;

        if (isFullscreenOn == false)
        {
            isFullscreenOn = false;
            PlayerPrefs.SetInt("FullscreenToggleState", 0);
            Debug.Log("Exiting FullScreen");
        }
        else
        {
            isFullscreenOn = true;
            PlayerPrefs.SetInt("FullscreenToggleState", 1);
        }
    }
 
    /// <summary>
    /// Adjusts Vsync based on parameter passed
    /// </summary>
    public void AdjustVysnc(bool isVsyncOn)
    {
        if (isVsyncOn == false)
        {
            PlayerPrefs.SetInt("VsyncToggleState", 0);
            QualitySettings.vSyncCount = 0;
            Debug.Log("The Vsync is Off");
        }
        else
        {
            PlayerPrefs.SetInt("VsyncToggleState", 1);
            QualitySettings.vSyncCount = 1;
        }
    }

    /// <summary>
    /// Log conversion formula used for volume adjustment
    /// </summary>
    /// <param name="oldValue"> base volume number to be passed in</param>
    /// <returns></returns>
    public float ConvertToLog(float oldValue)
    {
        float newValue = ((oldValue - -80f) / (20f - -80f)) * (1f - 0.0001f) + 0.0001f;

        return Mathf.Log10(newValue) * 20;
    }

    //Function to adjust the master volume by readjusting the value text and slider and setting the float for playerprefs
    public void AdjustMasterVolume()
    {
        masterValueText.text = (masterSlider.value + 80).ToString() + "%";
        theMixer.SetFloat("MasterVol", ConvertToLog(masterSlider.value));
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    //Function to adjust the music volume by readjusting the value text and slider and setting the float for playerprefs
    public void AdjustMusicVolume()
    {
        musicValueText.text = (musicSlider.value + 80).ToString() + "%";
        theMixer.SetFloat("MusicVol", ConvertToLog(musicSlider.value));
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    //Function to adjust the sfx volume by readjusting the value text and slider and setting the float for playerprefs
    public void AdjustSfxVolume()
    {
        sfxValueText.text = (sfxSlider.value + 80).ToString() + "%";
        theMixer.SetFloat("SFXVol", ConvertToLog(sfxSlider.value));
        PlayerPrefs.SetFloat("SfxVolume", sfxSlider.value);
    }

    //Function to change screen resolution
    public void AdjustResolution()
    {


        switch (resDropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(1920, 1080, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 1);
                    resolutionType = 1;
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(1600, 900, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 2);
                    resolutionType = 2;
                    break;
                }
            case 2:
                {
                    Screen.SetResolution(1366, 768, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 3);
                    resolutionType = 3;
                    break;
                }
            case 3:
                {
                    Screen.SetResolution(1280, 720, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 4);
                    resolutionType = 3;
                    break;
                }
        }

    }
}