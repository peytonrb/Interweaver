using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    ///<summary>
    /// Handles all scene-transitioning related logic with functions that are called from other scripts.
    /// Calls essential scene initialization-related functions in other singleton scripts when certain scenes finish loading
    ///</summary>


    public static SceneHandler instance;
    public string currentSceneName;
    public Animator anim;

    public bool isInDuelScene = false;

    public List<string> duelScenes;

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        anim = GetComponentInChildren<Animator>();

 
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        //Debug.Log(mode);

        anim.SetTrigger("SceneStart");

        currentSceneName = scene.name;

        AudioManager.instance.PlayMusicOnSceneChange(scene.name);

        var cam = GameObject.Find("Camera Target").GetComponent<MultiplayerCam>();
        cam.players = new List<Transform>();

        for (int i = 0; i < PlayerManager.players.Count; i++)
        {
            cam.players.Add(PlayerManager.players[i].transform);
        }


        switch (scene.name)
        {
            case "MenuScene":
                {
                    isInDuelScene = false;
                    DuelManager.instance.SetWinnerToNull();
                    DuelManager.instance.DuelSceneEnded();
                    

                    break;
                }
            case "JoinScene":
                {
                    isInDuelScene = false;
                    DuelManager.instance.SetWinnerToNull();
                    break;
                }
            case "PactScene":
                {
                    isInDuelScene = false;
                    foreach (Player player in FindObjectsOfType<Player>())
                    {
                        player.ResetPlayerOnSceneChange();
                    }
                    DuelManager.instance.DuelSceneEnded();
                    DuelManager.instance.SetWinnerToNull();
                    UI.instance.EnablePactSceneUI();
                    break;
                }
            case "MerchantScene":
                {
                    isInDuelScene = false;
                    foreach (Player player in FindObjectsOfType<Player>())
                    {
                        player.MoveToSpawnPoint();
                        player.ResetPlayerOnSceneChange();
                        player.GetComponent<PlayerUIControl>().canReadyUp = true;
                    }

                    DDOLCamera.instance.GetComponentInChildren<MultiplayerCam>().ResetFocus(true);
                    DuelManager.instance.DuelSceneEnded();
                    DuelManager.instance.SetWinnerToNull();
                    UI.instance.EnableMerchantSceneUI();
                    break;
                }
            case "WinScene":
                {
                    isInDuelScene = false;
                    DuelManager.instance.DuelSceneEnded();
                    UI.instance.EnableWinSceneUI();
                    break;
                }
            default:
                {
                    //Duel Scene
                    
                    isInDuelScene = true;
                    DuelManager.instance.PactSceneEnded();
                    DuelManager.instance.MerchantSceneEnded();
                    DuelManager.instance.DuelSceneStarted();
                    DuelManager.instance.SetWinnerToNull();
                    DDOLCamera.instance.GetComponentInChildren<AssignCameraBounds>().AssignCamBounds();

                    foreach (Player player in FindObjectsOfType<Player>())
                    {
                        player.ResetPlayerOnSceneChange();
                        player.GetComponent<PlayerUIControl>().SetPlayerUI();
                    }

                    StartCoroutine(DelayPlayerAnims());
                    
                    break;
                }
        }
    }

    public IEnumerator DelayPlayerAnims()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (Player player in FindObjectsOfType<Player>())
        {
            player.SpawnAnimation();

        }

        yield break;
    }

    public IEnumerator LoadLevel(string sceneToLoad)
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
        yield break;
    }

    public void GoToMenuScene()
    {
        Time.timeScale = 1f;
        GameObject duelManager = GameObject.Find("DuelManager");
        duelManager.GetComponent<DuelManager>().DestroyAll();
        anim.SetTrigger("SceneEnd");
        StartCoroutine(LoadLevel("MenuScene"));
    }

    public void GoToJoinScene()
    {
        Time.timeScale = 1f;
        anim.SetTrigger("SceneEnd");
        StartCoroutine(LoadLevel("JoinScene"));
    }

    public void GoToDuelScene()
    {
        anim.SetTrigger("SceneEnd");
        StartCoroutine(LoadLevel(duelScenes[Random.Range(0,duelScenes.Count)]));
    }

    public void GoToPactScene()
    {
        anim.SetTrigger("SceneEnd");
        StartCoroutine(LoadLevel("PactScene"));
    }

    public void GoToMerchantScene()
    {
        anim.SetTrigger("SceneEnd");
        StartCoroutine(LoadLevel("MerchantScene"));
    }

    public void GoToWinScene()
    {
        anim.SetTrigger("SceneEnd");
        StartCoroutine(LoadLevel("WinScene"));
    }

}
