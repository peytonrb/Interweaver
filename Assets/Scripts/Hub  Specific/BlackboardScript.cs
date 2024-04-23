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
    [SerializeField] private TextMeshProUGUI pressEToExit;
    private AnimaticCutsceneController acc;
    private VideoCutsceneController vcc;
    public GameObject popupUIPrompt;
    [HideInInspector] [Tooltip ("If true, then you are currently staring at the blackboard, and blackboard functionality is on.")] public bool onBlackboard;
    private int levelsCompleted;
    private int lostSoulTotalAlpine;
    private int lostSoulTotalCavern;
    private int lostSoulTotalSepultus;
    public bool DebugLoadAnyLevel = true;

    [Header("VFX")]
    [SerializeField] private GameObject portalVFX;
    [SerializeField] private GameObject player;
    private GameObject activeVFX;

    [Header("Audio")]
    [SerializeField] private AudioClip portalSpawning;
    [SerializeField] private AudioClip portalSustain;
    [SerializeField] private AudioClip portalEnd;
    private AudioSource portalSustainSource;

    // Start is called before the first frame update
    void Start()
    {
        acc = GetComponent<AnimaticCutsceneController>();
        vcc = GetComponent<VideoCutsceneController>();

        interactableUI.SetActive(false);
        pressEToExit.gameObject.SetActive(false);
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

        portalSustainSource = null;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            var weaverNPCInteraction = InputManagerScript.instance.playerInput.actions["NPCInteraction"].GetBindingDisplayString();

            popupUIPrompt.SetActive(true);

            popupUIPrompt.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().SetText("<sprite name=" + weaverNPCInteraction + ">"
                         + " ...");
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            popupUIPrompt.SetActive(false);
        }
    }

    public void GoToFromBlackboard(MovementScript movementScript) {
        if (onBlackboard == false) {
            GameObject vfx = Instantiate(portalVFX, new Vector3(player.transform.position.x, player.transform.position.y - 1f, player.transform.position.z), Quaternion.identity);
            vfx.GetComponent<PortalVFXController>().isActive = true;
            activeVFX = vfx;
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, portalSpawning, 1f);
            StartCoroutine(PlayDelayedPortalSound());
            StartCoroutine(OpenBlackboard(movementScript));
            
        }
        else {
            activeVFX.GetComponent<PortalVFXController>().isWaiting = false;
            interactableUI.SetActive(false);
            pressEToExit.gameObject.SetActive(false);
            movementScript.ToggleCanLook(true);
            movementScript.ToggleCanMove(true);
            blackboardCamera.Priority = 0;
            InputManagerScript.instance.isOnBlackboard = false;
            onBlackboard = false;
            portalSustainSource = AudioManager.instance.KillAudioSource(portalSustainSource);
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, portalEnd, 1f);
            if (!InputManagerScript.instance.isGamepad)
            {
                Cursor.visible = false;
            }
        }
    }

        IEnumerator PlayDelayedPortalSound()
            {
                yield return new WaitForSeconds(2.596f); 
                portalSustainSource = AudioManager.instance.AddSFX(portalSustain, true, portalSustainSource);
            }

    IEnumerator OpenBlackboard(MovementScript movementScript)
    {
        yield return new WaitForSeconds(0.75f);
        movementScript.ToggleCanLook(false);
        movementScript.ToggleCanMove(false);
        if (!InputManagerScript.instance.isGamepad)
        {
            Cursor.visible = true;
        }
        blackboardCamera.Priority = 2;
        StartCoroutine(WaitForBlendToFinish());
        if (popupUIPrompt.activeSelf) {
            popupUIPrompt.SetActive(false);
        }
        InputManagerScript.instance.isOnBlackboard = true;
        onBlackboard = true;
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
                        activeVFX.GetComponent<PortalVFXController>().isWaiting = false;
                        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, portalEnd, 1f);
                        portalSustainSource = AudioManager.instance.KillAudioSource(portalSustainSource);
                        blackboardCamera.Priority = 0;
                        StartCoroutine(CutsceneLoader(1));
                    }
                break;
                case 1:
                    //GO TO CAVERN
                    if (levelNumber <= levelsCompleted || DebugLoadAnyLevel) {
                        activeVFX.GetComponent<PortalVFXController>().isWaiting = false;
                        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, portalEnd, 1f);
                        portalSustainSource = AudioManager.instance.KillAudioSource(portalSustainSource);
                        blackboardCamera.Priority = 0;
                        StartCoroutine(CutsceneLoader(3));
                    }
                break;

                 case 2:
                    //GO TO SEPULTUS
                    if (levelNumber <= levelsCompleted || DebugLoadAnyLevel) {
                        activeVFX.GetComponent<PortalVFXController>().isWaiting = false;
                        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, portalEnd, 1f);
                        portalSustainSource = AudioManager.instance.KillAudioSource(portalSustainSource);
                        blackboardCamera.Priority = 0;
                        StartCoroutine(CutsceneLoader(5));
                    }
                break;
            }
        }
        
    }

    IEnumerator CutsceneLoader(int index)
    {
        yield return new WaitForSeconds(0.75f);
        player.GetComponent<DarknessMechanicScript>().enabled = true;
        yield return new WaitForSeconds(1.25f);
        acc.ChangeCutscene(index);
        vcc.ChangeCutscene(index);
        SceneHandler.instance.LoadLevel("AnimaticCutscenes");
    }

    IEnumerator WaitForBlendToFinish() {
        yield return null;
        while (mainCamera.IsBlending) {
            yield return null;
        }
        if (onBlackboard) {
            defaultButton.Select();
            interactableUI.SetActive(true);
            pressEToExit.gameObject.SetActive(true);
            //Changes the text on the exit prompt depending on the control scheme
            PlayerInput playerInput = InputManagerScript.instance.GetComponent<PlayerInput>();
            if (playerInput.currentControlScheme == "Gamepad") {
                pressEToExit.text = "Press B to Exit";
            }
            else {
                pressEToExit.text = "Press E to Exit";
            }
        }
        yield break;
    }
}
