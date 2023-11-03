using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveFXScript : MonoBehaviour
{
    [CannotBeNullObjectField]
    public LineRenderer weaveRenderer;
    
    [CannotBeNullObjectField]
    public ParticleSystem weaveActivation;

    void Start()
    {
        weaveRenderer.gameObject.SetActive(false);
        weaveActivation.Stop();
    }

    public void DrawWeave(Vector3 playerPos, Vector3 weaveablePos)
    {
        weaveRenderer.gameObject.SetActive(true);
        weaveRenderer.positionCount = 2;
        weaveRenderer.SetPosition(0, playerPos);
        weaveRenderer.SetPosition(1, weaveablePos);
    }

    public void DisableWeave()
    {
        weaveRenderer.gameObject.SetActive(false);
    }

    public void ActivateWeave()
    {
        Debug.Log("here");
        weaveActivation.Play();
    }
}
