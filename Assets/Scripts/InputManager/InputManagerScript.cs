using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using TMPro;
using UnityEngine.UI;


public class InputManagerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject familiar;
    public bool canSwitch = true; // bool which determines if possession can occur
    public Vector2 movement;
    public Vector2 weaveCursor;
    public bool switching;
    public GameObject pauseScreen;
    private PauseScript pauseScript;
    public static InputManagerScript instance;
    private bool hasFamiliarInvoke;
    private bool hasFamiliarInvoke2;

    public bool isGamepad = false;
    private PlayerControllerNew playerScript;
    private WeaveController weaveController;
    private FamiliarScript familiarScript;
    private MovementScript movementScript;
    private MoleDigScript moleDigScript;
    public PlayerInput playerInput;


    [SerializeField] private Image popUIForFamiliar;
    [SerializeField] private Image otherPopUIForFamiliar;
    private bool isMole, isOwl, isStag;
    public enum myEnums
    {
        Owl,
        Mole,
        Stag
    }
    public myEnums familiarEnums;


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
        familiarScript = familiar.GetComponent<FamiliarScript>();
        pauseScript = pauseScreen.GetComponent<PauseScript>();
        moleDigScript = familiar.GetComponent<MoleDigScript>();
        playerInput = GetComponent<PlayerInput>();

        //usingController = pauseScript.GetUsingController(); //Checks if using the controller
        // Debug.Log(playerInput.currentControlScheme);
        

        switch (familiarEnums)
        {
            case myEnums.Owl:
                isMole = false;
                isStag = false;
                isOwl = true;
                break;
            case myEnums.Mole:
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
        #region//MolePopUI
        //*************************************************************************
        //if (moleDigScript != null && (moleDigScript.isOnDigableLayer) && !hasFamiliarInvoke)
        //{
        //    //this is where I would put the ui being active and showing the button for digging
        //    popUIForFamiliar.gameObject.SetActive(true);
        //    popUIForFamiliar.gameObject.transform.GetComponentInChildren<TMP_Text>().
        //        SetText(playerInput.actions["MoleFamiliarInteract"].GetBindingDisplayString());

        //    hasFamiliarInvoke = true;
           
        //}

        //else if (moleDigScript != null && (!moleDigScript.isOnDigableLayer) && hasFamiliarInvoke)
        //{
        //    //this is where I would probably have it turned off when it leaves the layer
        //    popUIForFamiliar.gameObject.SetActive(false);
        //    hasFamiliarInvoke = false;
        //}

        //if ((moleDigScript != null && (moleDigScript.startedToDig) && !hasFamiliarInvoke2))
        //{
        //    Debug.Log("this should turn on");
        //    //for the familiar dig and wants to make a pillar
        //    otherPopUIForFamiliar.gameObject.SetActive(true);
        //    otherPopUIForFamiliar.gameObject.transform.GetComponentInChildren<TMP_Text>().
        //    SetText(playerInput.actions["MoleAltFamiliarInteract"].GetBindingDisplayString());
        //    hasFamiliarInvoke2 = true;
        //}

        //else if ((moleDigScript != null && (!moleDigScript.startedToDig) && hasFamiliarInvoke2))
        //{
        //    //this is where I would probably have it turned off when it undigs
        //    otherPopUIForFamiliar.gameObject.SetActive(false);
        //    hasFamiliarInvoke2 = false;
        //}
        //*************************************************************************
        #endregion
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
            WeaveableManager.Instance.DestroyJoints(weaveController.currentWeaveable.listIndex);
            weaveController.OnDrop();
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
            playerInput.SwitchCurrentActionMap("Familiar");
        }
    }

    public void OnPossessWeaver(InputValue input)
    {
        if ((input.isPressed) && (!familiarScript.isDead))
        {
            PossessWeaver();
        }
    }

    public void PossessWeaver()
    {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        CharacterController familiarCharacterController = familiar.GetComponent<CharacterController>();
        if (familiarScript.myTurn && familiarCharacterController.isGrounded && canSwitch && !familiarScript.talkingToNPC)
        {
            familiarScript.Depossess();
            playerInput.SwitchCurrentActionMap("Weaver");
        }
    }

    #endregion//******************************************************

    //Both Characters
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
                Debug.Log("Interacting Familiar");
            }
            else
            {
                NPCInteractionScript npcInteractScript = player.GetComponent<NPCInteractionScript>();
                npcInteractScript.Interact();
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
