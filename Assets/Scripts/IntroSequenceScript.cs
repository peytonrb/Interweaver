using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroSequenceScript : MonoBehaviour
{
    public static bool gameStarted = false;
    private VideoPlayer videoPlayer;
    [SerializeField] private GameObject panel;
    private CanvasGroup canvasGroup;
    [SerializeField] private float transitionSpeed;
    private bool videoStarted;


    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        canvasGroup = panel.GetComponent<CanvasGroup>();
        videoStarted = false;
    }

    public bool GetGameStarted() 
    {
        return gameStarted;
    }

    public void GameStarted() 
    {
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.isPlaying == true) {
            videoStarted = true;
        }

        if (videoStarted == true && videoPlayer.isPlaying == false) {
            if (canvasGroup.alpha < 1) {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1.0f, transitionSpeed * Time.deltaTime);
            }
            else {
                GameStarted();
                SceneManager.LoadScene("Menu");
            }
        }

    }

    public void DestroySelf() 
    {
        Destroy(gameObject);
    }
}
