using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "FloatingIsland") {
            FloatingIslandScript floatingIsland = other.gameObject.GetComponent<FloatingIslandScript>();
            floatingIsland.ReturnCamera();
        }
    }
}
