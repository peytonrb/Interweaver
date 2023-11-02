using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveFXScript : MonoBehaviour
{
    [CannotBeNullObjectField]
    public LineRenderer weaveRenderer;

    void Start()
    {
        weaveRenderer.gameObject.SetActive(false);
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
}
