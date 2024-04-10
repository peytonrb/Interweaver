using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlackboardScript : MonoBehaviour
{
    [SerializeField] private GameObject interactableUI;
    [SerializeField] private CinemachineVirtualCamera blackboardCamera;
    [SerializeField] private CinemachineBrain mainCamera;
    [SerializeField] private TextMeshProUGUI lostSoulCountAlpine;
    [SerializeField] private TextMeshProUGUI lostSoulCountCavern;
    [SerializeField] private TextMeshProUGUI lostSoulCountSepultus;
    [SerializeField] private Button defaultButton;
    [SerializeField] private GameObject pressEToExit;
    private AnimaticCutsceneController acc;
    private VideoCutsceneController vcc;
    public GameObject popupUIPrompt;
    [HideInInspector] [Tooltip ("If true, then you are currently staring at the blackboard, and blackboard functionality is on.")] public bool onBlackboard;
    private int levelsCompleted;
    private int lostSoulTotalAlpine;
    private int lostSoulTotalCavern;
    private int lostSoulTotalSepultus;
    public bool DebugLoadAnyLevel = true;

    // Start is called before the first frame update
    void Start()
    {
        acc = GetComponent<AnimaticCutsceneController>();
        vcc = GetComponent<VideoCutsceneController>();

        interactableUI.SetActive(false);
        pressEToExit.SetActive(false);
        blackboardCamera.Priority = 0;
        onBlackboard = false;
        levelsCompleted = PlayerData.instance.GetLevelsCompleted();
        lostSoulTotalAlpine = PlayerData.instance.GetAlpineLostSoulCount();
        lostSoulTotalCavern = PlayerData.instance.GetCavernLostSoulCount();
        lostSoulTotalSepultus = PlayerData.instance.GetSepultusLostSoulCount();
        
        lostSoulCountAlpine.text = "SOULS COLLECTED: " + lostSoulTotalAlpine;
        lostSoulCountCavern.text = "SOULS COLLECTED: " + lostSoulTotalCavern;
        lostSoulCountSepultus.text = "SOULS COLLECTED: " + lostSoulTotalSepultus;

        lostSoulCountAlpine.gameObject.SetActive(false);
        lostSoulCountCavern.gameObject.SetActive(false);
        lostSoulCountSepultus.gameObject.SetActive(false);
        popupUIPrompt.SetActive(false);
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
            pressEToExit.SetActive(false);
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
                        acc.ChangeCutscene(1);
                        vcc.ChangeCutscene(1);
                        SceneHandler.instance.LoadLevel("AnimaticCutscenes");
                    }
                break;
                case 1:
                    //GO TO CAVERN
                    if (levelNumber <= levelsCompleted || DebugLoadAnyLevel) {
                        acc.ChangeCutscene(3);
                        vcc.ChangeCutscene(3);
                        SceneHandler.instance.LoadLevel("AnimaticCutscenes");
                    }
                break;

                 case 2:
                    //GO TO SEPULTUS
                    if (levelNumber <= levelsCompleted || DebugLoadAnyLevel) {
                        acc.ChangeCutscene(5);
                        vcc.ChangeCutscene(5);
                        SceneHandler.instance.LoadLevel("AnimaticCutscenes");
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
            defaultButton.Select();
            interactableUI.SetActive(true);
            pressEToExit.SetActive(true);
        }
        yield break;
    }
}
