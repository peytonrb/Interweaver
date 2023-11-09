using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDetectionScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //this code definitely could use improvement. No way to call controls change mid-game
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            InputManagerScript.instance.isGamepad = true;
        }
        else if (Keyboard.current != null || Mouse.current != null)
        {
            //Debug.Log("k&M");
            InputManagerScript.instance.isGamepad = false;
        }
    }

}
