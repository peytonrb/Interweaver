using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class VideoCutsceneController : MonoBehaviour
{
    public static int cutscene = 0;
    public bool isOnTrigger;
    public VideoClip[] clips;
    private VideoPlayer videoPlayer;
    [SerializeField] private bool playOnStart;
    private bool started;

    // Start is called before the first frame update
    void Start()
    {
        if (isOnTrigger == false) {
            videoPlayer = GetComponent<VideoPlayer>();

            if (playOnStart == true) {
                PlayVideo();
            }
        }
    }

    public void PlayVideo() {
        videoPlayer.clip = clips[cutscene];
        videoPlayer.Play();
    }

    void Update() {
        if (started == true) {
            if (videoPlayer.isPlaying == false) {
                switch (cutscene) {
                    case 0:
                        SceneManager.LoadScene("Hub");
                    break;
                    case 1:
                        SceneManager.LoadScene("AlpineCombined");
                    break;
                    case 2:
                        SceneManager.LoadScene("Hub");
                    break;
                    case 3:
                        SceneManager.LoadScene("Cavern");
                    break;
                    case 4:
                        SceneManager.LoadScene("Hub");
                    break;
                    case 5:
                        SceneManager.LoadScene("Sepultus");
                    break;
                    case 6:
                        SceneManager.LoadScene("Menu");
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

    public void ChangeCutscene(int scene) {
        cutscene = scene;
    }
}
