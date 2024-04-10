using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{   
    private bool usingController = false;
    private Toggle toggle;
    private EventSystem eventSystem;

    [SerializeField] private GameObject optionGroup;
    [SerializeField] private GameObject defaultGroup;
    [SerializeField] private GameObject controlsPanel;
    [CannotBeNullObjectField] [SerializeField] private GameObject inputManager;

    [SerializeField] private GameObject keyboardGroup;
    [SerializeField] private GameObject controllerGroup;

    [SerializeField] private GameObject ControllerImage;
    [SerializeField] private GameObject KeyboardImage;

    [SerializeField] private GameObject spiderBoss;
    [SerializeField] private GameObject subtitlesCanvas;

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

    [Header("Arachnophobia Setting")]
    public Toggle arachnophobiaToggle;
    [HideInInspector] public int arachnophobiaInt;

    [Header("Subtitles")]
    public Toggle subtitlesToggle;
    [HideInInspector] public int subtitlesInt;

    [Header("Button UI Selection On Screen Change")]
    [SerializeField] private Button defaultMenuButton;
    [SerializeField] private Button defaultOptionsButton;

    public void Start()
    {
        toggle = GetComponentInChildren<Toggle>();
        eventSystem = FindObjectOfType<EventSystem>();

        if (spiderBoss != null) {
            if (PlayerPrefs.HasKey("ArachnophobiaToggleState")) {
                arachnophobiaInt = PlayerPrefs.GetInt("ArachnophobiaToggleState");
            }
            else {
                //Arachnophobia setting stays off by default
                arachnophobiaInt = 0;
            }

            if (arachnophobiaInt == 0) {
                SpiderBossScript spiderscript = spiderBoss.GetComponent<SpiderBossScript>();
                if (spiderscript != null) {
                    spiderscript.ToggleArachnophobia(false);
                }
                arachnophobiaToggle.isOn = false;
                
            }
            else {
                SpiderBossScript spiderscript = spiderBoss.GetComponent<SpiderBossScript>();
                if (spiderscript != null) {
                    spiderscript.ToggleArachnophobia(true);
                }
                arachnophobiaToggle.isOn = true;
            }
        }

        if (subtitlesCanvas != null) {
            if (PlayerPrefs.HasKey("SubtitlesToggleState")) {
                subtitlesInt = PlayerPrefs.GetInt("SubtitlesToggleState");
            }  
            else {
                subtitlesInt = 0;
            } 

            if (subtitlesInt == 1) {
                subtitlesToggle.isOn = true;
                subtitlesCanvas.SetActive(true);
            }
            else {
                subtitlesToggle.isOn = false;
                subtitlesCanvas.SetActive(false);
            }
        }

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
    //Resumes the game
    public void Resume() {
        Time.timeScale = 1;
        if (optionGroup.activeSelf) {
            optionGroup.SetActive(false);
            optionGroup.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (controlsPanel.activeSelf) {
            controlsPanel.SetActive(false);
            controlsPanel.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (defaultGroup.activeSelf == false) {
            defaultGroup.SetActive(true);
            defaultGroup.GetComponent<CanvasGroup>().alpha = 1;
        }
        InputManagerScript.instance.canSwitch = true;
        gameObject.SetActive(false);
    }

    //Switches between using controller and using keyboard
    public void ToggleUsingController(bool isController)
    {
        if (isController)
        {
            if (Gamepad.current == null)
            {
                toggle.isOn = false;
            }
            else
            {
                InputManagerScript.instance.ToggleControlScheme(true);
                ControllerImage.SetActive(true);
                KeyboardImage.SetActive(false);
                usingController = true;
            }
        }
        else
        {
            if (Keyboard.current == null)
            {
                toggle.isOn = true;
            }
            else
            {
                InputManagerScript.instance.ToggleControlScheme(false);
                ControllerImage.SetActive(false);
                KeyboardImage.SetActive(true);
                usingController = false;
            }
        }

    }

    public void ToggleOptions()
    {
        if (optionGroup.activeSelf)
        {
            optionGroup.SetActive(false);
            defaultGroup.SetActive(true);

            optionGroup.GetComponent<CanvasGroup>().alpha = 0f;
            defaultGroup.GetComponent<CanvasGroup>().alpha = 1f;

            defaultMenuButton.Select();
        }
        else
        {
            defaultGroup.SetActive(false);
            optionGroup.SetActive(true);

            optionGroup.GetComponent<CanvasGroup>().alpha = 1f;
            defaultGroup.GetComponent<CanvasGroup>().alpha = 0f;

            defaultOptionsButton.Select();
        }
    }

    public void ToggleControls() 
    {
        if (controlsPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
            defaultGroup.SetActive(true);
            inputManager.SetActive(true);

            controlsPanel.GetComponent<CanvasGroup>().alpha = 0f;
            defaultGroup.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else 
        {
            controlsPanel.SetActive(true);
            defaultGroup.SetActive(false);
            inputManager.SetActive(false);

            controlsPanel.GetComponent<CanvasGroup>().alpha = 1f;
            defaultGroup.GetComponent<CanvasGroup>().alpha = 0f;

            if (usingController)
            {
                controllerGroup.SetActive(true);
                keyboardGroup.SetActive(false);
            }
            else 
            {
                controllerGroup.SetActive(false);
                keyboardGroup.SetActive(true);
            }
        }
    }

    public void ResetToCheckpoint()
    {
        InputManagerScript.instance.ResetCurrentCharacter();
    }

    public void QuitToMenu()
    {
        SceneHandler.instance.LoadLevel("Menu");
    }

    /// <summary>
    /// Adjusts fullscreen based on the parameter
    /// </summary>
    public void AdjustFullscreen(bool isFullscreenOn)
    {
        Screen.fullScreen = fullscreenToggle.isOn;

        if (fullscreenToggle.isOn == false)
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

    public void AdjustArachnophobia() {
        if (arachnophobiaToggle.isOn == false) {
            PlayerPrefs.SetInt("ArachnophobiaToggleState",0);
            if (spiderBoss != null) {
                SpiderBossScript spiderboss = spiderBoss.GetComponent<SpiderBossScript>();
                spiderboss.ToggleArachnophobia(false);
            }
        }
        else {
            PlayerPrefs.SetInt("ArachnophobiaToggleState",1);
            if (spiderBoss != null) {
                SpiderBossScript spiderboss = spiderBoss.GetComponent<SpiderBossScript>();
                spiderboss.ToggleArachnophobia(true);
            }
        }
    }

    public void AdjustSubtitles() {
        if (subtitlesToggle.isOn == false) {
            PlayerPrefs.SetInt("SubtitlesToggleState", 0);
            if (subtitlesCanvas != null) {
                subtitlesCanvas.SetActive(false);
            }
        }
        else {
            PlayerPrefs.SetInt("SubtitlesToggleState", 1);
            if (subtitlesCanvas != null) {
                subtitlesCanvas.SetActive(true);
            }
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
