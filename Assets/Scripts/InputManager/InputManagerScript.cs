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

    //Familiar Script only has player switch and player interact and familiar movement ability inputs.
    //Player Script handles a majority of inputs.
    //Dont listen to this ^^^ Its just me talking to myself...

    //MOVE
    //******************************************************
    public void Move(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();
    }

    public void WeaverLookandMove() {
        MovementScript movementScript = player.GetComponent<MovementScript>();
        movementScript.LookAndMove();
    }

    public void FamiliarLookandMove() {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        MovementScript movementScript = familiar.GetComponent<MovementScript>();
        if (familiarScript.myTurn) {
            movementScript.LookAndMove();
        }
    }
    //******************************************************

    //WEAVER ABILITIES
    //******************************************************
    public void WeaverInteract(InputAction.CallbackContext context) {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (context.started) {
            if (playerScript.enableInteractInput) {
                
                playerScript.interactInput = true;
                Debug.Log(playerScript.interactInput);
            }
        }
    }

    public void WeaverUninteract(InputAction.CallbackContext context) {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (context.started) {
            if (playerScript.enableUninteractInput) {
                playerScript.uninteractInput = true;
            }
        }
    }

    public void WeaveModeSwitch(InputAction.CallbackContext context) {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (context.started) {
            if (playerScript.enableWeaveModeSwitch) {
                playerScript.weaveModeSwitch = true;
            }
        } 
    }

    public void WeaveCursor(InputAction.CallbackContext context) {
        weaveCursor = context.ReadValue<Vector2>();
    }
    //******************************************************

    //SWITCHING
    //******************************************************
    public void Switch(InputAction.CallbackContext context) {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        if (context.started) {
            if (familiarScript.myTurn) {
                familiarScript.Depossess();
            } 
            else {
                playerScript.Possession();
            }
        }
    }
    //******************************************************

    //PAUSING
    //******************************************************
    public void Pause(InputAction.CallbackContext context) {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (context.started) {
            playerScript.Pausing();
        }
    }
    //******************************************************

    //OWL FAMILIAR ABILITIES
    //******************************************************
    public void OwlAbility(InputAction.CallbackContext context) {
        FamiliarScript familiarScript = familiar.GetComponent<FamiliarScript>();
        if (context.started) {
            //This has to be performed through a bool since this particular method is only activated through collisions
            familiarScript.familiarMovementAbility = true;
        }
        else {
            familiarScript.familiarMovementAbility = false;
        }
    }

    public void OwlDive(InputAction.CallbackContext context) {
        OwlDiveScript owlDiveScript = familiar.GetComponent<OwlDiveScript>();
        if (context.started) {
            owlDiveScript.divePressed = true;
        }
        else if (context.performed && !context.canceled) {
            owlDiveScript.diveHeld = true;
        }
        else if (context.canceled) {
            owlDiveScript.divePressed = false;
            owlDiveScript.diveHeld = false;
        }
    }
    //******************************************************


}
