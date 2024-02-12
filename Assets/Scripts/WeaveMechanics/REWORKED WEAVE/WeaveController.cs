using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveController : MonoBehaviour
{
    [Header("For Input Manager - DO NOT MODIFY")]
    public bool isWeaving;
    public WeaveableObject currentWeaveable;
    public WeaveableManager weaveableManager;

    void Start()
    {
        weaveableManager = GameObject.FindWithTag("WeaveableManager").GetComponent<WeaveableManager>();
    }

    // adjusts targeting arrow based on gamepad
    // <param> the direction that the player is looking in
    public void GamepadTargetingArrow(Vector2 lookDir)
    {

    }

    // adjusts targeting arrow based on mouse position
    // <param> the direction that the player is looking in
    public void MouseTargetingArrow(Vector2 lookDir)
    {

    }

    // no other objects are being woven. weave this object. 
    public void WeaveObject()
    {

    }

    // drop objects. does not uncombine objects.
    public void OnDrop()
    {

    }
}
