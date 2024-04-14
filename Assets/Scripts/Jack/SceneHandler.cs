using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    ///<summary>
    /// Handles all scene-transitioning related logic with functions that are called from other scripts.
    /// Calls essential scene initialization-related functions in other singleton scripts when certain scenes finish loading
    ///</summary>


    public static SceneHandler instance;
    public string currentSceneName;
    public GameObject sceneLoader;
    public Slider progressBar;
    [HideInInspector] public AsyncOperation loadOperation;
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
        

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        //Debug.Log(mode);

        currentSceneName = scene.name;

        AudioManager.instance.PlayMusicOnSceneChange(scene.name);



        //why is this here?
        switch (scene.name)
        {
            case "Menu":
                {



                    break;
                }
        }
    }

    public void LoadLevel(string sceneToLoad)
    {

        StartCoroutine(LoadAsynchrously(sceneToLoad));

    }

    IEnumerator LoadAsynchrously(string sceneToLoad)
    {
         loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        sceneLoader.SetActive(true);


        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);

            progressBar.value = progress;

            yield return null;
        }

        if (loadOperation.isDone)
        {
            sceneLoader.SetActive(false);
        }
    }

}
