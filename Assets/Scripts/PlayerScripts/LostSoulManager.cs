using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LostSoulManager : MonoBehaviour
{
    public GameObject lostSoulUI;
    public TextMeshProUGUI lostSoulText;
    public Animator animator;
    // [SerializeField] private int numLostSouls;
    private readonly HashSet<GameObject> alreadyCollidedWith = new HashSet<GameObject>();
    private GameMasterScript gameMaster;

    void Start()
    {
        // numLostSouls = 0;
        gameMaster = GameObject.FindWithTag("GM").GetComponent<GameMasterScript>();
    }

    void OnTriggerEnter(Collider hit) //void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Lost Soul" && !alreadyCollidedWith.Contains(hit.gameObject))
        {
            alreadyCollidedWith.Add(hit.gameObject);
            animator.SetBool("isOpen", true);
            // numLostSouls++;
            gameMaster.totalLostSouls++;
            lostSoulText.text = "" + gameMaster.totalLostSouls;
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
