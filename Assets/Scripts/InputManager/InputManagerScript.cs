using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class InputManagerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject familiar;
    public Vector2 movement;
    public Vector2 weaveCursor;
    public bool switching;

    public static InputManagerScript instance;


    private PlayerController playerScript;
    private FamiliarScript familiarScript;

    private PlayerInput playerInput;
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
        familiarScript = familiar.GetComponent<FamiliarScript>();

        playerInput = GetComponent<PlayerInput>();
        
    }

    #region//WEAVER ABILITIES
    //******************************************************

    //MOVE
    //******************************************************
    public void OnWeaverMove(InputValue input)
    {
        //Debug.Log("why");
        movement = input.Get<Vector2>();
        MovementScript movementScript = player.GetComponent<MovementScript>();
        movementScript.LookAndMove();
    }

    //******************************************************
    public void OnWeaverInteract(InputValue input)
    {

        if (input.isPressed)
        {
            if (!playerScript.inRelocateMode && !playerScript.inCombineMode) // occasionally reads a hit during compile time???? NOT ANYMOREEEEEEE HEHEHEHEHE
            {
                playerScript.interactInput = true;
                playerScript.WeaveActivated();

                //playerScript.inRelocateMode = true;
            }
            else if (playerScript.inCombineMode)
            {
                playerScript.weaveableScript.OnCombineInput();
                playerScript.inRelocateMode = true;
                playerScript.inCombineMode = false;
            }
        }
    }

    public void OnDrop(InputValue input)
    {
        if (input.isPressed)
        {
            playerScript.uninteract = true;
            playerScript.interactInput = false;
            playerScript.inRelocateMode = false;
            playerScript.inCombineMode = false;
        }
    }

    public void OnToggleWeaveMode(InputValue input)
    {
        if (input.isPressed)
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

    public void WeaveCursor(InputValue input)
    {
        weaveCursor = input.Get<Vector2>();
    }

    public void OnWeaverNPCInteractions(InputValue input)
    {
        if (input.isPressed)
        {
            NPCInteractionScript npcInteractScript = player.GetComponent<NPCInteractionScript>();
            if (input.isPressed)
            {
                npcInteractScript.Interact();
            }
        }
    }

    public void OnRotate(InputValue input)
    {

        Vector2 dir = input.Get<Vector2>();

        if (dir != Vector2.zero && playerScript.inRelocateMode)
        {
            if (playerScript.weaveableScript != null)
            {
                switch(dir)
                {
                    case Vector2 v when v.Equals(Vector2.up):
                    {
                        playerScript.weaveableScript.CallRotate(Vector3.up, 90);
                        break;
                    }
                    case Vector2 v when v.Equals(Vector2.down): 
                    {
                        playerScript.weaveableScript.CallRotate(Vector3.up, -90);
                        break; 
                    }
                    case Vector2 v when v.Equals(Vector2.right):
                    {
                        playerScript.weaveableScript.CallRotate(Vector3.forward, 90);
                        break;
                    }
                    case Vector2 v when v.Equals(Vector2.left):
                    {
                        playerScript.weaveableScript.CallRotate(Vector3.forward, -90);
                        break;
                    }
                }
            }
        }
    }

    public void OnUnCombineAction(InputValue input)
    {
        if (input.isPressed)
        {
            WeaveableNew[] weaveableArray = FindObjectsOfType<WeaveableNew>();

            foreach (WeaveableNew weaveable in weaveableArray)
            {
                if (weaveable.isCombined)
                {
                    weaveable.Uncombine();
                }
            }
        }
    }

    #endregion//******************************************************

    //SWITCHING
    //******************************************************
    public void OnPossessFamiliar(InputValue input)
    {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        CharacterController playerCharacterController = player.GetComponent<CharacterController>();

        if (input.isPressed)
        {
            if (!familiarScript.myTurn && playerCharacterController.isGrounded)
            {
                playerScript.Possession();
                playerInput.SwitchCurrentActionMap("Familiar");
                Debug.Log(playerInput.currentActionMap);
            }
        }
    }

    public void OnPossessWeaver(InputValue input)
    {
        if (input.isPressed)
        {
            FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
            CharacterController familiarCharacterController = familiar.GetComponent<CharacterController>();
            if (familiarScript.myTurn && familiarCharacterController.isGrounded)
            {
                familiarScript.Depossess();
                playerInput.SwitchCurrentActionMap("Weaver");
                Debug.Log(playerInput.currentActionMap);
            }
        }
    }

    //******************************************************

    //PAUSING
    //******************************************************
    public void Pause(InputValue input)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (input.isPressed)
        {
            playerScript.Pausing();
        }
    }
    //******************************************************

    #region //OWL FAMILIAR ABILITIES
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
        OwlDiveScript owlDiveScript = familiar.GetComponent<OwlDiveScript>();
        bool check = input.isPressed;

        if (check)
        {
            
            //This has to be performed through a bool since this particular method is only activated through collisions
            familiarScript.familiarMovementAbility = true;

            // Add a check per familiar for later
            owlDiveScript.DiveStart();
            owlDiveScript.DivePressed();
        }
        else
        {
            familiarScript.familiarMovementAbility = false;

            //Add a check per familiar for later
            owlDiveScript.DiveRelease();
        }
    }

    public void OnFamiliarNPCInteraction(InputAction input)
    {
        if (input.IsPressed())
        {
            NPCInteractionScript npcInteractScript = familiar.GetComponent<NPCInteractionScript>();
            npcInteractScript.Interact();
        }
    }
   
    #endregion//******************************************************
}
