using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject familiar;
    public Vector2 movement;
    public Vector2 weaveCursor;
    public bool switching;

    public static InputManagerScript instance;

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
    }

    //Familiar Script only has player switch and player interact and familiar movement ability inputs.
    //Player Script handles a majority of inputs.
    //Dont listen to this ^^^ Its just me talking to myself...


    //MOVE
    //******************************************************
    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void WeaverLookandMove()
    {
        MovementScript movementScript = player.GetComponent<MovementScript>();
        movementScript.LookAndMove();
    }

    public void FamiliarLookandMove()
    {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        MovementScript movementScript = familiar.GetComponent<MovementScript>();
        if (familiarScript.myTurn)
        {
            movementScript.LookAndMove();
        }
    }
    //******************************************************

    //WEAVER ABILITIES
    //******************************************************
    public void WeaverInteract(InputAction.CallbackContext context)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();         // old weave
        // if (context.started)
        // {
        //     if (playerScript.enableInteractInput)
        //     {
        //         playerScript.interactInput = true;
        //         Debug.Log(playerScript.interactInput);
        //     }
        // }

        PlayerController playerScript = player.GetComponent<PlayerController>();    // new weave

        if (context.started)
        {
            if (!playerScript.inRelocateMode && !playerScript.inCombineMode) // occasionally reads a hit during compile time????
            {
                playerScript.interactInput = true;
                playerScript.WeaveActivated();

                //playerScript.inRelocateMode = true;
            }
            else if (playerScript.inCombineMode)
            {
                playerScript.inRelocateMode = true;
                playerScript.inCombineMode = false;
            }
        }
    }

    public void WeaverUninteract(InputAction.CallbackContext context)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();     // old weave
        // if (context.started)
        // {
        //     if (playerScript.enableUninteractInput)
        //     {
        //         playerScript.uninteractInput = true;
        //     }
        // }

        PlayerController playerScript = player.GetComponent<PlayerController>(); // new weave
        if (context.started)
        {
            playerScript.uninteract = true;
            playerScript.interactInput = false;
            playerScript.inRelocateMode = false;
            playerScript.inCombineMode = false;
        }
    }

    public void WeaveModeSwitch(InputAction.CallbackContext context)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();     // old weave
        // if (context.started)
        // {
        //     if (playerScript.enableWeaveModeSwitch)
        //     {
        //         playerScript.weaveModeSwitch = true;
        //     }
        // }

        PlayerController playerScript = player.GetComponent<PlayerController>(); // new weave
        if (context.started)
        {
            Debug.Log("here");
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

    public void WeaveCursor(InputAction.CallbackContext context)
    {
        weaveCursor = context.ReadValue<Vector2>();
    }

    public void NPCInteraction(InputAction.CallbackContext context)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();
        PlayerController playerScript = player.GetComponent<PlayerController>();
        if (context.started)
        {
            playerScript.Interact();
        }
    }
    //******************************************************

    //SWITCHING
    //******************************************************
    public void Switch(InputAction.CallbackContext context)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();
        PlayerController playerScript = player.GetComponent<PlayerController>();
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        if (context.started)
        {
            if (familiarScript.myTurn)
            {
                familiarScript.Depossess();
            }
            else
            {
                playerScript.Possession();
            }
        }
    }
    //******************************************************

    //PAUSING
    //******************************************************
    public void Pause(InputAction.CallbackContext context)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();
        PlayerController playerScript = player.GetComponent<PlayerController>();
        if (context.started)
        {
            playerScript.Pausing();
        }
    }
    //******************************************************

    //OWL FAMILIAR ABILITIES
    //******************************************************
    public void OwlAbility(InputAction.CallbackContext context)
    {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        if (context.started)
        {
            //This has to be performed through a bool since this particular method is only activated through collisions
            familiarScript.familiarMovementAbility = true;
        }
        else
        {
            familiarScript.familiarMovementAbility = false;
        }
    }

    public void OwlDive(InputAction.CallbackContext context)
    {
        OwlDiveScript owlDiveScript = familiar.GetComponent<OwlDiveScript>();
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        if (context.started)
        {
            owlDiveScript.DiveStart();
            familiarScript.familiarMovementAbility = true;

        }
        else if (context.performed && !context.canceled)
        {
            owlDiveScript.DivePressed();
            familiarScript.familiarMovementAbility = true;
        }
        else if (context.canceled)
        {
            owlDiveScript.DiveRelease();
            familiarScript.familiarMovementAbility = false;
        }
    }
    //******************************************************


}
