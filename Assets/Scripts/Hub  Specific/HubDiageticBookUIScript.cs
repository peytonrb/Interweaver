using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HubDiageticBookUIScript : MonoBehaviour
{
    [Header("References")]
    private CinemachineBrain mainCamera;
    [SerializeField] private CinemachineVirtualCamera bookCamera;
    [SerializeField] private GameObject interactableUI;
    [SerializeField] private List<GameObject> pageList = new List<GameObject>();

    ////sets of buttons that are going to be active in the different pages
    ////*******************************************************
    //[SerializeField] private GameObject pageTurnButtonCanvasSound;
    //[SerializeField] private GameObject pageTurnButtonCanvasVideo;
    //[SerializeField] private GameObject pageTurnButtonCanvasGame;
    ////*******************************************************

    //bools for the different pages that are going to be active
    //*******************************************************
    private bool soundPageInvoke;
    private bool videoPageInvoke;
    private bool gamePageInvoke;
    //*******************************************************

    //the default buttons that are going to be selected for the pages
    //*******************************************************
    [SerializeField] private Button defaultButtonforSound;
    [SerializeField] private Button defaultButtonforVideo;
    [SerializeField] private Button defaultButtonforGame;
    //*******************************************************
    private MovementScript characterReadingBook;
    [Header("Variables")]
    private int currentPageNumber = 0;
    [SerializeField] private bool inBook;
    EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>();
        CloseAllPages();
        interactableUI.SetActive(false);
        soundPageInvoke = false;
        videoPageInvoke = false;
        gamePageInvoke = false;
    }

    private void Update()
    {
        if (0 < pageList.Count && pageList[0].activeInHierarchy && !soundPageInvoke) 
        {
            defaultButtonforSound.Select();
            soundPageInvoke = true;
            videoPageInvoke = false;
            gamePageInvoke = false;
        }

        if (1 < pageList.Count && pageList[1].activeInHierarchy && !videoPageInvoke)
        {
            defaultButtonforVideo.Select();
            soundPageInvoke = false;
            videoPageInvoke = true;
            gamePageInvoke = false;
        }
        if (2 < pageList.Count && pageList[2].activeInHierarchy && !gamePageInvoke)
        {
            defaultButtonforGame.Select();
            soundPageInvoke = false;
            videoPageInvoke = false;
            gamePageInvoke = true;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            interactableUI.SetActive(true);
            var weaverNPCInteraction = InputManagerScript.instance.playerInput.actions["NPCInteraction"].GetBindingDisplayString();
            interactableUI.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().SetText("<sprite name=" + weaverNPCInteraction + ">"
                         + " ...");
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            interactableUI.SetActive(false);
        }
    }

    public void GoToFromBook(MovementScript movementScript) 
    {
        if (inBook == false)
        {
            movementScript.ToggleCanLook(false);
            movementScript.ToggleCanMove(false);
            characterReadingBook = movementScript;
            bookCamera.Priority = 2;
            StartCoroutine(WaitForBlendToFinish());
            if (interactableUI.activeSelf) 
            {
                interactableUI.SetActive(false);
            }
            inBook = true;
        }
        else 
        {
            interactableUI.SetActive(false);
            movementScript.ToggleCanLook(true);
            movementScript.ToggleCanMove(true);
            CloseAllPages();
            characterReadingBook = null;
            bookCamera.Priority = 0;
            inBook = false;
            if (!InputManagerScript.instance.isGamepad)
            {
                Cursor.visible = false;
            }
        }
    }

    public void TurnPageLeft()
    {
        if (currentPageNumber <= 0)
        {
            GoToFromBook(characterReadingBook);
        }
        else
        {
            pageList[currentPageNumber].SetActive(false);
            currentPageNumber --;
            pageList[currentPageNumber].SetActive(true);
        }
    }

    public void TurnPageRight()
    {
        if (currentPageNumber + 1 >= pageList.Count)
        {
            Debug.Log("Too far!");
        }
        else
        {
            pageList[currentPageNumber].SetActive(false);
            currentPageNumber ++;
            pageList[currentPageNumber].SetActive(true);
        }
    }

    private void CloseAllPages()
    {
        foreach (GameObject page in pageList)
        {
            page.SetActive(false);
        }
        
        currentPageNumber = 0;
    }

    public void TurnToSpecificPage(int pageNumber)
    {
        pageList[currentPageNumber].SetActive(false);
        currentPageNumber = pageNumber;
        pageList[currentPageNumber].SetActive(true);
    }

    IEnumerator WaitForBlendToFinish() 
    {
        yield return null;
        while (mainCamera.IsBlending) 
        {
            yield return null;
        }
        if (inBook) 
        {
            pageList[0].SetActive(true);
            
            currentPageNumber = 0;
            interactableUI.SetActive(true);
            if (!InputManagerScript.instance.isGamepad)
            {
                Cursor.visible = true;
            }
        }
        yield break;
    }
    
}