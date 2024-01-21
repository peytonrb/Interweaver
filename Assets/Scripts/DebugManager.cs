using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public static bool debugIsOn = true;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public bool GetDebugOn() {
        return debugIsOn;
    }
}
