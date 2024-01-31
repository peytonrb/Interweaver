using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public static bool debugIsOn = true;
    private bool debugUIison;
    [SerializeField] private GameObject debugUI;
    private FamiliarScript familiarScript;
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
        familiarScript = familiar.GetComponent<FamiliarScript>();
        cms = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
        gms = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMasterScript>();

        totalweavercheckpoints.text = "Total Weaver Checkpoints: " + gms.weaverCheckpoints.Length.ToString();
        totalfamiliarcheckpoints.text = "Total Familiar Checkpoints: " + gms.familiarCheckpoints.Length.ToString();

        UpdateFamiliarTurn(familiarScript.myTurn);
        UpdateCameraOnPriority(cms.weaverCameraOnPriority,cms.familiarCameraOnPriority);
        UpdateCurrentCheckpoint(gms.WeaverCheckPointNum,gms.FamiliarCheckPointNum);
        UpdatePositions(weaver.transform,familiar.transform);
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
            if (debugUIison) {
                UpdateFamiliarTurn(familiarScript.myTurn);
                UpdateCameraOnPriority(cms.weaverCameraOnPriority,cms.familiarCameraOnPriority);
                UpdateCurrentCheckpoint(gms.WeaverCheckPointNum,gms.FamiliarCheckPointNum);
                UpdatePositions(weaver.transform,familiar.transform);
            }
            //Toggle Debug UI
            if (Input.GetKeyDown(KeyCode.R)) {
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
        yield return new WaitForSeconds(1f);
        UpdateFPS();
    }
}
