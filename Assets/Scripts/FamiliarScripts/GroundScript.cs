using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    //Camera returns to standard player view once the floating island has touched the ground.
    //This script may become obsolete in the future.
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "FloatingIsland") {
            FloatingIslandScript floatingIsland = other.gameObject.GetComponent<FloatingIslandScript>();
            //floatingIsland.ReturnCamera();
        }
    }
}
