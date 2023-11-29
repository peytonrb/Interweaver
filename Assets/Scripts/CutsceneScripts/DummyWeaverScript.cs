using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DummyWeaverScript : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;
    //private CutsceneManagerScript cms;
    private PlayableDirector director;
    private bool playerNotRelocated; //fail safe in case timeline were to loop

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        //cms = GetComponentInParent<CutsceneManagerScript>();
        director = GetComponentInParent<PlayableDirector>();

        

        playerNotRelocated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf) {
            if (director.time >= 7.25f && playerNotRelocated) {
                playerController.SetNewPosition(transform.position,transform.rotation);
            }
        }
    }
}
