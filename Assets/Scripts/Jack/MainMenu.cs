using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Variables")]
    public GameObject[] buttons;
    public CanvasGroup mainMenu, creditsMenu, optionsMenu;
    public GameObject TitleLogo;
    public AudioClip soundFile;
    public AudioClip hoverFile;

    EventSystem eventSystem;
    void Start()
    {
        eventSystem = EventSystem.current;
        optionsMenu.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    //Function for switching the active button using the eventsystem using a parameter
    public void ChangeActiveButtons(int buttonToChoose)
    {
        //eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(buttons[buttonToChoose]);
        Debug.Log("Changed active button to " +buttons[buttonToChoose].name);
    }

    //Function for turning off start screen and opening the main menu on by setting alpha to 1
    public void OpenMainMenu()
    {
        mainMenu.alpha = 1;
        mainMenu.blocksRaycasts = true;
        TitleLogo.SetActive(true);
        PlayButtonSound();
    }

    //Function for closing the main menu by setting alpha to 0 and allowing for clicks to not be made
    public void CloseMainMenu()
    {
        PlayButtonSound();
        mainMenu.alpha = 0;
        mainMenu.blocksRaycasts = false;
        TitleLogo.SetActive(false);
    }

    //Function for opening the options menu by setting alpha to 1 and allowing for clicks to be made
    public void OpenOptions()
    {
        PlayButtonSound();
        optionsMenu.alpha = 1;
        optionsMenu.blocksRaycasts = true;
        optionsMenu.gameObject.SetActive(true);
        ChangeActiveButtons(1);
        CloseMainMenu();
    }


    //Function for closing the options menu by setting alpha to 0 and allowing for clicks to not be made
    public void CloseOptions()
    {
        PlayButtonSound();
        optionsMenu.alpha = 0;
        optionsMenu.blocksRaycasts = false;
        EventSystem.current.SetSelectedGameObject(null);
        optionsMenu.gameObject.SetActive(false);
        OpenMainMenu();

    }

    //Function for credits the options menu by setting alpha to 0 and allowing for clicks to not be made
    public void OpenCredits()
    {
        PlayButtonSound();
        creditsMenu.alpha = 1;
        creditsMenu.blocksRaycasts = true;
        CloseMainMenu();

    }

    //Function for credits the options menu by setting alpha to 0 and allowing for clicks to not be made
    public void CloseCredits()
    {
        PlayButtonSound();
        creditsMenu.alpha = 0;
        creditsMenu.blocksRaycasts = false;

        EventSystem.current.SetSelectedGameObject(null);
        OpenMainMenu();
    }

    public void PlayButtonSound()
    {
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, soundFile, 1f);
    }

    //Function for quitting out of the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    //Function for playing sound when a button is pressed
    public void PlayButtonPress()
    {
        // Start Game!
        SceneHandler.instance.LoadLevel("sc_Prototype");
    }
    public void PlayHoverSound()
    {
        //Debug.Log("Hover Played");
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, hoverFile, 1f);
    }

    //Detect conntroller input
    public void MoveSelect(InputAction input)
    {
        if (input.IsPressed() && eventSystem.currentSelectedGameObject == null)
        {
            if (!optionsMenu.gameObject.activeInHierarchy)
            {
                ChangeActiveButtons(0);
            } else {
                ChangeActiveButtons(1);
            }
            
        }
    }
}
