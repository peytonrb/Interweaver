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
    [HideInInspector] public AsyncOperation loadOperation = null;

    // pause stuff (i couldnt find a better solution tehe)
    public bool arachnophobiaState = false;
    public bool subtitlesState = false;
    public bool fullscreenState = true;
    public bool vysncState = false;
    public float masterVolState = 0f;
    public float musicVolState = 0f;
    public float sfxVolState = 0f;

    private bool isLoading = false;
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

    }

    public void LoadLevel(string sceneToLoad)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadAsynchrously(sceneToLoad));
            isLoading = true;
        }
        

    }

    IEnumerator LoadAsynchrously(string sceneToLoad)
    {

        sceneLoader.SetActive(true);

        loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);

            progressBar.value = progress;

            yield return null;
        }

        if (loadOperation.isDone)
        {
            isLoading = false;
            sceneLoader.SetActive(false);
        }
    }

}
