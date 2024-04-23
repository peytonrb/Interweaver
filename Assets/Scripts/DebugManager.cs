using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public static bool debugIsOn = false;
    private bool debugUIison;
    [SerializeField] private GameObject debugUI;
    private FamiliarScript familiarScript;
    private GameObject cameraMaster;
    private GameObject gameMaster;
    private CameraMasterScript cms;
    private GameMasterScript gms;
    public TextMeshProUGUI whosturn;
    public TextMeshProUGUI weavercamera;
    public TextMeshProUGUI familiarcamera;
    public TextMeshProUGUI weavercheckpoint;
    public TextMeshProUGUI familiarcheckpoint;
    public TextMeshProUGUI weaverposition;
    public TextMeshProUGUI familiarposition;
    public TextMeshProUGUI totalweavercheckpoints;
    public TextMeshProUGUI totalfamiliarcheckpoints;
    private GameObject weaver;
    private GameObject familiar;
    private int framerate;
    public TextMeshProUGUI fps;
    private bool familiarfound;
    private bool gamemasterfound;
    private bool cameramasterfound;
    private bool allfound;


    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        weaver = GameObject.FindGameObjectWithTag("Player");
        familiar = GameObject.FindGameObjectWithTag("Familiar");
        gameMaster = GameObject.FindGameObjectWithTag("GM");
        cameraMaster = GameObject.FindGameObjectWithTag("CameraMaster");
        if (familiar != null) {
            familiarScript = familiar.GetComponent<FamiliarScript>();
            familiarfound = true;
        }
        if (gameMaster != null) {
            gms = gameMaster.GetComponent<GameMasterScript>();
            gamemasterfound = true;
        }
        if (cameraMaster != null) {
            cms = cameraMaster.GetComponent<CameraMasterScript>();
            cameramasterfound = true;
        }

        if (familiarfound && gamemasterfound && cameramasterfound) {
            totalweavercheckpoints.text = "Total Weaver Checkpoints: " + gms.weaverCheckpoints.Length.ToString();
            totalfamiliarcheckpoints.text = "Total Familiar Checkpoints: " + gms.familiarCheckpoints.Length.ToString();

            UpdateFamiliarTurn(familiarScript.myTurn);
            UpdateCameraOnPriority(cms.weaverCameraOnPriority,cms.familiarCameraOnPriority);
            UpdateCurrentCheckpoint(gms.WeaverCheckPointNum,gms.FamiliarCheckPointNum);
            UpdatePositions(weaver.transform,familiar.transform);
            allfound = true;
        }
        
        StartCoroutine(ForcedDelay());

        if (debugUIison) {
            debugUI.SetActive(true);
        }
        else {
            debugUI.SetActive(false);
        }

    }

    public bool GetDebugOn() {
        return debugIsOn;
    }

    void Update() {
        if (debugIsOn) {
            if (debugUIison && allfound) {
                UpdateFamiliarTurn(familiarScript.myTurn);
                UpdateCameraOnPriority(cms.weaverCameraOnPriority,cms.familiarCameraOnPriority);
                UpdateCurrentCheckpoint(gms.WeaverCheckPointNum,gms.FamiliarCheckPointNum);
                UpdatePositions(weaver.transform,familiar.transform);

                if (Input.GetKeyDown(KeyCode.Z)) {
                    PlayerData.instance.SaveGame();
                }
                if (Input.GetKeyDown(KeyCode.X)) {
                    PlayerData.instance.LoadGame();
                }
                if (Input.GetKeyDown(KeyCode.C)) {
                    PlayerData.instance.NewGame();
                }
            }
            //Toggle Debug UI
            if (Input.GetKeyDown(KeyCode.BackQuote)) {
                if (debugUIison) {
                    debugUI.SetActive(false);
                    StopAllCoroutines();
                    debugUIison = false;
                }
                else {
                    debugUI.SetActive(true);
                    StartCoroutine(ForcedDelay());
                    debugUIison = true;
                }
            }
        } 
    }

    void UpdateFamiliarTurn(bool turn) {
        if (turn == false) {
            whosturn.text = "Current Character: Weaver";
        }
        else {
            whosturn.text = "Current Character: Familiar";
        }
    }

    void UpdateCameraOnPriority(int weaverscamera, int familiarscamera) {
        weavercamera.text = "Current Weaver Camera: " + weaverscamera;
        familiarcamera.text = "Current Familiar Camera: " + familiarscamera;
    }

    void UpdateCurrentCheckpoint(int weaverscheckpoint, int familiarscheckpoint) {
        weavercheckpoint.text = "Current Weaver Checkpoint: " + weaverscheckpoint;
        familiarcheckpoint.text = "Current Familiar Checkpoint: " + familiarscheckpoint;
    }

    void UpdatePositions(Transform weavertransform, Transform familiartransform) {
        weaverposition.text = "Weaver Position: " + weavertransform.position.ToString();
        familiarposition.text = "Familiar Position: " + familiartransform.position.ToString();
    }

    void UpdateFPS() {
        framerate = (int)(1f / Time.unscaledDeltaTime);
        fps.text = "FPS: " + framerate;
        StartCoroutine(ForcedDelay());
    }

    IEnumerator ForcedDelay() {
        yield return new WaitForSeconds(1.5f);
        UpdateFPS();
    }
}
