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
    [CannotBeNullObjectField][SerializeField] private GameObject inputManager;

    [SerializeField] private GameObject keyboardGroup;
    [SerializeField] private GameObject controllerGroup;

    [SerializeField] private GameObject ControllerImage;
    [SerializeField] private GameObject KeyboardImage;

    [SerializeField] private GameObject spiderBoss;
    [HideInInspector] public bool changeSpiderlings;
    [HideInInspector] public bool changeSpiderlingsTo; //Changes arachnophobia on the spider (either to true or false).
    [SerializeField] private GameObject subtitlesCanvas;

    private bool hasControllerInvoke;

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
    [SerializeField] private Button defaultControlsKeyboardButton;
    [SerializeField] private Button defaultControlsControllerButton;

    [Header("Debugging")]
    [SerializeField] private bool arachnophobiaState;
    [SerializeField] private bool subtitlesState;
    [SerializeField] private bool fullscreenState;
    [SerializeField] private bool vysncState;
    [SerializeField] private float masterVolState;
    [SerializeField] private float musicVolState;
    [SerializeField] private float sfxVolState;

    void Awake()
    {
        Debug.Log("Res: " + PlayerPrefs.GetInt("resType"));
    }

    public void Start()
    {
        toggle = GetComponentInChildren<Toggle>();
        eventSystem = FindObjectOfType<EventSystem>();
        hasControllerInvoke = false;
        


        if (spiderBoss != null)
        {
            // if (PlayerPrefs.HasKey("ArachnophobiaToggleState"))
            // {
            //     arachnophobiaInt = PlayerPrefs.GetInt("ArachnophobiaToggleState");
            // }
            // else
            // {
            //     //Arachnophobia setting stays off by default
            //     arachnophobiaInt = 0;
            // }

            // if (arachnophobiaInt == 0)
            // {
            //     SpiderBossScript spiderscript = spiderBoss.GetComponent<SpiderBossScript>();
            //     if (spiderscript != null)
            //     {
            //         spiderscript.ToggleArachnophobia(false);
            //     }
            //     arachnophobiaToggle.isOn = false;
            //     arachnophobiaState = false;
            // }
            // else
            // {
            //     SpiderBossScript spiderscript = spiderBoss.GetComponent<SpiderBossScript>();
            //     if (spiderscript != null)
            //     {
            //         spiderscript.ToggleArachnophobia(true);
            //     }
            //     arachnophobiaToggle.isOn = true;
            //     arachnophobiaState = true;
            // }

            if (SceneHandler.instance.arachnophobiaState)
            {
                SpiderBossScript spiderscript = spiderBoss.GetComponent<SpiderBossScript>();
                if (spiderscript != null)
                {
                    spiderscript.ToggleArachnophobia(true);
                }
                arachnophobiaToggle.isOn = true;
                arachnophobiaState = true;
            }
            else
            {
                SpiderBossScript spiderscript = spiderBoss.GetComponent<SpiderBossScript>();
                if (spiderscript != null)
                {
                    spiderscript.ToggleArachnophobia(false);
                }
                arachnophobiaToggle.isOn = false;
                arachnophobiaState = false;
            }
        }

        if (subtitlesCanvas != null)
        {
            // if (PlayerPrefs.HasKey("SubtitlesToggleState"))
            // {
            //     subtitlesInt = PlayerPrefs.GetInt("SubtitlesToggleState");
            // }
            // else
            // {
            //     subtitlesInt = 0;
            // }

            // if (subtitlesInt == 1)
            // {
            //     subtitlesToggle.isOn = true;
            //     subtitlesCanvas.SetActive(true);
            //     subtitlesState = true;
            // }
            // else
            // {
            //     subtitlesToggle.isOn = false;
            //     subtitlesCanvas.SetActive(false);
            //     subtitlesState = false;
            // }

            if (SceneHandler.instance.subtitlesState)
            {
                subtitlesToggle.isOn = true;
                subtitlesCanvas.SetActive(true);
                subtitlesState = true;
            }
            else
            {
                subtitlesToggle.isOn = false;
                subtitlesCanvas.SetActive(false);
                subtitlesState = false;
            }
        }

        // //Check if there is a key for the playerprefs for the fullscreen and set the int depending on it
        // if (PlayerPrefs.HasKey("FullscreenToggleState"))
        //     fullscreenInt = PlayerPrefs.GetInt("FullscreenToggleState");
        // else
        //     fullscreenInt = 1;

        // if (fullscreenInt == 1)
        // {
        //     fullscreenToggle.isOn = true;
        //     Screen.fullScreen = true;
        //     fullscreenState = true;
        // }
        // else
        // {
        //     fullscreenToggle.isOn = false;
        //     Screen.fullScreen = false;
        //     fullscreenState = false;
        // }

        if (SceneHandler.instance.fullscreenState)
        {
            fullscreenToggle.isOn = true;
            Screen.fullScreen = true;
            fullscreenState = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
            Screen.fullScreen = false;
            fullscreenState = false;
        }

        //Check if there is a key for the playerprefs for the vsync and set the int depending on it
        // if (PlayerPrefs.HasKey("VsyncToggleState"))
        //     vsyncInt = PlayerPrefs.GetInt("VsyncToggleState");
        // else
        //     vsyncInt = 1;

        // if (vsyncInt == 1)
        // {
        //     vSyncToggle.isOn = true;
        //     QualitySettings.vSyncCount = 1;
        //     vysncState = true;
        // }
        // else
        // {
        //     vSyncToggle.isOn = false;
        //     QualitySettings.vSyncCount = 0;
        //     vysncState = false;
        // }

        if (SceneHandler.instance.vysncState)
        {
            vSyncToggle.isOn = true;
            QualitySettings.vSyncCount = 1;
            vysncState = true;
        }
        else
        {
            vSyncToggle.isOn = false;
            QualitySettings.vSyncCount = 0;
            vysncState = false;
        }

        //Check if the master volume has a key via playerprefs and adjusts it value according to value stored in key
        // if (PlayerPrefs.HasKey("MasterVolume"))
        // {
        //     theMixer.SetFloat("MasterVol", ConvertToLog(PlayerPrefs.GetFloat("MasterVolume")));
        //     masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        //     masterVolState = PlayerPrefs.GetFloat("MasterVolume");
        // }

        theMixer.SetFloat("MasterVol", ConvertToLog(SceneHandler.instance.masterVolState));
        masterSlider.value = SceneHandler.instance.masterVolState;

        //Check if the music volume has a key via playerprefs and adjusts it value according to value stored in key
        // if (PlayerPrefs.HasKey("MusicVolume"))
        // {
        //     theMixer.SetFloat("MusicVol", ConvertToLog(PlayerPrefs.GetFloat("MusicVolume")));
        //     musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        //     musicVolState = PlayerPrefs.GetFloat("MusicVolume");
        // }
        // else
        // {
        //     musicSlider.value = -40f;
        // }
        theMixer.SetFloat("MusicVol", ConvertToLog(SceneHandler.instance.musicVolState));
        musicSlider.value = SceneHandler.instance.musicVolState;

        //Check if the sfx volume has a key via playerprefs and adjusts it value according to value stored in key
        // if (PlayerPrefs.HasKey("SfxVolume"))
        // {
        //     theMixer.SetFloat("SFXVol", ConvertToLog(PlayerPrefs.GetFloat("SfxVolume")));
        //     sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        //     sfxVolState = PlayerPrefs.GetFloat("SfxVolume");
        // }
        // else
        // {
        //     sfxSlider.value = -40f;
        // }

        theMixer.SetFloat("SFXVol", ConvertToLog(SceneHandler.instance.sfxVolState));
        sfxSlider.value = SceneHandler.instance.sfxVolState;

        //Adjust the value of the value texts to the right of the slider using the value stored in playerprefs
        masterValueText.text = (masterSlider.value + 80).ToString() + "%";
        musicValueText.text = (musicSlider.value + 80).ToString() + "%";
        sfxValueText.text = (sfxSlider.value + 80).ToString() + "%";

    }

    private void Update()
    {
        CheckIfControllerOn();

    }
    //Resumes the game
    public void Resume()
    {
        Time.timeScale = 1;
        if (optionGroup.activeSelf)
        {
            optionGroup.SetActive(false);
            optionGroup.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (controlsPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
            controlsPanel.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (defaultGroup.activeSelf == false)
        {
            defaultGroup.SetActive(true);
            defaultGroup.GetComponent<CanvasGroup>().alpha = 1;
        }
        InputManagerScript.instance.canSwitch = true;
        if (InputManagerScript.instance.familiar.GetComponent<FamiliarScript>().myTurn)
            InputManagerScript.instance.playerInput.SwitchCurrentActionMap("Familiar");
        else
            InputManagerScript.instance.playerInput.SwitchCurrentActionMap("Weaver");
        gameObject.SetActive(false);
    }

    //Switches between using controller and using keyboard
    public void CheckIfControllerOn()
    {


        if (Gamepad.current == null && hasControllerInvoke)
        {
            ControllerImage.SetActive(false);
            KeyboardImage.SetActive(true);
            hasControllerInvoke = false;

        }
        else if (Gamepad.current != null && !hasControllerInvoke)
        {
            ControllerImage.SetActive(true);
            KeyboardImage.SetActive(false);
            hasControllerInvoke = true;
            defaultMenuButton.Select();
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
            Debug.Log("GWUH");
            resDropdown.value = PlayerPrefs.GetInt("resType");

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

            defaultMenuButton.Select();
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
                defaultControlsControllerButton.Select();
            }
            else
            {
                controllerGroup.SetActive(false);
                keyboardGroup.SetActive(true);
                defaultControlsKeyboardButton.Select();
            }
        }
    }

    public void ResetToCheckpoint()
    {
        InputManagerScript.instance.ResetCurrentCharacter();
    }

    /// <summary>
    /// Quits to either the menu or hub scene.
    /// </summary>
    /// <param name="level">
    /// 0 for Menu, 1 for Hub
    /// </param>
    public void QuitToScene(int level)
    {
        switch (level)
        {
            case 0:
                Time.timeScale = 1;
                SceneHandler.instance.LoadLevel("Menu");
                break;
            case 1:
                Time.timeScale = 1;
                SceneHandler.instance.LoadLevel("Hub");
                break;
        }

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
            SceneHandler.instance.fullscreenState = false;
        }
        else
        {
            isFullscreenOn = true;
            PlayerPrefs.SetInt("FullscreenToggleState", 1);
            SceneHandler.instance.fullscreenState = true;
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
            SceneHandler.instance.vysncState = false;
        }
        else
        {
            PlayerPrefs.SetInt("VsyncToggleState", 1);
            QualitySettings.vSyncCount = 1;
            SceneHandler.instance.vysncState = true;
        }
    }

    public void AdjustArachnophobia()
    {
        if (arachnophobiaToggle.isOn == false)
        {
            PlayerPrefs.SetInt("ArachnophobiaToggleState", 0);
            SceneHandler.instance.arachnophobiaState = false;
            if (spiderBoss != null)
            {
                SpiderBossScript spiderboss = spiderBoss.GetComponent<SpiderBossScript>();
                spiderboss.ToggleArachnophobia(false);
                changeSpiderlings = true;
                changeSpiderlingsTo = false;
            }
        }
        else
        {
            PlayerPrefs.SetInt("ArachnophobiaToggleState", 1);
            SceneHandler.instance.arachnophobiaState = true;
            if (spiderBoss != null)
            {
                SpiderBossScript spiderboss = spiderBoss.GetComponent<SpiderBossScript>();
                spiderboss.ToggleArachnophobia(true);
                changeSpiderlings = true;
                changeSpiderlingsTo = true;
            }
        }
    }

    public void AdjustSubtitles()
    {
        if (subtitlesToggle.isOn == false)
        {
            PlayerPrefs.SetInt("SubtitlesToggleState", 0);
            if (subtitlesCanvas != null)
            {
                subtitlesCanvas.SetActive(false);
            }
            SceneHandler.instance.subtitlesState = false;
        }
        else
        {
            PlayerPrefs.SetInt("SubtitlesToggleState", 1);
            if (subtitlesCanvas != null)
            {
                subtitlesCanvas.SetActive(true);
            }
            SceneHandler.instance.subtitlesState = true;
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
        SceneHandler.instance.masterVolState = masterSlider.value;
    }

    //Function to adjust the music volume by readjusting the value text and slider and setting the float for playerprefs
    public void AdjustMusicVolume()
    {
        musicValueText.text = (musicSlider.value + 80).ToString() + "%";
        theMixer.SetFloat("MusicVol", ConvertToLog(musicSlider.value));
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        SceneHandler.instance.musicVolState = musicSlider.value;
    }

    //Function to adjust the sfx volume by readjusting the value text and slider and setting the float for playerprefs
    public void AdjustSfxVolume()
    {
        sfxValueText.text = (sfxSlider.value + 80).ToString() + "%";
        theMixer.SetFloat("SFXVol", ConvertToLog(sfxSlider.value));
        PlayerPrefs.SetFloat("SfxVolume", sfxSlider.value);
        SceneHandler.instance.sfxVolState = sfxSlider.value;
    }

    //Function to change screen resolution
    public void AdjustResolution()
    {
        switch (resDropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(1920, 1080, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 0);
                    resolutionType = PlayerPrefs.GetInt("resType");
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(1600, 900, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 1);
                    resolutionType = PlayerPrefs.GetInt("resType");
                    break;
                }
            case 2:
                {
                    Screen.SetResolution(1366, 768, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 2);
                    resolutionType = PlayerPrefs.GetInt("resType");
                    break;
                }
            case 3:
                {
                    Screen.SetResolution(1280, 720, fullscreenToggle.isOn);
                    PlayerPrefs.SetInt("resType", 3);
                    resolutionType = PlayerPrefs.GetInt("resType");
                    break;
                }
        }
    }
}
