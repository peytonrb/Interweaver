using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LostSoulManager : MonoBehaviour
{
    public GameObject lostSoulUI;
    public TextMeshProUGUI lostSoulText;
    public Animator animator;

    [Header("Dialogue")]
    public GameObject textbox;
    public bool isSpeaking;
    
    private readonly HashSet<GameObject> alreadyCollidedWith = new HashSet<GameObject>();
    private GameMasterScript gameMaster;
    private int level;
    private bool gotScene;

    [Header("FOR TESTING PURPOSES ONLY - DO NOT TOUCH")]
    [HideInInspector] public GameObject[] lostSouls;
    [SerializeField] private List<GameObject> lostSoulsList = new List<GameObject>();

    [Header("Audio")]
    [SerializeField] private AudioClip lostSoulGetSound;
    private AudioSource lostSoulGetSource;
    
    void Start()
    {
        gameMaster = GameObject.FindWithTag("GM").GetComponent<GameMasterScript>();

        //Finds the lost souls in the level and adds them to the lost souls list. There should only be 3 in a level.
        lostSouls = GameObject.FindGameObjectsWithTag("Lost Soul");

        gotScene = false;

        for (int i = 0; i < lostSouls.Length; i++) {
            LostSoulController soulController = lostSouls[i].GetComponent<LostSoulController>();
            
            lostSoulsList.Insert(soulController.soulID, lostSouls[i]);
        }

        lostSoulGetSource = null;

    }

    void Update() {
        if (gotScene == false) {
            if (SceneHandler.instance.currentSceneName != null) {
                //Finds what scene the player is currently in.
                switch (SceneHandler.instance.currentSceneName) {
                    case "AlpineCombined":
                        //Tells the lost souls list which objects dont exist anymore
                        for (int i = 0; i < PlayerData.instance.GetAlpineLostSouls().Count; i++) {
                            //If this is false, then this doesn't exist anymore
                            if (PlayerData.instance.GetAlpineLostSouls()[i] == false) {
                                if (lostSoulsList[i] != null) {
                                    LostSoulController soulController = lostSoulsList[i].GetComponent<LostSoulController>();
                                    soulController.DestroyMyself();
                                }
                            }
                        }
                        level = 1;
                        gotScene = true;
                    break;
                    case "Cavern":
                        //Tells the lost souls list which objects dont exist anymore
                        for (int i = 0; i < PlayerData.instance.GetCavernLostSouls().Count; i++) {
                            //If this is false, then this doesn't exist anymore
                            if (PlayerData.instance.GetCavernLostSouls()[i] == false) {
                                if (lostSoulsList[i] != null) {
                                    LostSoulController soulController = lostSoulsList[i].GetComponent<LostSoulController>();
                                    soulController.DestroyMyself();
                                }
                            }
                        }
                        level = 2;
                        gotScene = true;
                    break;
                    case "Sepultus":
                        //Tells the lost souls list which objects dont exist anymore
                        for (int i = 0; i < PlayerData.instance.GetSepultusLostSouls().Count; i++) {
                            //If this is false, then this doesn't exist anymore
                            if (PlayerData.instance.GetSepultusLostSouls()[i] == false) {
                                if (lostSoulsList[i] != null) {
                                    LostSoulController soulController = lostSoulsList[i].GetComponent<LostSoulController>();
                                    soulController.DestroyMyself();
                                }
                            }
                        }
                        level = 3;
                        gotScene = true;
                    break;
                }
            }
        }
        
        if (isSpeaking && !textbox.activeSelf) // sloppy way of checking when dialogue is over
        {
            isSpeaking = false;
            this.GetComponent<MovementScript>().ToggleCanMove(true);
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Lost Soul" && !alreadyCollidedWith.Contains(hit.gameObject))
        {
            alreadyCollidedWith.Add(hit.gameObject);
            animator.SetBool("isOpen", true);
            lostSoulGetSource = AudioManager.instance.AddSFX(lostSoulGetSound, false, lostSoulGetSource);

            // lost soul dialogue -- add lil open animation
            isSpeaking = true;
            this.GetComponent<MovementScript>().ToggleCanMove(false);
            DialogueManager.instance.StartDialogue(hit.GetComponent<LostSoulController>().lostSoulDialogue, textbox);

            //Gamemaster's lost soul count is added.
            gameMaster.totalLostSouls++;
            lostSoulText.text = "" + gameMaster.totalLostSouls;

            switch (level) {
                case 1:
                    LostSoulController soulController = hit.gameObject.GetComponent<LostSoulController>();
                    
                    PlayerData.instance.SetAlpineLostSouls(soulController.soulID,false);
                break;
                case 2:
                    soulController = hit.gameObject.GetComponent<LostSoulController>();
                    
                    PlayerData.instance.SetCavernLostSouls(soulController.soulID,false);
                break;
                case 3:
                    soulController = hit.gameObject.GetComponent<LostSoulController>();
                    
                    PlayerData.instance.SetSepultusLostSouls(soulController.soulID,false);
                break;
            }
            
            Destroy(hit.gameObject);
            StartCoroutine(lostSoulOnScreen());
        }
    }

    // keeps lost soul UI on screen for a little bit then hides
    IEnumerator lostSoulOnScreen()
    {
        yield return new WaitForSeconds(5);
        animator.SetBool("isOpen", false);
    }
}
