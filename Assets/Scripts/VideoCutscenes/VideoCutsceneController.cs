using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoCutsceneController : MonoBehaviour
{
    public static int cutscene = 0;
    public bool isOnTrigger;
    public VideoClip[] clips;
    private VideoPlayer videoPlayer;
    [SerializeField] private bool playOnStart;
    private bool started;
    [SerializeField] private GameObject introController;

    // Start is called before the first frame update
    void Start()
    {
        if (isOnTrigger == false) {
            videoPlayer = GetComponent<VideoPlayer>();

            IntroSequenceScript introSequenceScript = introController.GetComponent<IntroSequenceScript>();
            bool gameStarted = introSequenceScript.GetGameStarted();

            if (gameStarted) {
                introController.SetActive(false);
                if (playOnStart == true) {
                    PlayVideo();
                }
            }
            else {
                gameObject.SetActive(false);
            }
        }
    }

    public void PlayVideo() {
        videoPlayer.clip = clips[cutscene];
        videoPlayer.Play();
    }

    void Update() {
        if (isOnTrigger == false) {
            if (started == true) {
                if (videoPlayer.isPlaying == false) {
                    switch (cutscene) {
                        case 0:
                            SceneHandler.instance.LoadLevel("Hub");
                        break;
                        case 1:
                            SceneHandler.instance.LoadLevel("AlpineCombined");
                        break;
                        case 2:
                            SceneHandler.instance.LoadLevel("Hub");
                        break;
                        case 3:
                            SceneHandler.instance.LoadLevel("Cavern");
                        break;
                        case 4:
                            SceneHandler.instance.LoadLevel("Hub");
                        break;
                        case 5:
                            SceneHandler.instance.LoadLevel("Sepultus");
                        break;
                        case 6:
                            SceneHandler.instance.LoadLevel("Menu");
                        break;       
                    }
                }
            }
            else {
                if (videoPlayer.isPlaying == true) {
                    started = true;
                }
            }
        }
        
    }

    public void SkipCutscene() {
        switch (cutscene) {
            case 0:
                SceneHandler.instance.LoadLevel("Hub");
            break;
            case 1:
                SceneHandler.instance.LoadLevel("AlpineCombined");
            break;
            case 2:
                SceneHandler.instance.LoadLevel("Hub");
            break;
            case 3:
                SceneHandler.instance.LoadLevel("Cavern");
            break;
            case 4:
                SceneHandler.instance.LoadLevel("Hub");
            break;
            case 5:
                SceneHandler.instance.LoadLevel("Sepultus");
            break;
            case 6:
                SceneHandler.instance.LoadLevel("Menu");
            break;       
        }
    }

    public void ChangeCutscene(int scene) {
        cutscene = scene;
    }
}
