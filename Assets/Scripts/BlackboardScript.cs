using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BlackboardScript : MonoBehaviour
{
    [SerializeField] private GameObject interactiveBlackboardUI;
    [SerializeField] private CinemachineVirtualCamera blackboardCamera;
    [SerializeField] private CinemachineBrain mainCamera;
    [SerializeField] private GameObject playerstuff;
    private PlayerData playerdata;
    [Tooltip ("If true, then you are currently staring at the blackboard, and blackboard functionality is on.")] public bool onBlackboard;
    private int levelsCompleted;

    // Start is called before the first frame update
    void Start()
    {
        playerdata = playerstuff.GetComponent<PlayerData>();

        interactiveBlackboardUI.SetActive(false);
        blackboardCamera.Priority = 0;
        onBlackboard = false;
        levelsCompleted = playerdata.GetLevelsCompleted();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToFromBlackboard(MovementScript movementScript) {
        if (onBlackboard == false) {
            movementScript.ToggleCanLook(false);
            movementScript.ToggleCanMove(false);
            blackboardCamera.Priority = 2;
            StartCoroutine(WaitForBlendToFinish());
            onBlackboard = true;
        }
        else {
            interactiveBlackboardUI.SetActive(false);
            movementScript.ToggleCanLook(true);
            movementScript.ToggleCanMove(true);
            blackboardCamera.Priority = 0;
            onBlackboard = false;
        }
    }

    /// <summary>
    /// Goes to the level specified by index.
    /// 0 is for Alpine, 1 is for Cavern.
    /// </summary>
    /// <param name="levelNumber"></param>
    public void GoToLevel(int levelNumber) {
        if (onBlackboard == true) {
            switch (levelNumber) {
                case 0:
                    //GO TO ALPINE
                    if (levelNumber <= levelsCompleted) {
                        SceneHandler.instance.LoadLevel("AlpineCombined");
                    }
                break;
                case 1:
                    //GO TO CAVERN
                    if (levelNumber <= levelsCompleted) {
                        SceneHandler.instance.LoadLevel("Cavern");
                    }
                break;
            }
        }
        
    }

    IEnumerator WaitForBlendToFinish() {
        yield return null;
        while (mainCamera.IsBlending) {
            yield return null;
        }
        if (onBlackboard) {
            interactiveBlackboardUI.SetActive(true);
        }
        yield break;
    }
}
