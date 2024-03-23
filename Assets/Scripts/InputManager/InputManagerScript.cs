using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class InputManagerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject familiar;
    public GameObject wyvern;
    public bool canSwitch = true; // bool which determines if possession can occur
    public Vector2 movement;
    public Vector2 weaveCursor;
    public bool switching;
    public GameObject pauseScreen;
    private PauseScript pauseScript;
    public static InputManagerScript instance;

    //the invoke bools are there so then it can only happen once instead of every frame in the update function
    //*****************************************
    private bool hasFamiliarInvoke; 
    private bool hasFamiliarInvoke2;
    private bool hasWeaverInvoke;
    //*****************************************

    public bool isGamepad = false;
    private PlayerControllerNew playerScript;
    private WeaveController weaveController;
    private FamiliarScript familiarScript;
    private MovementScript movementScript;
    private MoleDigScript moleDigScript;
    private OwlDiveScript owlDiveScript;
    private WyvernBossManager wyvernScript;
    public PlayerInput playerInput;
    private MovementScript familiarMovement;

    [CannotBeNullObjectField] public PossessionUIAnimationHandler weaverAnimationUIPosessionHandler;

    [CannotBeNullObjectField] public PossessionUIAnimationHandler familiarAnimationUIPosessionHandler;


    [SerializeField] private GameObject popUiFamiliarCanvas;

    [SerializeField] private GameObject popUiWeaverCanvas;
    private bool isMole, isOwl, isStag;
    public enum myEnums
    {
        Owl,
        Mole,
        Stag
    }
    public myEnums familiarEnums;
    private string currentSceneName;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        playerScript = player.GetComponent<PlayerControllerNew>();
        weaveController = player.GetComponent<WeaveController>();
        movementScript = player.GetComponent<MovementScript>();
        familiarMovement = familiar.GetComponent<MovementScript>();
        familiarScript = familiar.GetComponent<FamiliarScript>();
        pauseScript = pauseScreen.GetComponent<PauseScript>();               
        playerInput = GetComponent<PlayerInput>();
        currentSceneName = SceneManager.GetActiveScene().name;
        if (wyvern != null) 
        {
            wyvernScript = wyvern.GetComponent<WyvernBossManager>();
        }
        

       
        switch (familiarEnums)
        {
            case myEnums.Owl:
                isMole = false;
                isStag = false;
                isOwl = true;
                break;
            case myEnums.Mole:
                moleDigScript = familiar.GetComponent<MoleDigScript>();
                isMole = true;
                isStag = false;
                isOwl = false;
                break;
            case myEnums.Stag:
                isMole = false;
                isStag = true;
                isOwl = false;
                break;
        }
    }

    private void Update()
    {
        FamiliarUI();
        WeaverUI();
      
    }

    public void ToggleControlScheme(bool isController)
    {
        if (isController)
        {
            isGamepad = true;
            playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);

        }
        else
        {
            isGamepad = false;
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
    }

    #region//WEAVER ABILITIES
    //******************************************************

    //MOVE
    //******************************************************
    public void OnWeaverMove(InputValue input)
    {
        movement = input.Get<Vector2>();
        MovementScript movementScript = player.GetComponent<MovementScript>();
        movementScript.LookAndMove();
    }

    // JUMP
    //******************************************************
    public void OnJump(InputValue input)
    {
        JumpAndDashScript jumpAndDashScript = player.GetComponent<JumpAndDashScript>();
        jumpAndDashScript.DoJump();
    }

    // DASH
    //******************************************************
    public void OnDash(InputValue input)
    {
        JumpAndDashScript jumpAndDashScript = player.GetComponent<JumpAndDashScript>();
        jumpAndDashScript.DoDash();
    }
    //******************************************************
    public void OnWeaverInteract(InputValue input)
    {
        //pops up the ui if the bollean is true but for now not for the weave
       
        if (input.isPressed)
        {
            if (weaveController.isWeaving)
            {
                weaveController.CheckIfWeaveable(isGamepad);
            }
            else
            {
                weaveController.WeaveObject(isGamepad);
            }
        }
    }

    public void OnDrop(InputValue input)
    {
        if (input.isPressed)
        {
            weaveController.OnDrop();
        }
    }

    public void OnWeaverTargeting(InputValue input)
    {
        Vector2 inputVector = input.Get<Vector2>();

        if (isGamepad)
        {
            if (weaveController.isWeaving)
            {
                weaveController.currentWeaveable.MoveWeaveableToTarget(inputVector);
            }
            else
            {
                weaveController.GamepadTargetingArrow(inputVector);
            }
        }
        else
        {
            if (weaveController.isWeaving)
            {
                weaveController.currentWeaveable.MoveWeaveableToMouse();
            }
            else
            {
                weaveController.MouseTargetingArrow(inputVector);
            }
        }
    }

    public void OnWeaverNPCInteractions(InputValue input)
    {

    }

    public void OnRotate(InputValue input)
    {
        Vector2 dir = input.Get<Vector2>();

        if (weaveController.currentWeaveable != null && weaveController.currentWeaveable.isBeingWoven)
        {
            switch (dir)
            {
                case Vector2 v when v.Equals(Vector2.up):
                    {
                        weaveController.currentWeaveable.RotateObject(WeaveableObject.rotateDir.forward);
                        break;
                    }
                case Vector2 v when v.Equals(Vector2.down):
                    {
                        weaveController.currentWeaveable.RotateObject(WeaveableObject.rotateDir.back);
                        break;
                    }
                case Vector2 v when v.Equals(Vector2.right):
                    {
                        weaveController.currentWeaveable.RotateObject(WeaveableObject.rotateDir.right);
                        break;
                    }
                case Vector2 v when v.Equals(Vector2.left):
                    {
                        weaveController.currentWeaveable.RotateObject(WeaveableObject.rotateDir.left);
                        break;
                    }
            }
        }
    }

    public void OnUncombineAction(InputValue input)
    {
        if (input.isPressed)
        {
            //Disable this when in Hub
            WeaveableManager.Instance.DestroyJoints(weaveController.currentWeaveable.listIndex);
            weaveController.OnDrop();
        }
    }

    public void OnWyvernCameraToggle(InputValue input)
    {
        if (input.isPressed)
        {
            WyvernLookAt wyvernLookAtScript = player.transform.Find("WyvernCamera").GetComponent<WyvernLookAt>();
            GameObject wyvernCamera = wyvernLookAtScript.transform.GetChild(0).gameObject;
            
            if (!wyvernLookAtScript.cameraIsActive)
            {
                wyvernCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 2;
                wyvernLookAtScript.cameraIsActive = true;
            }
            else
            {
                wyvernCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 1;
                wyvernLookAtScript.cameraIsActive = false;
            }
        }
    }

    #endregion//******************************************************

    #region//SWITCHING
    //******************************************************
    public void OnPossessFamiliar(InputValue input)
    {
        

        //When we add the hub scene, we will put this line back in. This error was likely caused by the scene of that name not being part of the build settings.
        //&& SceneHandler.instance.currentSceneName != "Hub" has been causing issues in this if statement
        if (input.isPressed && !playerScript.isDead)
        {
            PossessFamiliar();
           
            //if (currentSceneName == "AlpineCombined" || currentSceneName == "Cavern") {
            //    PossessFamiliar();
            //}
            
        }
    }

    public void PossessFamiliar()
    {
        //Ability to possess familiar inside hub is disabled since there are no familiars in the hub.
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        CharacterController playerCharacterController = player.GetComponent<CharacterController>();

        if (!familiarScript.myTurn && !weaveController.isWeaving && playerCharacterController.isGrounded && !playerScript.inCutscene && canSwitch && !playerScript.talkingToNPC)
        {
            playerScript.Possession();
            weaverAnimationUIPosessionHandler.SwitchingToFamiliar();
            familiarAnimationUIPosessionHandler.SwitchingToFamiliar();
            //Familiar turn is false
            if (wyvernScript != null) 
            {
                if (wyvern.activeSelf) {
                    wyvernScript.WeaverOrFamiliar(familiarScript.myTurn);
                }
            }
            playerInput.SwitchCurrentActionMap("Familiar");
        }
    }

    public void OnPossessWeaver(InputValue input)
    {
        if ((input.isPressed) && (!familiarScript.isDead))
        {
            PossessWeaver();
            
            //if (currentSceneName == "AlpineCombined" || currentSceneName == "Cavern") {
            //    PossessWeaver();
            //}

        }
    }

    public void PossessWeaver()
    {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        CharacterController familiarCharacterController = familiar.GetComponent<CharacterController>();
        if (familiarScript.myTurn && familiarCharacterController.isGrounded && canSwitch && !familiarScript.talkingToNPC)
        {
            familiarScript.Depossess();
            weaverAnimationUIPosessionHandler.SwitchingToWeaver();
            familiarAnimationUIPosessionHandler.SwitchingToWeaver();
            //Familiar turn is true
            if (wyvernScript != null) 
            {
                if (wyvern.activeSelf) {
                    wyvernScript.WeaverOrFamiliar(familiarScript.myTurn);
                }
            }
            playerInput.SwitchCurrentActionMap("Weaver");
        }
    }

    #endregion//******************************************************

    #region//Both Characters
    //******************************************************
    public void OnPause(InputValue input)
    {
        if (input.isPressed)
        {

            if (!pauseScreen.activeSelf)
            {
                pauseScript = pauseScreen.GetComponent<PauseScript>();
                pauseScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                pauseScript.Resume();
            }

        }
    }

    /*
    public void OnQuit(InputValue input)
    {
        if (input.isPressed)
        {
            Application.Quit();
        }
    }
    */

    public void ResetCurrentCharacter()
    {
        if (familiarScript.myTurn)
        {
            familiarScript.GetComponent<MovementScript>().GoToCheckPoint();
        }
        else
        {
            playerScript.GetComponent<MovementScript>().GoToCheckPoint();
        }

        pauseScript.Resume();
    }

    public void OnNPCInteraction(InputValue input)
    {
        if (input.isPressed)
        {
            if (familiarScript.myTurn)
            {
                NPCInteractionScript npcInteractScript = familiar.GetComponent<NPCInteractionScript>();
                npcInteractScript.Interact();
                DialogueManager.instance.DisplayNextSentence();
                Debug.Log("Interacting Familiar");
            }
            else
            {
                NPCInteractionScript npcInteractScript = player.GetComponent<NPCInteractionScript>();
                npcInteractScript.Interact();
                DialogueManager.instance.DisplayNextSentence();
                Debug.Log("Interacting Weaver");
            }
        }

    }

    public void ControllerRumble(float duration, float leftMotorFreq = 0, float rightMotorFreq = 0)
    {
        if (isGamepad)
        {
            StartCoroutine(RumbleTheController(duration, leftMotorFreq, rightMotorFreq));
        }
    }

    IEnumerator RumbleTheController(float dur, float leftMotorFreq, float rightMotorFreq)
    {
        Gamepad.current.SetMotorSpeeds(leftMotorFreq, rightMotorFreq);

        yield return new WaitForSeconds(dur);

        Gamepad.current.SetMotorSpeeds(0, 0);

        yield break;
    }

    //******************************************************
    #endregion

    private void WeaverUI()
    {
        var weaverTargetingName = playerInput.actions["WeaverTargeting"].GetBindingDisplayString();
        var weaverRotatingName = playerInput.actions["Rotate"].GetBindingDisplayString();
        if ((weaveController != null) && (weaveController.isWeaving) && !hasWeaverInvoke && movementScript.isInTutorial)
        {
            popUiWeaverCanvas.gameObject.SetActive(true);

            popUiWeaverCanvas.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().
                SetText("<sprite name="+weaverTargetingName+">" + " to move weave" +
                "<br><sprite name=" + weaverRotatingName + ">" + " to rotate weave");          

            hasWeaverInvoke = true;
        }

        else if ((!weaveController.isWeaving) && hasWeaverInvoke)
        {
            popUiWeaverCanvas.gameObject.SetActive(false);
            hasWeaverInvoke = false;
        }
    }
    private void FamiliarUI()
    {
        switch (familiarEnums)
        {
            case myEnums.Owl:
                #region//OwlPopUI
                //*************************************************************************
                if ((!familiarMovement.isNearGround) && (!hasFamiliarInvoke) && (familiarMovement.active) 
                    && popUiFamiliarCanvas != null && familiarMovement.isInTutorial)
                {
                    var inputName = playerInput.actions["FamiliarInteract"].GetBindingDisplayString();
                    //this is where I would put the ui being active and showing the button for digging
                    popUiFamiliarCanvas.gameObject.SetActive(true);
                    popUiFamiliarCanvas.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().
                        SetText("<sprite name=" + inputName + ">" + " to dive");
                    //playerInput.actions["FamiliarInteract"].GetBindingDisplayString()
                    hasFamiliarInvoke = true;
                }

                else if ((familiarMovement.isNearGround) && (hasFamiliarInvoke) || (!familiarMovement.active))
                {
                    if (popUiFamiliarCanvas != null)
                        popUiFamiliarCanvas.gameObject.SetActive(false);

                    hasFamiliarInvoke = false;
                }
                #endregion
                break;
            case myEnums.Mole:
                #region//MolePopUI
                //*************************************************************************
                var digInputName = playerInput.actions["MoleFamiliarInteract"].GetBindingDisplayString();
                var pillarInputName = playerInput.actions["MoleAltFamiliarInteract"].GetBindingDisplayString();
                var lowerPillarInputName = playerInput.actions["MoleAltAltFamiliarInteract"].GetBindingDisplayString();
                if (moleDigScript != null && (moleDigScript.isOnDigableLayer) && !hasFamiliarInvoke && familiarMovement.isInTutorial && familiarMovement.active)
                {
                    //this is where I would put the ui being active and showing the button for digging
                    popUiFamiliarCanvas.gameObject.SetActive(true);
                    popUiFamiliarCanvas.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().
                        SetText("<sprite name=" + digInputName + ">" + " to dig");

                    hasFamiliarInvoke = true;                   

                }

                else if (moleDigScript != null && (!moleDigScript.isOnDigableLayer) && hasFamiliarInvoke || !familiarMovement.active)
                {
                    //this is where I would probably have it turned off when it leaves the layer
                    popUiFamiliarCanvas.gameObject.SetActive(false);
                    hasFamiliarInvoke = false;
                }

                if ((moleDigScript.startedToDig) && !hasFamiliarInvoke2 && familiarMovement.isInTutorial && familiarMovement.active)
                {
                    popUiFamiliarCanvas.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().
                    SetText("<sprite name=" + pillarInputName + ">" + " to make pillar " +
                 "/  <sprite name=" + lowerPillarInputName + ">" + " to lower pillar" +
                        "<br><sprite name=" + digInputName + ">" + " to dig");
                    hasFamiliarInvoke2 = true;
                }

                else if ((!moleDigScript.startedToDig) && hasFamiliarInvoke2)
                {
                    popUiFamiliarCanvas.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().
                    SetText("<sprite name=" + digInputName + ">" + " to dig");
                    hasFamiliarInvoke2 = false;
                }

                //*************************************************************************
                #endregion

                break;
            case myEnums.Stag:
                #region //StagPopUI
                //*************************************
                var jumpInputName = playerInput.actions["StagFamiliarInteract"].GetBindingDisplayString();
                if (familiarMovement.isInTutorial)
                {
                    popUiFamiliarCanvas.gameObject.SetActive(true);
                    popUiFamiliarCanvas.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().
                        SetText("<sprite name=" + jumpInputName + ">" + " press and hold to jump");

                   
                }

                else if (!familiarMovement.isInTutorial)
                {
                    popUiFamiliarCanvas.gameObject.SetActive(false);
                    hasFamiliarInvoke = false;
                }
                //*************************************
                #endregion
                break;
        }
       
    }


    #region //FAMILIAR ABILITIES
    //These functions will never be called in hub since the action map will not change to Familiar.
    //******************************************************
    public void OnFamiliarMove(InputValue input)
    {
        movement = input.Get<Vector2>();
        MovementScript movementScript = familiar.GetComponent<MovementScript>();
        movementScript.LookAndMove();
        //Note: There wont need to be a check if its the familiar's turn as the weaver and familiar are on seperate input action maps.
    }

    public void OnFamiliarInteract(InputValue input)
    {
        bool isPressed = input.isPressed;

        switch (familiarEnums)
        {
            case myEnums.Owl: // owl jazz here
                OwlDiveScript owlDiveScript = familiar.GetComponent<OwlDiveScript>();

                if (isPressed)
                {
                    familiarScript.familiarMovementAbility = true;
                    owlDiveScript.DivePressed();
                }
                else
                {
                    familiarScript.familiarMovementAbility = false;
                    owlDiveScript.DiveRelease();
                }
                break;
            case myEnums.Mole:
                break;
            case myEnums.Stag:
                break;
        }
    }
    #endregion//******************************************************

    #region //MOLE FAMILIAR ABILITIES
    //this is hot garbo because the button for the owl dive ability and the mole ability are the same (not sure if they're supposed) but it works if you uncomment
    //******************************************************

    public void OnMoleJump(InputValue input)
    {
        switch (familiarEnums)
        {
            case myEnums.Owl: // owl jazz here
                break;
            case myEnums.Mole:
                MoleDigScript moleDigScript = familiar.GetComponent<MoleDigScript>();
                JumpAndDashScript jumpAndDashScript = familiar.GetComponent<JumpAndDashScript>();
                if (!moleDigScript.borrowed)
                {
                    jumpAndDashScript.DoJump();
                }
                break;
            case myEnums.Stag:
                break;
        }
    }

    public void OnMoleFamiliarInteract(InputValue input) // burrowing and surfacing
    {
        bool isPressed = input.isPressed;

        switch (familiarEnums)
        {
            case myEnums.Owl: // owl jazz here
                break;
            case myEnums.Mole:
                MoleDigScript moleDigScript = familiar.GetComponent<MoleDigScript>();

                if (isPressed)
                {
                    familiarScript.familiarMovementAbility = true;
                    moleDigScript.DigPressed();
                }
                else
                {
                    familiarScript.familiarMovementAbility = false;
                }
                break;
            case myEnums.Stag:
                break;
        }
    }

    public void OnMoleAltFamiliarInteract(InputValue input) // building pillars
    {
        bool isPressed = input.isPressed;

        switch (familiarEnums)
        {
            case myEnums.Owl:
                break;
            case myEnums.Mole:
                MolePillarScript molePillarScript = familiar.GetComponent<MolePillarScript>();
                MoleDigScript moleDigScript = familiar.GetComponent<MoleDigScript>();

                if (isPressed && moleDigScript.borrowed && Time.timeScale != 0)
                {
                    molePillarScript.riseInputPressed = true;
                    molePillarScript.lowerInputPressed = false;
                    molePillarScript.DeployPillar();
                    molePillarScript.rise = true;
                    molePillarScript.lower = false;
                }
                else
                {
                    molePillarScript.riseInputPressed = false;
                }
                break;
            case myEnums.Stag:
                break;
        }
    }

    public void OnMoleAltAltFamiliarInteract(InputValue input)
    {
        bool isPressed = input.isPressed;

        switch (familiarEnums)
        {
            case myEnums.Owl:
                break;
            case myEnums.Mole:
                MolePillarScript molePillarScript = familiar.GetComponent<MolePillarScript>();
                MoleDigScript moleDigScript = familiar.GetComponent<MoleDigScript>();

                if (isPressed && moleDigScript.borrowed && Time.timeScale != 0)
                {
                    molePillarScript.riseInputPressed = false;
                    molePillarScript.lowerInputPressed = true;
                    molePillarScript.rise = false;
                    molePillarScript.lower = true;
                }
                else
                {
                    molePillarScript.lowerInputPressed = false;
                }
                break;
            case myEnums.Stag:
                break;
        }
    }
    #endregion//******************************************************

    #region //STAG FAMILIAR ABILITIES
    //******************************************************

    public void OnStagFamiliarInteract(InputValue input)
    {
        bool isPressed = input.isPressed;
        switch (familiarEnums)
        {
            case myEnums.Owl:
                break;
            case myEnums.Mole:
                break;
            case myEnums.Stag:
                StagLeapScript stagLeapScript = familiar.GetComponent<StagLeapScript>();
                if (isPressed && Time.timeScale != 0)
                {
                    StartCoroutine(stagLeapScript.ChargeJump());
                }
                else
                {
                    stagLeapScript.EndCharging();
                }
                break;
        }
    }

    public void OnStagAltFamiliarInteract(InputValue input)
    {
        bool isPressed = input.isPressed;
        switch (familiarEnums)
        {
            case myEnums.Owl:
                break;
            case myEnums.Mole:
                break;
            case myEnums.Stag:
                StagSwapScript stagSwapScript = familiar.GetComponent<StagSwapScript>();
                if (isPressed && Time.timeScale != 0)
                {
                    StartCoroutine(stagSwapScript.ChargeSwap());
                }
                else
                {
                    stagSwapScript.DoSwap();
                }
                break;
        }
    }
    #endregion//******************************************************
}
