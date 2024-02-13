using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


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

    public bool isGamepad = false;
    private PlayerController playerScript;
    private FamiliarScript familiarScript;
    private MovementScript movementScript;
    public PlayerInput playerInput;

    private bool isMole, isOwl, isStag;
    public enum myEnums
    {
        Owl,
        Mole,
        Stag
    }
    public myEnums familiarEnums;

    [Header("Temp - For Weave Rework")]
    public bool isNewWeave = false;
    public WeaveController weaveController;


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

        playerScript = player.GetComponent<PlayerController>();
        movementScript = player.GetComponent<MovementScript>();
        familiarScript = familiar.GetComponent<FamiliarScript>();
        pauseScript = pauseScreen.GetComponent<PauseScript>();
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
        jumpAndDashScript.DoJumpDash();
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
        if (input.isPressed)
        {
            if (!isNewWeave) // FOR OLD WEAVE
            {
                if (!playerScript.inRelocateMode && !playerScript.inCombineMode) // occasionally reads a hit during compile time???? NOT ANYMOREEEEEEE HEHEHEHEHE
                {
                    playerScript.interactInput = true;
                    playerScript.WeaveActivated();

                }
                else if (playerScript.inCombineMode)
                {
                    playerScript.weaveableScript.OnCombineInput();

                    //StartCoroutine(WeaveModeTimer());  //ayo peyton rework this                       
                }
            }
            else // FOR REWORKED WEAVE
            {
                if (weaveController.isWeaving)
                {
                    weaveController.currentWeaveable.AddToWovenObjects();
                }
                else
                {
                    weaveController.WeaveObject(isGamepad);
                }
            }
        }
    }

    IEnumerator WeaveModeTimer() // and this
    {
        yield return new WaitForSeconds(1);
        playerScript.inRelocateMode = true;
        playerScript.inCombineMode = false;
    }

    public void OnDrop(InputValue input)
    {
        if (input.isPressed)
        {
            if (!isNewWeave) // for old weave
            {
                if (playerScript.isCurrentlyWeaving)
                {
                    playerScript.uninteract = true;
                }

                playerScript.interactInput = false;
                playerScript.inRelocateMode = false;
                playerScript.inCombineMode = false;
            }
            else // for reworked weave
            {
                weaveController.OnDrop();
            }
        }
    }

    public void OnToggleWeaveMode(InputValue input) // no longer exists in reworked weave
    {
        if (input.isPressed)
        {
            if (!isNewWeave) // old weave functionality
            {
                if (playerScript.inRelocateMode)
                {
                    playerScript.inCombineMode = true;
                    playerScript.inRelocateMode = false;
                }
                else if (playerScript.inCombineMode && !playerScript.floatingIslandCrystal)
                {
                    playerScript.inCombineMode = false;
                    playerScript.inRelocateMode = true;
                }
            }
        }
    }

    public void OnWeaverTargeting(InputValue input)
    {
        Vector2 inputVector = input.Get<Vector2>();

        if (!isNewWeave) // OLD WEAVE FUNCTIONALITY
        {
            if (isGamepad)
            {
                if (playerScript.isCurrentlyWeaving)
                {
                    playerScript.weaveableScript.MovingWeaveController(inputVector);
                }

                if (inputVector != Vector2.zero)
                {
                    playerScript.ControllerAimTargetter(inputVector);
                }
            }
            else
            {
                playerScript.MouseAimTargetter(inputVector);
            }
        }
        else // REWORKED WEAVE FUNCTIONALITY
        {
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
    }

    public void OnWeaverNPCInteractions(InputValue input)
    {

    }

    public void OnRotate(InputValue input)
    {
        Vector2 dir = input.Get<Vector2>();

        if (dir != Vector2.zero && playerScript.inRelocateMode)
        {
            if (playerScript.weaveableScript != null)
            {
                switch (dir)
                {
                    case Vector2 v when v.Equals(Vector2.up):
                        {
                            //playerScript.weaveableScript.CallRotate(Vector3.forward, 45);
                            playerScript.weaveableScript.RotateObject(WeaveableNew.rotateDir.forward);
                            break;
                        }
                    case Vector2 v when v.Equals(Vector2.down):
                        {
                            //playerScript.weaveableScript.CallRotate(Vector3.forward, -45);
                            playerScript.weaveableScript.RotateObject(WeaveableNew.rotateDir.back);
                            break;
                        }
                    case Vector2 v when v.Equals(Vector2.right):
                        {
                            //playerScript.weaveableScript.CallRotate(Vector3.up, 45);
                            playerScript.weaveableScript.RotateObject(WeaveableNew.rotateDir.right);
                            break;
                        }
                    case Vector2 v when v.Equals(Vector2.left):
                        {
                            //playerScript.weaveableScript.CallRotate(Vector3.up, -45);
                            playerScript.weaveableScript.RotateObject(WeaveableNew.rotateDir.left);
                            break;
                        }
                }
            }
        }
    }

    public void OnUncombineAction(InputValue input)
    {
        if (input.isPressed)
        {
            if (!isNewWeave) // FOR OLD WEAVE
            {
                WeaveableNew[] weaveableArray = FindObjectsOfType<WeaveableNew>();

                foreach (WeaveableNew weaveable in weaveableArray)
                {
                    if (weaveable.isCombined)
                    {
                        weaveable.Uncombine();
                        playerScript.weaveVisualizer.StopAura(weaveable.gameObject);
                    }
                }
            }
            else // FOR REWORKED WEAVE
            {
                weaveController.weaveableManager.DestroyJoints(weaveController.currentWeaveable.listIndex);
            }
        }
    }

    #endregion//******************************************************

    #region//SWITCHING
    //******************************************************
    public void OnPossessFamiliar(InputValue input)
    {
        if (input.isPressed)
        {
            PossessFamiliar();
        }
    }

    public void PossessFamiliar()
    {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        CharacterController playerCharacterController = player.GetComponent<CharacterController>();

        if (!familiarScript.myTurn && !playerScript.isCurrentlyWeaving && playerCharacterController.isGrounded && !playerScript.inCutscene && canSwitch && !playerScript.talkingToNPC)
        {
            playerScript.Possession();
            playerInput.SwitchCurrentActionMap("Familiar");
        }
    }

    public void OnPossessWeaver(InputValue input)
    {
        if (input.isPressed)
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

    public void OnQuit(InputValue input)
    {
        if (input.isPressed)
        {
            Application.Quit();
        }
    }

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
                    Debug.Log("Pressed");
                    molePillarScript.riseInputPressed = true;
                    molePillarScript.lowerInputPressed = false;
                    molePillarScript.DeployPillar();
                    molePillarScript.rise = true;
                    molePillarScript.lower = false;
                }
                else
                {
                    Debug.Log("Removed");
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

    #region //MOLE FAMILIAR ABILITIES
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
