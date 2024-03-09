using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlackboardScript : MonoBehaviour
{
    [SerializeField] private GameObject interactableUI;
    [SerializeField] private CinemachineVirtualCamera blackboardCamera;
    [SerializeField] private CinemachineBrain mainCamera;
    [SerializeField] private GameObject playerstuff;
    [SerializeField] private TextMeshProUGUI lostSoulCount;
    public GameObject popupUIPrompt;
    private PlayerData playerdata;
    [HideInInspector] [Tooltip ("If true, then you are currently staring at the blackboard, and blackboard functionality is on.")] public bool onBlackboard;
    private int levelsCompleted;
    private int lostSoulTotal;

    // Start is called before the first frame update
    void Start()
    {
        playerdata = playerstuff.GetComponent<PlayerData>();

        interactableUI.SetActive(false);
        blackboardCamera.Priority = 0;
        onBlackboard = false;
        levelsCompleted = playerdata.GetLevelsCompleted();
        lostSoulTotal = playerdata.GetLostSoulCount();
        lostSoulCount.text = "TOTAL LOST SOULS: " + lostSoulTotal;
        popupUIPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            var weaverNPCInteraction = InputManagerScript.instance.playerInput.actions["NPCInteraction"].GetBindingDisplayString();

            popupUIPrompt.SetActive(true);

            popupUIPrompt.gameObject.transform.GetComponent<TMP_Text>().SetText("<sprite name=" + weaverNPCInteraction + ">"
                        + " Interact");
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            popupUIPrompt.SetActive(false);
        }
    }

    public void GoToFromBlackboard(MovementScript movementScript) {
        if (onBlackboard == false) {
            movementScript.ToggleCanLook(false);
            movementScript.ToggleCanMove(false);
            blackboardCamera.Priority = 2;
            StartCoroutine(WaitForBlendToFinish());
            if (popupUIPrompt.activeSelf) {
                popupUIPrompt.SetActive(false);
            }
            onBlackboard = true;
        }
        else {
            interactableUI.SetActive(false);
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
            interactableUI.SetActive(true);
        }
        yield break;
    }
}
