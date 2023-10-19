using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject familiar;
    public Vector2 movement;
    public bool switching;

    //Familiar Script only has player switch and player interact and familiar movement ability inputs.
    //Player Script handles a majority of inputs.

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

    public void Pause(InputAction.CallbackContext context) {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (context.started) {
            playerScript.Pausing();
        }
    }

    

}
