using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LostSoulManager : MonoBehaviour
{
    public GameObject lostSoulUI;
    public TextMeshProUGUI lostSoulText;
    public Animator animator;
    private readonly HashSet<GameObject> alreadyCollidedWith = new HashSet<GameObject>();
    private GameMasterScript gameMaster;
    private int level;

    [Header("FOR TESTING PURPOSES ONLY - DO NOT TOUCH")]
    [SerializeField] private GameObject[] lostSouls;
    [SerializeField] private List<GameObject> lostSoulsList = new List<GameObject>();
    
    void Start()
    {
        gameMaster = GameObject.FindWithTag("GM").GetComponent<GameMasterScript>();

        //Finds the lost souls in the level and adds them to the lost souls list. There should only be 3 in a level.
        lostSouls = GameObject.FindGameObjectsWithTag("Lost Soul");

        for (int i = 0; i < lostSouls.Length; i++) {
            LostSoulController soulController = lostSouls[i].GetComponent<LostSoulController>();
            
            lostSoulsList.Insert(soulController.soulID, lostSouls[i]);
        }

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
            break;
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Lost Soul" && !alreadyCollidedWith.Contains(hit.gameObject))
        {
            alreadyCollidedWith.Add(hit.gameObject);
            animator.SetBool("isOpen", true);
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
